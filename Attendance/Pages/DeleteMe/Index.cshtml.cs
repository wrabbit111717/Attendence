using Attendance.Data;
using Attendance.Infrastructure.Data;
using Attendance.Models;
using Attendance.Providers;
using Attendance.Services;
using Attendance.Services.Services;
using Attendance.Services.ViewModels.Briedcases;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static Attendance.Data.AppConstants;

namespace Attendance.Pages.Briefcase
{

    public class ResultFilter : ActionFilterAttribute
    {
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            if (filterContext.Result is FileResult)
            {
                var result = (FileStreamResult)filterContext.Result;
                var stream = (FileStream)result.FileStream;
                if (stream != null && File.Exists(stream.Name))
                {
                    File.Delete(stream.Name);
                }
            }
        }
    }

    [ResultFilter()]
    public class IndexModel : PageModel
    {
        #region dependency services

        private readonly IBriefcaseService _briefcaseService;
        private readonly IConfiguration _configuration;
        private readonly AttendanceContext _context;
        private readonly IBriefcaseRepository _briefcaseRepository;

        #endregion dependency services

        #region Binding properties
        public PageAlertType AlertType;

        [BindProperty]
        public ExportViewModel ExportModel { get; set; }
        public IList<VIQInfoModel> VIQInfoModel { get; set; } = new List<VIQInfoModel>();
        public SelectList InspectorTypes { get; set; }
        public IEnumerable<VIQInfoModel> RegisteredQuestionnaires { get; set; } = new List<VIQInfoModel>();
        #endregion Binding properties

        public IndexModel(AttendanceContext context
            , IBriefcaseRepository briefcaseRepository
            , IConfiguration configuration
            , IBriefcaseService briefcaseService
            )
        {
            _briefcaseService = briefcaseService;
            _configuration = configuration;
            _context = context;
            _briefcaseRepository = briefcaseRepository;
            AlertType = PageAlertType.Info;
        }

        public async Task OnGetAsync(string action, int? id, int alerttype = 0)
        {
            await OnRenderDataPageAsync();
            if ("transfer" == action)
            {
                //add questionnaire
                VIQInfoModel viqinfo = VIQInfoModel.FirstOrDefault(item => item.QId == id);
                bool result = _briefcaseRepository.AddQuestionnaire(viqinfo);
                if (result == false)
                {
                    OnLog(AppMessages.Export_QuestionnaireAlready, PageAlertType.Info);
                }
                else
                {
                    OnLog(AppMessages.Export_QuestionnaireTranferred, PageAlertType.Success);
                }
            }
            else if (action == "remove")
            {
                _briefcaseRepository.RemoveQuestionnaire(id ?? -1);
                OnLog(AppMessages.Export_QuestionnaireRemoved, PageAlertType.Info);
            }
        }


        public async Task<IActionResult> OnPostAsync()
        {
            // Model validation
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelState.Values.Where(s => s.Errors.Any()).SelectMany(s => s.Errors.Select(e => e.ErrorMessage)).FirstOrDefault();
                await OnRenderDataPageAsync();
                OnLog(errorMessage, PageAlertType.Danger);
                return Page();
            }

            // Check having selected questionnaire
            RegisteredQuestionnaires = _briefcaseRepository.GetRegisteredQuestionnaires();
            if (!RegisteredQuestionnaires.Any())
            {
                await OnRenderDataPageAsync();
                OnLog(AppMessages.Export_NoQuestionnaireSelected, PageAlertType.Warning);
                return Page();
            }

            // start export to SDF file
            var vIQIds = RegisteredQuestionnaires.Select(i => i.QId).ToList();
            var vIQInfoItems = _context.VIQInfo.Where(i => vIQIds.Contains(i.QId)).ToArray();
            ExportModel.ExportPassword = _configuration[AppConfigKeys.SDF_PASSWORD];
            ExportModel.VIQInfoModels = vIQInfoItems;
            var file = await _briefcaseService.ExportAsync(ExportModel);
            return File(file.OpenRead(), "application/octet-stream", file.Name);
        }

        private void OnLog(string message, PageAlertType alertType)
        {
            AlertType = alertType;
            TempData["message"] = message;
            TempData["result"] = alertType.GetDescription();
        }

        private async Task OnRenderDataPageAsync()
        {

            //var loggedInUserId = User.Claims.FirstOrDefault(c => string.Equals(c.Type, ClaimTypes.NameIdentifier, StringComparison.InvariantCultureIgnoreCase))?.Value;
            // Inspects dropdownlist
            var items = _context.InspectionTypes.Select(t => new SelectListItem()
            {
                Text = t.InspectionType,
                Value = t.InspectionType
            }).ToList();
            InspectorTypes = new SelectList(items, nameof(SelectListItem.Value), nameof(SelectListItem.Text));

            //init viqinfomodel and registered questionnaires
            VIQInfoModel = await _context.VIQInfo.Where(_ => _.UserQuestionnaires.Any(_ => _.UserId == 2)).OrderByDescending(p => p.QId).ToListAsync(); // a.goulielmos 10/12/2020
            RegisteredQuestionnaires = _briefcaseRepository.GetRegisteredQuestionnaires();
        }
    }
}