using Attendance.Models;
using Attendance.Services.Extensions;
using Attendance.Services.Providers;
using Attendance.Services.ViewModels;
using Attendance.Services.ViewModels.Briedcases;
using Attendance.Services.ViewModels.VettingInfos;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlServerCe;
using System.IO;
using System.Threading.Tasks;

namespace Attendance.Services.Services
{
    /// <summary>
    /// Create Briefcase service
    /// </summary>
    public interface IBriefcaseService
    {
        /// <summary>
        /// Export Data Models to .SDF file
        /// </summary>
        /// <param name="model">Data model</param>
        /// <returns>.Sdf file content</returns>
        Task<FileInfo> ExportAsync(ExportViewModel model);

        /// <summary>
        /// Export Data Models to .SDF file
        /// </summary>
        /// <param name="model">Data model</param>
        /// <returns>.Sdf file content</returns>
        Task ImportAsync(string dataSource, string sdfPassword, params ImportVettingInfoViewModel[] models);

        /// <summary>
        /// Import .SDF file to Data Model
        /// </summary>
        /// <param name="model">Data model</param>
        /// <param name="sdfPassword">password for .SDF file</param>
        /// <returns>Data model list</returns>
        Task<IEnumerable<VIQInfoModel>> ExtractFromDBAsync(UploadFileModel model, string sdfPassword);
    }

    public class BriefcaseService : IBriefcaseService
    {
        private readonly ISqlConnectionProvider _sqlConnect;
        private readonly ISqlCeEngineProvider _sqlCeEngineProvider;
        private readonly ISqlSettingBuilder _connectionBuilder;
        private readonly IVettingInfosService _vettingInfosService;
        private readonly IConfiguration _configuration;

        public BriefcaseService(
            ISqlCeEngineProvider sqlCeEngineProvider
            , ISqlConnectionProvider sqlConnect
            , ISqlSettingBuilder connectionBuilder
            , IVettingInfosService vettingInfosService
            , IConfiguration configuration
            )
        {
            _sqlConnect = sqlConnect;
            _sqlCeEngineProvider = sqlCeEngineProvider;
            _connectionBuilder = connectionBuilder;
            _vettingInfosService = vettingInfosService;
            _configuration = configuration;
        }

        public async Task<FileInfo> ExportAsync(ExportViewModel model)
        {
            var dataSource = InstanceExportFileName(model.UserName, model.VettingType);

            //Create Database
            await CreateDatabaseAsync(model, dataSource.DirectoryName, dataSource.Name);

            // Insert selected questionnaires to SDF file
            await TransferBriefcaseData(model.VIQInfoModels);

            _sqlConnect.Close();

            return dataSource;

        }

        public async Task<IEnumerable<VIQInfoModel>> ExtractFromDBAsync(UploadFileModel model, string sdfPassword)
        {
            string fileName = Path.Combine(Path.GetTempPath(), model.FileName);
            await SaveSDFUploadFileToLocalAsync(model, fileName);

            _sqlConnect.Open(_connectionBuilder.BuildCompactConnectionString(fileName, sdfPassword));
            string vIQInfoQuery = "SELECT * FROM VIQInfo";

            var table = await _sqlConnect.QueryAsync(vIQInfoQuery);

            return table.ToObjects<VIQInfoModel>();
        }

        public async Task ImportAsync(string dataSource, string sdfPassword, params ImportVettingInfoViewModel[] models)
        {
            foreach (var model in models)
                await _vettingInfosService.CreateAsync(model, dataSource, sdfPassword);
        }

        #region Helpers

        private async Task CreateDatabaseAsync(ExportViewModel model, string filePath, string fileName)
        {
            var fullPath = Path.Combine(filePath, fileName);
            // Init Database
            var validate = _sqlCeEngineProvider.ValidateDataSource(fullPath);

            _sqlCeEngineProvider.CreateDatabase(fullPath, model.ExportPassword);
            _sqlConnect.Open(_sqlCeEngineProvider.CurrentConnectString);

            // Init Schemas
            string[] lines = AppResource.SQL_Create_Export_DB.Split(
               new[] { "\r\n", "\r", "\n" },
               StringSplitOptions.None
            );
            foreach (var line in lines)
                await _sqlConnect.CommandAsync(line);

            // Init Data
            await _sqlConnect.CommandAsync(AppResource.SQL_Create_Init_VersionCode);
            await _sqlConnect.CommandAsync(AppResource.SQL_Create_Init_VisitLog
                , new SqlCeParameter("@p0", SqlDbType.DateTime) { Value = DateTime.Parse("1753-01-01") }
                , new SqlCeParameter("@p1", SqlDbType.DateTime) { Value = DateTime.Parse("9999-12-31") }
                , new SqlCeParameter("@p2", SqlDbType.Int) { Value = 0 }
                , new SqlCeParameter("@p3", SqlDbType.Int) { Value = 30 }
                , new SqlCeParameter("@p5", SqlDbType.NVarChar) { Value = fileName }
                );
            await _sqlConnect.CommandAsync(AppResource.SQL_Create_Init_User,
                 new SqlCeParameter("@p0", SqlDbType.NVarChar) { Value = model.UserName }
               );

            await InitData();
        }

