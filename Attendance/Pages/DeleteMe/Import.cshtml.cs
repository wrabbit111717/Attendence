using Attendance.Data;
using Attendance.Infrastructure.Data;
using Attendance.Models;
using Attendance.Providers;
using Attendance.Services;
using Attendance.Services.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static Attendance.Data.AppConstants;
namespace Attendance.Pages.Briefcase
{
    public class ImportModel : PageModel
    {

        #region services
        private readonly ICompactVettingsService _vettingsService;
        private readonly IConfiguration _configuration;
        private readonly IBriefcaseService _briefcaseService;
        private readonly IBriefcaseImportRepository _importRepository;
        private readonly AttendanceContext _context;
        #endregion services

        #region Binding Properties

        public PageAlertType AlertType = PageAlertType.Info;

        [BindProperty]
        public ImportFileViewModel FileContent { get; set; }

        [BindProperty]
        public int? SelectedVettingInfo { get; set; }

        public int SelectedQuestionnaire { get; set; }
        public IList<Attendance.Models.VettingInfo> RegisteredAttendances { get; set; } = new List<Attendance.Models.VettingInfo>();
        public IList<VIQInfoModel> ImportedItems { get; set; } = new List<VIQInfoModel>();
        public IList<VIQInfoModel> SdfDbItems { get; set; } = new List<VIQInfoModel>();

        #endregion Binding Properties

        public ImportModel(
            IBriefcaseImportRepository importRepository
            , IBriefcaseService briefcaseService
            , IConfiguration configuration
            , ICompactVettingsService vettingsService,
              AttendanceContext context
            )
        {
            _vettingsService = vettingsService;
            _configuration = configuration;
            _briefcaseService = briefcaseService;
            _importRepository = importRepository;
            _context = context;
        }

        public async Task OnGetAsync(string action, int? qId, string code, int alerttype = 0)
        {
            OnRenderDataPage();
            if (!string.IsNullOrEmpty(action))
            {
                if (action == "importSelected" && qId.HasValue)
                {
                    SelectedQuestionnaire = qId.Value;
                    var vettingInfos = await _vettingsService.VettingsByQuestionAsync(qId.Value, _importRepository.DataSource, _configuration[AppConfigKeys.SDF_PASSWORD]);
                    _importRepository.SetAttendanceItems(vettingInfos);
                    RegisteredAttendances = _importRepository.GetAttendanceItems().ToList();
                }
            }
        }

        public async Task<IActionResult> OnPostAsync(string action)
        {
            if (action == "extract")
            {
                await ExtractDataFromDBFileAsync();
            }
            if (action == "import")
            {
                await TransferToDBAsync();
            }
            return Page();
        }

        #region Helpers

        private async Task<IActionResult> ExtractDataFromDBFileAsync()
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelState.Values.Where(s => s.Errors.Any()).SelectMany(s => s.Errors.Select(e => e.ErrorMessage)).FirstOrDefault();
                OnRenderDataPage();
                OnLog(errorMessage, PageAlertType.Danger);
                return Page();
            }

            var fileModel = await FileContent.File.ToFileModel();
            _importRepository.DataSource = fileModel.FileName;
            var importTmpItems = await _briefcaseService.ExtractFromDBAsync(fileModel, _configuration[AppConfigKeys.SDF_PASSWORD]);

            // Recent changes:
            // Authorized user has permission check 
            //var loggedInUser = int.Parse(User.Claims.FirstOrDefault(c => string.Equals(c.Type, ClaimTypes.NameIdentifier, StringComparison.InvariantCultureIgnoreCase))?.Value);
            //foreach (var item in importTmpItems)
            //{
            //    var relatedRecord = await _context.VIQInfo.Include(x => x.UserQuestionnaires).Where(x => x.UserQuestionnaires.Any(x => x.UserId == loggedInUser && x.QId == item.QId)).FirstOrDefaultAsync();
            //    if (relatedRecord == null)
            //    {
            //        OnRenderDataPage();
            //        OnLog(AppMessages.Import_QuestionnaireNotAuthorized, PageAlertType.Danger);
            //        return Page();
            //    }
            //}

            _importRepository.ClearSdfDBItems();
            _importRepository.AddSdfDBItems(importTmpItems.ToArray());
            OnRenderDataPage();
            OnLog(AppMessages.Import_UploadFileSucceeded, PageAlertType.Info);

            return Page();
        }

        private async Task<IActionResult> TransferToDBAsync()
        {
            OnRenderDataPage();
            if (!SelectedVettingInfo.HasValue)
            {
                OnLog(AppMessages.Import_NoAttendanceSelected, PageAlertType.Danger);
                return Page();
            }

            var filePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), _importRepository.DataSource);

            var model = await _vettingsService.VettingInfoByIdAsync(SelectedVettingInfo.Value
                , filePath
                , _configuration[AppConfigKeys.SDF_PASSWORD]);

            if (model != null)
            {
                try
                {
                    await _briefcaseService.ImportAsync(filePath, _configuration[AppConfigKeys.SDF_PASSWORD], model);
                    OnLog(AppMessages.TransferToDatabaseSucceeded, PageAlertType.Info);
                }
                catch (Exception ex) { OnLog(ex.Message, PageAlertType.Danger); }
            }

            return Page();
        }

        private void OnRenderDataPage()
        {
            if (!SdfDbItems.Any())
                SdfDbItems = _importRepository.SdfDBItems().ToList();

            RegisteredAttendances = _importRepository.GetAttendanceItems().ToList();
        }

        private void OnLog(string message, PageAlertType alertType)
        {
            AlertType = alertType;
            TempData["message"] = message;
            TempData["result"] = alertType.GetDescription();
        }
        #endregion Helpers
    }

    public class ImportFileViewModel
    {
        [Required]
        [Display(Name = "File")]
        public IFormFile File { get; set; }
    }
}