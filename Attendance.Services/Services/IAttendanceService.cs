using Attendance.Infrastructure.Data;
using Attendance.Models;
using Attendance.Services.Providers;
using Attendance.Services.ViewModels;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Attendance.Services.Services
{
    public interface IAttendanceService
    {
       
        PaginationViewModel<AttendanceDetailItem> GetAttendanceDetail(int vetId, int? page, bool? positive, bool? negative, bool? remark, int? qid);
        List<VIQDetailItem> GetVIQDetail(int qID);
        IDictionary<int, string> GetQids(int vetId);
        Task<FileInfo> GetPDFDocument(int vetId, int? page, bool? positive, bool? negative, bool? remark, int? qid);
        ObservationInfo GetObservation(int vetID, string qids, string objectids);
        Task<ObservationAssigneeInfo> GetObservationAssigneeAsync(int observationID);
        Task<List<ObservationNoc>> GetObservationNocListAsync(int observationID);
        Task<List<ObservationSoc>> GetObservationSocListAsync(int observationID);

    }

    public class AttendanceService : IAttendanceService
    {
        private readonly AttendanceContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly ISqlConnectionProvider _sqlConnect;
        private readonly IRazorViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IWebHostEnvironment _env;

        public AttendanceService(
            AttendanceContext dbContext
            , IRazorViewEngine viewEngine
            , IConfiguration configuration
            , ISqlConnectionProvider sqlConnect
            , ITempDataProvider tempDataProvider
            , IActionContextAccessor actionContextAccessor
            , IWebHostEnvironment env
            )
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _sqlConnect = sqlConnect;
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
            _actionContextAccessor = actionContextAccessor;
            _env = env;
        }

        public async Task<FileInfo> GetPDFDocument(int vetId, int? page, bool? positive, bool? negative, bool? remark, int? qid)
        {

            var info = await _dbContext.VettingInfo
            .Include(x => x.InspectionType)
            .FirstOrDefaultAsync(m => m.VetId == vetId);

            if (info == null)
            {
                throw new InvalidOperationException("Vet id not found!");
            }

            var regDate = info.RegistrationDate?.ToString("dd-MM-yyyy");

            var title = $"Attendance {regDate} - {info.VesselName} ({info.InspectionType?.InspectionType})";

            var model = GetAttendanceDetail(vetId, page, positive, negative, remark, qid);

            var viewPath = "~/Pages/VettingInfo/_Export.cshtml";

            var viewEngineResult = _viewEngine.GetView(null, viewPath, false);

            if (!viewEngineResult.Success)
            {
                throw new InvalidOperationException("Invalid view path!");
            }

            var view = viewEngineResult.View;

            using (var output = new StringWriter())
            {
                var tempData = new TempDataDictionary(
                         _actionContextAccessor.ActionContext.HttpContext,
                         _tempDataProvider)
                {
                    { "HiddenQuestionnaire", qid.HasValue },
                    { "Title", title },
                };
                var viewContext = new ViewContext(
                    _actionContextAccessor.ActionContext,
                    view,
                    new ViewDataDictionary<PaginationViewModel<AttendanceDetailItem>>(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                    { Model = model },
                    tempData,
                    output,
                    new HtmlHelperOptions());

                await view.RenderAsync(viewContext);

                var fileInfo = await PDFConverter.Convert(Path.Combine(_env.ContentRootPath), output.ToString());

                string newfilePath = Path.Combine(fileInfo.Directory.FullName, $"{title}.pdf");

                fileInfo.MoveTo(newfilePath);

                return fileInfo;
            }
        }

        public PaginationViewModel<AttendanceDetailItem> GetAttendanceDetail(int vetId, int? page, bool? positive, bool? negative, bool? remark, int? qid)
        {

            var pagedData = new PaginationViewModel<AttendanceDetailItem>(page);

            try
            {
                string positiveFilter = "";
                string negativeFilter = "";
                string remarkFilter = "";

                if (positive.HasValue && positive.Value)
                {
                    positiveFilter = " AND B.answer = 1";
                }

                if (negative.HasValue && negative.Value)
                {
                    negativeFilter = " AND B.answer = 2";
                }

                if (positive.HasValue && positive.Value && negative.HasValue && negative.Value)
                {
                    negativeFilter = "";
                    positiveFilter = "AND (B.answer = 1 Or B.answer = 2)";
                }

                if (remark.HasValue && remark.Value)
                {
                    remarkFilter = " AND B.comments <> ''";
                }

                var qids = string.Join(',', GetQids(vetId).Keys);

                if (!string.IsNullOrEmpty(qids))
                {
                    qids = $" ({qids}) ";
                }

                if (qid.HasValue)
                {
                    qids = $" ({qid}) ";
                }

                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("AttendanceContext")))
                {
                    connection.Open();
                    string query = @$"
SELECT
{{0}}
FROM vw_viq_union a
LEFT JOIN VETTING B ON A.OBJECTID=B.OBJECTID
LEFT JOIN Significance C ON C.SignificanceId = B.significance
LEFT JOIN viqinfo i ON i.qid = a.qid
WHERE a.qid IN {qids} 
and B.VETID={vetId}
and (b.qid is null or b.qid = a.qid)
{positiveFilter} {negativeFilter} {remarkFilter}
{{1}}
{{2}}
";
                    var columns = $@"
a.DisplayCode,
a.defaultcode,
a.ObjectDescription,
a.ObjectComments,
a.ObjectId,
a.qid,
b.AssigneeRank,
b.VerifiedinVetting,
b.answer,
b.significance,
c.significance AS SignificanceName,
b.comments,
i.title
";
                    var paginationQuery = "";
                    var commandQuery = string.Format(query, "count(*)", "", "");
                    if (page.HasValue)
                    {
                        using (var command = new SqlCommand(commandQuery, connection))
                        {
                            var total = long.Parse(command.ExecuteScalar().ToString());
                            pagedData.TotalRecords = total;
                            paginationQuery = $"OFFSET (({pagedData.PageNumber} - 1) * {pagedData.PageSize}) ROWS FETCH NEXT {pagedData.PageSize} ROWS ONLY;";
                        }

                    }
                    using (SqlCommand command = new SqlCommand(string.Format(query, columns, "ORDER BY A.GLOBALDISPLAYINDEX, a.qid", paginationQuery), connection))
                    {
                        command.ExecuteNonQuery();
                        using (var reader = command.ExecuteReader())
                        {
                            var resultList = new List<AttendanceDetailItem>();
                            while (reader.Read())
                            {
                                AttendanceDetailItem item = new AttendanceDetailItem();
                                item.DisplayCode = (string)reader["DisplayCode"];
                                item.DefaultCode = (string)reader["defaultcode"];
                                item.CategoryQuestion = (string)reader["ObjectDescription"];
                                item.Answer = reader["answer"] == DBNull.Value ? "" : (string)reader["answer"];
                                item.Significance = reader["significance"] == DBNull.Value ? 0 : (int)reader["significance"]; // a.goulielmos 10/12/2020
                                item.SignificanceName = reader["SignificanceName"] == DBNull.Value ? "" : (string)reader["SignificanceName"];
                                item.comments = reader["comments"] == DBNull.Value ? "" : (string)reader["comments"];
                                item.CategoryQuestionComment = reader["ObjectComments"] == DBNull.Value ? "" : (string)reader["ObjectComments"];
                                item.CategoryQuestionDescription = reader["ObjectDescription"] == DBNull.Value ? "" : (string)reader["ObjectDescription"];
                                item.ObjectId = reader["ObjectId"] == DBNull.Value ? Guid.Empty : (Guid)reader["ObjectId"];
                                item.qid = reader["qid"] == DBNull.Value ? null : (int?)reader["qid"];
                                item.title = (string)reader["title"];
                                item.AssigneeRank = reader["AssigneeRank"] == DBNull.Value ? null : (byte?)reader["AssigneeRank"];
                                item.VerifiedinVetting = reader["VerifiedinVetting"] == DBNull.Value ? null : (int?)reader["VerifiedinVetting"];
                                resultList.Add(item);
                            }
                            pagedData.Data = resultList;

                        }
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }

            return pagedData;
        }

        public IDictionary<int, string> GetQids(int vetId)
        {
            var qids = new Dictionary<int, string>();
            var defaultQid = _dbContext.VettingInfo.Where(_ => _.VetId == vetId).Select(_ => _.QId).First();
            var title = _dbContext.VIQInfo.Where(_ => _.QId == defaultQid).Select(_ => _.Title).First();
            qids.Add(defaultQid, title);
            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("AttendanceContext")))
                {
                    connection.Open();
                    string query = $"SELECT DISTINCT v.qid, i.Title FROM VETTING v LEFT JOIN VIQInfo i ON i.Qid = v.qid WHERE v.qid is not null AND v.qid <> {defaultQid} AND v.VETID = {vetId}";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                qids.Add((int)reader["qid"], (string)reader["Title"]);
                            }
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return qids;
        }

        public List<VIQDetailItem> GetVIQDetail(int qID)
        {
            List<VIQDetailItem> resultList = new List<VIQDetailItem>();
            List<VIQDetailItem> orderedResultList = new List<VIQDetailItem>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("AttendanceContext")))
                {
                    connection.Open();

                    var qid = qID;
                    string query = $"select v.Id, v.ObjectId, v.GlobalDisplayIndex, v.ShowAfterId, q.questioncode 'Code', q.question 'Text', v.ObjectType, 'Question' 'Type' from viq v, QuestionPoolNew q where v.ObjectId = q.questionid and v.QId ={qid} union all select v.Id, v.ObjectId, v.GlobalDisplayIndex, v.ShowAfterId, c.CategoryCode 'Code', c.CategoryDescription 'Text', v.ObjectType, 'Category' 'Type' from viq v, category c where v.ObjectId = c.CategoryNewID and v.QId={qid} order by ShowAfterId";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                        Console.WriteLine("Done.");


                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                VIQDetailItem item = new VIQDetailItem();
                                item.Id = (int)reader["Id"];
                                item.ObjectId = (Guid)reader["ObjectId"];
                                item.Code = (string)reader["Code"];
                                item.Description = (string)reader["Text"];
                                item.ObjectType = (int)reader["ObjectType"];
                                item.Type = (string)reader["Type"];
                                item.GlobalDisplayIndex = reader["GlobalDisplayIndex"] == DBNull.Value ? null : (int?)reader["GlobalDisplayIndex"];
                                item.ShowAfterId = reader["ShowAfterId"] == DBNull.Value ? null : (int?)reader["ShowAfterId"];
                                resultList.Add(item);
                            }
                        }
                    }
                }
                if (resultList.Count >= 1)
                {
                    orderedResultList.Add(resultList[0]);
                    for (int i = 0; i < resultList.Count; i++)
                    {
                        for (int k = 1; k < resultList.Count; k++)
                        {
                            if (orderedResultList[i].GlobalDisplayIndex == resultList[k].ShowAfterId)
                            {
                                orderedResultList.Add(resultList[k]);

                                if (orderedResultList.Count == resultList.Count)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    orderedResultList = resultList;
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return orderedResultList;
        }

        public ObservationInfo GetObservation(int vetID, string qids, string objectids)
        {
            var strSire2ids = string.Empty;
            SqlConnection connection = null;
            ObservationInfo ObservationsResult = new ObservationInfo()
            {
                listObservationsSire2 = new List<ObservationsSire2ViewModel>(),
                listObservationsSire2Assignees = new List<ObservationsSire2Assignees>(),
                listObservationsSire2Attachments = new List<ObservationsSire2Attachments>()
            };
            try
            {
                using (connection = new SqlConnection(_configuration.GetConnectionString("AttendanceContext")))
                {
                    connection.Open();
                    string queryObsSire2 = $"SELECT obs.id,obs.vetid,obs.qid,obs.objectid,obs.part_id,obs.obs_text,obs.answer_id,obs.significance_id,obs.solved_onboard" +
                                   $",obs.verified FROM ObservationsSire2 AS obs WHERE obs.vetid = {vetID} AND obs.objectid IN(SELECT value FROM STRING_SPLIT('{objectids}', ',') WHERE RTRIM(value) <> '')" +
                                    $"AND obs.qid IN(SELECT value FROM STRING_SPLIT('{qids}', ',') WHERE RTRIM(value) <> '')";

                    using (SqlCommand command = new SqlCommand(queryObsSire2, connection))
                    {
                        command.ExecuteNonQuery();
                        using SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            ObservationsSire2ViewModel item = new ObservationsSire2ViewModel
                            {
                                id = reader["id"] != DBNull.Value ? (int)reader["id"] : 0,
                                vet_id = reader["vetid"] != DBNull.Value ? (int)reader["vetid"] : 0,
                                qid = reader["qid"] != DBNull.Value ? (int)reader["qid"] : 0,
                                objectid = reader["objectid"] != DBNull.Value ? (Guid?)reader["objectid"] : null,
                                obs_text_full = reader["obs_text"] != DBNull.Value ? (string)reader["obs_text"] : string.Empty,
                                verified = reader["verified"] != DBNull.Value ? (int)reader["verified"] : 0,
                            };
                            ObservationsResult.listObservationsSire2.Add(item);
                        }
                    }

                    if (ObservationsResult.listObservationsSire2.Count > 0)
                    {
                        strSire2ids = string.Join(",", ObservationsResult.listObservationsSire2.Select(p => p.id).ToList());
                        string queryObsAssignee = $"SELECT obsAssignee.id,obsAssignee.obs_id,obsAssignee.assignee_id,obsAssignee.rank_id FROM ObservationsSire2Assignees AS obsAssignee WHERE  obsAssignee.obs_id IN (SELECT value FROM STRING_SPLIT('{strSire2ids}', ',') WHERE RTRIM(value) <> '')";

                        using SqlCommand command = new SqlCommand(queryObsAssignee, connection);
                        command.ExecuteNonQuery();
                        using SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            ObservationsSire2Assignees itemAssignee = new ObservationsSire2Assignees
                            {
                                id = reader["id"] != DBNull.Value ? (int)reader["id"] : 0,
                                assignee_id = reader["assignee_id"] != DBNull.Value ? (int)reader["assignee_id"] : 0,
                                obs_id = reader["obs_id"] != DBNull.Value ? (int)reader["obs_id"] : 0,
                                rank_id = reader["rank_id"] != DBNull.Value ? (int)reader["rank_id"] : 0,
                            };
                            ObservationsResult.listObservationsSire2Assignees.Add(itemAssignee);
                        }
                    }

                    if (ObservationsResult.listObservationsSire2.Count > 0)
                    {
                        string queryObsAttachment = $"SELECT obsAttachment.id,obsAttachment.obs_id,obsAttachment.attachment_name,obsAttachment.attachment FROM ObservationsSire2Attachments AS obsAttachment WHERE obsAttachment.obs_id IN (SELECT value	FROM STRING_SPLIT('{strSire2ids}', ',') WHERE RTRIM(value) <> '')";
                        using SqlCommand command = new SqlCommand(queryObsAttachment, connection);
                        command.ExecuteNonQuery();
                        using SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            ObservationsSire2Attachments itemAttachment = new ObservationsSire2Attachments
                            {
                                id = reader["id"] != DBNull.Value ? (int)reader["id"] : 0,
                                obs_id = reader["obs_id"] != DBNull.Value ? (int)reader["obs_id"] : 0,
                                attachment_name = reader["attachment_name"] != DBNull.Value ? (string)reader["attachment_name"] : string.Empty,
                                attachment = reader["attachment"] != DBNull.Value ? (byte[])reader["attachment"] : null,
                            };
                            ObservationsResult.listObservationsSire2Attachments.Add(itemAttachment);
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                connection.Close();
            }
            return ObservationsResult;
        }
        public async Task<ObservationAssigneeInfo> GetObservationAssigneeAsync(int observationID)
        {
            ObservationAssigneeInfo ObservationAssigneeResult = new ObservationAssigneeInfo()
            {
                listAssigneeViewModel = new List<AssigneeViewModel>(),
                ObservationID = 0,
                OnservationText = string.Empty,
                VetID = 0
            };
            try
            {
                var observationObj = await _dbContext.ObservationsSire2.FirstOrDefaultAsync(p => p.id == observationID) ?? new ObservationsSire2();
                ObservationAssigneeResult.ObservationID = observationObj.id;
                ObservationAssigneeResult.VetID = observationObj?.vetid;
                ObservationAssigneeResult.OnservationText = observationObj?.obs_text;

                using (var connection = new SqlConnection(_configuration.GetConnectionString("AttendanceContext")))
                {
                    connection.Open();

                    string queryObsAssignee = $"SELECT REPLACE(cw.LASTNAME + ' ' + cw.FIRSTNAME,'\"\','') AS AssigneeFullName, rnk.rank as OfficerName FROM " +
                        $"ObservationsSire2Assignees as assign LEFT JOIN cwcrew as cw on assign.assignee_id = cw.id LEFT JOIN Ranks as rnk " +
                        $"on assign.rank_id = rnk.id WHERE assign.obs_id = {observationID}";
                    using SqlCommand command = new SqlCommand(queryObsAssignee, connection);
                    command.ExecuteNonQuery();
                    using SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        AssigneeViewModel itemAssignee = new AssigneeViewModel
                        {
                            AssigneeFullName = reader["AssigneeFullName"] != DBNull.Value ? (string)reader["AssigneeFullName"] : string.Empty,
                            OfficerName = reader["OfficerName"] != DBNull.Value ? (string)reader["OfficerName"] : string.Empty,
                        };
                        ObservationAssigneeResult.listAssigneeViewModel.Add(itemAssignee);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return ObservationAssigneeResult;
        }
        public enum NocSocType
        {
            HUMAN = 1,
            PROCESS = 2,
            HARDWARE = 3
        }
        public async Task<List<ObservationNoc>> GetObservationNocListAsync(int observationID)
        {
            List<ObservationNoc> observationNocSocList = new List<ObservationNoc>();

            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("AttendanceContext")))
                {
                    await connection.OpenAsync();
                    string query = @"
                            SELECT
                                hn.PifId AS HumanNOCPifId, hn.PifName AS HumanNOCPifName,
                                pn.id AS ProcessNOCId, pn.noc AS ProcessNOCNOC,
                                ha.id AS HardwareCauseAnalysisSire2Id, ha.noc AS HardwareCauseAnalysisSire2NOC
                            FROM
                                ObservationsSire2NOC onc
                            LEFT JOIN
                                HumanNOC hn ON hn.PifId = onc.noc_id AND onc.noc_type = @HumanNocType
                            LEFT JOIN
                                ProcessNOC pn ON pn.id = onc.noc_id AND onc.noc_type = @ProcessNocType
                            LEFT JOIN
                                HardwareCauseAnalysisSire2 ha ON ha.id = onc.noc_id AND onc.noc_type = @HardwareNocType
                            WHERE
                                onc.obs_id = @ObservationId
                            ;";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ObservationId", observationID);
                        command.Parameters.AddWithValue("@HumanNocType", (int)NocSocType.HUMAN);
                        command.Parameters.AddWithValue("@ProcessNocType", (int)NocSocType.PROCESS);
                        command.Parameters.AddWithValue("@HardwareNocType", (int)NocSocType.HARDWARE);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                ObservationNoc observationNocSocResult = new ObservationNoc()
                                {
                                    ObservationID = observationID,
                                };

                                // Set properties based on the current record in the reader
                                observationNocSocResult.HumanNOCPifName = reader["HumanNOCPifName"] != DBNull.Value ? reader["HumanNOCPifName"].ToString() : string.Empty;
                                observationNocSocResult.ProcessNOCNoc = reader["ProcessNOCNOC"] != DBNull.Value ? reader["ProcessNOCNOC"].ToString() : string.Empty;
                                observationNocSocResult.HardwareCauseAnalysisSire2Noc = reader["HardwareCauseAnalysisSire2NOC"] != DBNull.Value ? reader["HardwareCauseAnalysisSire2NOC"].ToString() : string.Empty;

                                observationNocSocList.Add(observationNocSocResult);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return observationNocSocList;
        }

        public async Task<List<ObservationSoc>> GetObservationSocListAsync(int observationID)
        {
            List<ObservationSoc> observationNocSocList = new List<ObservationSoc>();

            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("AttendanceContext")))
                {
                    await connection.OpenAsync();
                    
                    string query = @"
                                    SELECT
                                        hs.id AS HumanSOCId, hs.rank AS HumanSOCRank,
                                        ps.id AS ProcessSOCId, ps.level1 AS ProcessSOCLevel1, ps.level2 AS ProcessSOCLevel2, ps.level3 AS ProcessSOCLevel3, ps.level4 AS ProcessSOCLevel4,
                                        hc.id AS HardwareClassificationCodingSire2Id, hc.level1 AS HardwareClassificationCodingSire2Level1, hc.level2 AS HardwareClassificationCodingSire2Level2, hc.level3 AS HardwareClassificationCodingSire2Level3
                                    FROM
                                        ObservationsSire2SOC osc
                                    LEFT JOIN
                                        HumanSOC hs ON hs.id = osc.soc_id AND osc.soc_type = @HumanNocType
                                    LEFT JOIN
                                        ProcessSOC ps ON ps.id = osc.soc_id AND osc.soc_type = @ProcessNocType
                                    LEFT JOIN
                                        HardwareClassificationCodingSire2 hc ON hc.id = osc.soc_id AND osc.soc_type = @HardwareNocType
                                    WHERE
                                        osc.obs_id = @ObservationId   
                                    ;";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ObservationId", observationID);
                        command.Parameters.AddWithValue("@HumanNocType", (int)NocSocType.HUMAN);
                        command.Parameters.AddWithValue("@ProcessNocType", (int)NocSocType.PROCESS);
                        command.Parameters.AddWithValue("@HardwareNocType", (int)NocSocType.HARDWARE);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                ObservationSoc observationNocSocResult = new ObservationSoc()
                                {
                                    ObservationID = observationID,
                                };

                                // Set properties based on the current record in the reader
                                observationNocSocResult.HumanSOCRank = reader["HumanSOCRank"] != DBNull.Value ? reader["HumanSOCRank"].ToString() : string.Empty;
                                observationNocSocResult.ProcessSOCLevel1 = reader["ProcessSOCLevel1"] != DBNull.Value ? reader["ProcessSOCLevel1"].ToString() : string.Empty;
                                observationNocSocResult.ProcessSOCLevel2 = reader["ProcessSOCLevel2"] != DBNull.Value ? reader["ProcessSOCLevel2"].ToString() : string.Empty;
                                observationNocSocResult.ProcessSOCLevel3 = reader["ProcessSOCLevel3"] != DBNull.Value ? reader["ProcessSOCLevel3"].ToString() : string.Empty;
                                observationNocSocResult.ProcessSOCLevel4 = reader["ProcessSOCLevel4"] != DBNull.Value ? reader["ProcessSOCLevel4"].ToString() : string.Empty;
                                observationNocSocResult.HardwareCCodingSire2SOCLevel1 = reader["HardwareClassificationCodingSire2Level1"] != DBNull.Value ? reader["HardwareClassificationCodingSire2Level1"].ToString() : string.Empty;
                                observationNocSocResult.HardwareCCodingSire2SOCLevel2 = reader["HardwareClassificationCodingSire2Level2"] != DBNull.Value ? reader["HardwareClassificationCodingSire2Level2"].ToString() : string.Empty;
                                observationNocSocResult.HardwareCCodingSire2SOCLevel3 = reader["HardwareClassificationCodingSire2Level3"] != DBNull.Value ? reader["HardwareClassificationCodingSire2Level3"].ToString() : string.Empty;

                                observationNocSocList.Add(observationNocSocResult);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return observationNocSocList;
        }

    }
}
