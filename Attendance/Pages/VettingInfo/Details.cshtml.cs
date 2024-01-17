using Attendance.Data;
using Attendance.Infrastructure.Data;
using Attendance.Models;
using Attendance.Providers;
using Attendance.Services.Services;
using Attendance.Services.ViewModels;

using ClosedXML.Excel;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Attendance.Pages.VettingInfo
{
    public class DetailsModel : PageModel
    {

        public PageAlertType AlertType = PageAlertType.Info;

        private readonly AttendanceContext _context;
        private readonly IAttendanceService _attendanceService;
        private readonly IVettingsService _vettingService;
        public SelectList CarriedOutStatusList { get; set; }
        public SelectList QidList { get; set; }
        public SelectList StatusList { get; set; }
        public DetailsModel(AttendanceContext context,
            IAttendanceService attendanceService,
            IVettingsService vettingService)
        {
            _context = context;
            _attendanceService = attendanceService;
            _vettingService = vettingService;
        }
        public PaginationViewModel<AttendanceDetailItem> attendanceDetail { get; set; }

        public Models.VettingInfo VettingInfo { get; set; }

        public VettingInfoDetail VettingInfoDetail { get; set; }

        public Vetting Vetting { get; set; }

        [FromQuery(Name = "pageNumber")]
        public int? PageNumber { get; set; } = 1;

        [FromQuery(Name = "positive")]
        public bool Positive { get; set; } = false;
        [FromQuery(Name = "negative")]
        public bool Negative { get; set; } = false;
        [FromQuery(Name = "remark")]
        public bool Remark { get; set; } = false;
        [FromQuery(Name = "qid")]
        public int? Qid { get; set; }
        [FromQuery(Name = "questionComments")]
        public bool QuestionComments { get; set; } = false;
        [FromQuery(Name = "answerComments")]
        public bool AnswerComments { get; set; } = false;

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (Id == 0)
            {
                return NotFound();
            }

            VettingInfo = await _context.VettingInfo
                .Include(x => x.InspectionType)
                .FirstOrDefaultAsync(m => m.VetId == Id);

            if (VettingInfo == null)
            {
                return NotFound();
            }

            VettingInfoDetail = new VettingInfoDetail();

            VettingInfoDetail.VesselName = VettingInfo.VesselName;
            VettingInfoDetail.InspectorName = VettingInfo.InspectorName;
            VettingInfoDetail.CompanyRepresentativeName = VettingInfo.CompanyRepresentativeName;
            VettingInfoDetail.VettingCode = VettingInfo.VettingCode;
            VettingInfoDetail.VetShortDate = VettingInfo.VetDate.ToShortDateString();
            VettingInfoDetail.InspectionTypeName = VettingInfo.InspectionType?.InspectionType;
            VettingInfoDetail.MajorId = VettingInfo.MajorId;
            VettingInfoDetail.Major = VettingInfo.VettingCode.Substring(VettingInfo.VettingCode.LastIndexOf("-")).Trim().Trim('-');
            VettingInfoDetail.Port = VettingInfo.Port;
            VettingInfoDetail.Country = VettingInfo.Country;
            VettingInfoDetail.RegistrationShortDate = VettingInfo.RegistrationDate.HasValue ? VettingInfo.RegistrationDate.Value.ToShortDateString() : "";
            VettingInfoDetail.Positive = VettingInfo.Positive;
            VettingInfoDetail.Negative = VettingInfo.Negative;
            VettingInfoDetail.Answered = VettingInfo.Answered;
            VettingInfoDetail.Comments = VettingInfo.Comments;
            VettingInfoDetail.VetId = VettingInfo.VetId;
            VettingInfoDetail.QId = VettingInfo.QId;
            VettingInfoDetail.CarriedOutStatus = VettingInfo.CarriedOutStatus;
            VettingInfoDetail.Status = VettingInfo.Status;
            attendanceDetail = _attendanceService.GetAttendanceDetail(VettingInfo.VetId, PageNumber, Positive, Negative, Remark, Qid);

            var qidList = new List<SelectListItem>();

            var qidDict = _attendanceService.GetQids(VettingInfo.VetId);

            foreach (KeyValuePair<int, string> e in qidDict)
            {
                qidList.Add(new SelectListItem() { Text = e.Value ?? e.Key.ToString(), Value = e.Key.ToString() });
            }
            ;

            QidList = new SelectList(items: qidList, nameof(SelectListItem.Value), nameof(SelectListItem.Text));

            var selectList = new List<SelectListItem>();
            selectList.Add(new SelectListItem() { Text = "Loading", Value = "0" });
            selectList.Add(new SelectListItem() { Text = "Discharging", Value = "1" });
            selectList.Add(new SelectListItem() { Text = "STS Loading", Value = "2" });
            selectList.Add(new SelectListItem() { Text = "STS Discharging", Value = "3" });
            selectList.Add(new SelectListItem() { Text = "Bunkering", Value = "4" });
            selectList.Add(new SelectListItem() { Text = "Idle", Value = "5" });

            CarriedOutStatusList = new SelectList(items: selectList, nameof(SelectListItem.Value), nameof(SelectListItem.Text));

            var statusSelectList = new List<SelectListItem>();
            statusSelectList.Add(new SelectListItem() { Text = "Approved", Value = "0" });
            statusSelectList.Add(new SelectListItem() { Text = "On Hold", Value = "1" });
            statusSelectList.Add(new SelectListItem() { Text = "Not Approved", Value = "2" });
            StatusList = new SelectList(items: statusSelectList, nameof(SelectListItem.Value), nameof(SelectListItem.Text));

            var crewEvalutionInfo = await _context.FormDetail.FirstOrDefaultAsync(m => m.vetid == Id);
            VettingInfoDetail.IsCrawEvaluation = crewEvalutionInfo != null;
            VettingInfoDetail.listObservationsSire2 = new List<ObservationsSire2ViewModel>();
            VettingInfoDetail.listObservationsSire2Assignees = new List<ObservationsSire2Assignees>();
            VettingInfoDetail.listObservationsSire2Attachments = new List<ObservationsSire2Attachments>();
            if (attendanceDetail.Data.Count > 0)
            {
                var strQids = string.Join(",", attendanceDetail.Data.Select(p => p.qid).ToList());
                var strObjectIds = string.Join(",", attendanceDetail.Data.Select(p => p.ObjectId).ToList());
                var observationList = _attendanceService.GetObservation(Id, strQids, strObjectIds);
                
                VettingInfoDetail.listObservationsSire2.AddRange(observationList.listObservationsSire2);
                VettingInfoDetail.listObservationsSire2Assignees.AddRange(observationList.listObservationsSire2Assignees);
                VettingInfoDetail.listObservationsSire2Attachments.AddRange(observationList.listObservationsSire2Attachments);
            }
            return Page();
        }

        public async Task<IActionResult> OnGetExportToPdfAsync()
        {
            if (Id == 0)
            {
                return NotFound();
            }

            var fileInfo = await _attendanceService.GetPDFDocument(Id, null, Positive, Negative, Remark, Qid);

            var bytes = new byte[0];
            var fileName = "";

            using (FileStream fs = fileInfo.OpenRead())
            {
                using (var memoryStream = new MemoryStream())
                {
                    fs.CopyTo(memoryStream);
                    bytes = memoryStream.ToArray();
                    fileName = fileInfo.Name;

                }
            }

            fileInfo.Delete();
            return File(bytes, "application/pdf", fileName);

        }

        public async Task<IActionResult> OnGetExportToExcelAsync()
        {
            DataTable dt = new DataTable("AttendanceDetail");
            dt.Columns.AddRange(new DataColumn[4] {
                new DataColumn("VIQ No"),
                new DataColumn("Question"),
                new DataColumn("Answer"),
                new DataColumn("Remark")
            });
            var attendanceDetail = _attendanceService.GetAttendanceDetail(Id, null, Positive, Negative, Remark, Qid);
            foreach (var detail in attendanceDetail.Data)
            {
                dt.Rows.Add(detail.DisplayCode, detail.CategoryQuestion, detail.Answer == "1" ? "YES" : detail.Answer == "2" ? "NO" : detail.Answer == "3" ? "N/S" : detail.Answer == "4" ? "N/A" : "", detail.comments);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add(dt);
                ws.Style.Alignment.WrapText = true;
                ws.Style.Font.FontName = "Calibri";
                ws.Style.Font.FontSize = 11;
                var col1 = ws.Column("A");
                col1.Width = 14.29;
                var col2 = ws.Column("B");
                col2.Width = 75.57;
                var col3 = ws.Column("C");
                col3.Width = 10.43;
                var col4 = ws.Column("D");
                col4.Width = 64.43;
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return await Task.Run(() => File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "AttendanceDetail.xlsx"));
                }
            }
        }

        public async Task<IActionResult> OnPostEditAsync(int? vetId, string vesselName
            , string inspectorName
            , string compRep
            , string insCode,
            VettingInfoDetail vettingInfoDetail)
        {
            var vettingInfo = await _context.VettingInfo.Where(x => x.VetId == vetId).FirstOrDefaultAsync();
            vettingInfo.Comments = vettingInfoDetail.Comments;
            vettingInfo.CarriedOutStatus = vettingInfoDetail.CarriedOutStatus;
            vettingInfo.Status = vettingInfoDetail.Status;
            await _context.SaveChangesAsync();
            OnLog(AppMessages.Vetting_UpdateSucceeded, PageAlertType.Success);
            return RedirectToPage("/VettingInfo/Details", "", new { id = vetId, vesselname = vesselName, inspectorName = inspectorName, compRep = compRep, insCode = insCode });
        }

        private void OnLog(string message, PageAlertType alertType)
        {
            AlertType = alertType;
            TempData["message"] = message;
            TempData["result"] = alertType.GetDescription();
        }
    }
}