        private async Task InitData()
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("AttendanceContext")))
            {
                connection.Open();
                Console.WriteLine("SqlConnection Done.");

                //InspectionTypes              
                string query = $"Select InspectionTypeId, InspectionType, InspectionCode from InspectionTypes";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("Done.");


                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var parameters1 = new SqlCeParameter[]
                            {
                                new SqlCeParameter ("@p0" ,(object)reader["InspectionTypeId"] ?? DBNull.Value),
                                new SqlCeParameter ("@p1" ,(object)reader["InspectionType"] ?? DBNull.Value),
                                new SqlCeParameter ("@p2" ,(object)reader["InspectionCode"] ?? DBNull.Value),
                            };

                            await _sqlConnect.CommandAsync("Insert into InspectionTypes (InspectionTypeId, InspectionType, InspectionCode) values (@p0, @p1, @p2)", parameters1);
                        }
                    }
                }

                //Vessel              
                query = "Select VesselId, VesselName, Imo, flag, deliverydate, Grosstonage, deadweight, VesselCode" +
                                " from Vessel";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("Done.");


                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var parameters1 = new SqlCeParameter[]
                            {
                                new SqlCeParameter ("@p0" ,(object)reader["VesselId"] ?? DBNull.Value),
                                new SqlCeParameter ("@p1" ,(object)reader["VesselName"] ?? DBNull.Value),
                                new SqlCeParameter ("@p2" ,(object)reader["Imo"] ?? DBNull.Value),
                                new SqlCeParameter ("@p3" ,(object)reader["flag"] ?? DBNull.Value),
                                new SqlCeParameter ("@p4" ,(object)reader["deliverydate"] ?? DBNull.Value),
                                new SqlCeParameter ("@p5" ,(object)reader["Grosstonage"] ?? DBNull.Value),
                                new SqlCeParameter ("@p6" ,(object)reader["deadweight"] ?? DBNull.Value),
                                new SqlCeParameter ("@p7" ,(object)reader["VesselCode"] ?? DBNull.Value)
                            };

                            await _sqlConnect.CommandAsync("Insert into Vessel (VesselId, VesselName, Imo, flag, deliverydate, Grosstonage, deadweight, VesselCode) " +
                                  " values (@p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7)", parameters1);
                        }
                    }
                }

                //InspectionSource              
                query = "Select SOURCENAME, SOURCECODE, INSPECTIONSOURCEID, typecode from InspectionSource";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("Done.");


                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var parameters1 = new SqlCeParameter[]
                            {
                                new SqlCeParameter ("@p0" ,(object)reader["SOURCENAME"] ?? DBNull.Value),
                                new SqlCeParameter ("@p1" ,(object)reader["SOURCECODE"] ?? DBNull.Value),
                                new SqlCeParameter ("@p2" ,(object)reader["INSPECTIONSOURCEID"] ?? DBNull.Value)
                            };

                            await _sqlConnect.CommandAsync("Insert into InspectionSource (SOURCENAME, SOURCECODE, INSPECTIONSOURCEID) values (@p0, @p1, @p2)", parameters1);
                        }
                    }
                }
            }
        }

        private async Task TransferBriefcaseData(params VIQInfoModel[] models)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("AttendanceContext")))
            {
                connection.Open();
                Console.WriteLine("SqlConnection Done.");

                foreach (var model in models)
                {
                    //VIQInfo
                    var parameters = new SqlCeParameter[]
                      {
                        new SqlCeParameter ("@p_qid"    ,(object)(model.QId)),
                        new SqlCeParameter ("@p_title"  ,(object)(model.Title) ?? DBNull.Value),
                        new SqlCeParameter ("@p_author" ,(object)(model.Author) ?? DBNull.Value),
                        new SqlCeParameter ("@p_final"  ,(object)(model.Finalized) ?? DBNull.Value),
                        new SqlCeParameter ("@p_regdate",(object)(model.RegistrationDate) ?? DBNull.Value),
                        new SqlCeParameter ("@p_comment",(object)(model.Comments) ?? DBNull.Value),
                        new SqlCeParameter ("@p_viqgui" ,(object)(model.VIQGUI) ?? DBNull.Value),
                        new SqlCeParameter ("@p_numofq" ,(object)(model.NumOfQuestions) ?? DBNull.Value),
                      };
                    await _sqlConnect.CommandAsync(AppResource.SQL_Insert_VIQInfo, parameters);

                    int qid = model.QId;

                    //Questionnaire              
                    string query = $"select InternalDisplayCode,null 'Categoryid',c.DisplayCode,c.ParentCategory 'ParentCategoryId',c.ObjectComments,c.Defaultcode,c.ObjectId,c.ObjectType,c.ObjectDescription,c.CHildren,c.GlobalDisplayIndex,c.DisplayLevel,c.ParentId from(  select a.*, b.comment 'ObjectComments', b.questioncode 'DefaultCode', rtrim(b.question) 'ObjectDescription' from viq a , QuestionPoolNew b where a.objectid = b.questionid and a.objecttype = 0 and a.qid = {qid} union all select a.*, Comments 'ObjectComments', b.categorycode 'DefaultVode', rtrim(b.CategoryDescription) 'ObjectDescription' from viq a , Category b where a.objectid = b.categorynewid and a.objecttype = 2 and a.qid = {qid} ) c order by globaldisplayindex";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                        Console.WriteLine("Done.");


                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var parameters1 = new SqlCeParameter[]
                                {
                                    new SqlCeParameter ("@p_qid"    ,(object)(qid)),
                                    new SqlCeParameter ("@p_objid"  ,(object)reader["ObjectId"] ?? DBNull.Value),
                                    new SqlCeParameter ("@p_objtype" ,(object)reader["ObjectType"] ?? DBNull.Value),
                                    new SqlCeParameter ("@p_catid"  ,(object)reader["categoryId"] ?? DBNull.Value),
                                    new SqlCeParameter ("@p_parcatid" ,(object)reader["ParentCategoryId"] ?? DBNull.Value),
                                    new SqlCeParameter ("@p_glbidx" ,(object)reader["GlobalDisplayIndex"] ?? DBNull.Value),
                                    new SqlCeParameter ("@p_dl" ,(object)reader["DisplayLevel"] ?? DBNull.Value),
                                    new SqlCeParameter ("@p_children" ,(object)reader["Children"] ?? DBNull.Value),
                                    new SqlCeParameter ("@p_objdescr" ,(object)reader["ObjectDescription"] ?? DBNull.Value),
                                    new SqlCeParameter ("@p_comm" ,(object)reader["ObjectComments"] ?? DBNull.Value),
                                    new SqlCeParameter ("@p_displcode" ,(object)reader["DisplayCode"] ?? DBNull.Value),
                                    new SqlCeParameter ("@p_defcode" ,(object)reader["DefaultCode"] ?? DBNull.Value),
                                    new SqlCeParameter ("@p_parentid" ,(object)reader["ParentId"] ?? DBNull.Value),
                                    new SqlCeParameter ("@p_intdisplaycode" ,(object)reader["InternalDisplayCode"] ?? DBNull.Value)
                                };

                                await _sqlConnect.CommandAsync(AppResource.SQL_Insert_Questionnaire, parameters1);
                            }
                        }
                    }
                }
            }
        }

        private FileInfo InstanceExportFileName(string userName, string vettingType)
        {
            DateTime fromdate = DateTime.Now.Date;
            DateTime todate = fromdate.AddMonths(1);
            string fileName = $"{userName}_{vettingType}_{fromdate:dd-MM-yyyy}_to_{todate:dd-MM-yyyy}.sdf";
            var fileInfo = new FileInfo(Path.Combine(Path.GetTempPath(), fileName));
            return fileInfo;
        }

        private async Task SaveSDFUploadFileToLocalAsync(UploadFileModel model, string fileName)
        {
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            using (var fileStr = File.Create(fileName))
            {
                await fileStr.WriteAsync(model.FileContent, 0, model.FileContent.Length);
            }
        }

        #endregion Helpers
    }
}