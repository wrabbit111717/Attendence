using Attendance.Data;
using Attendance.Infrastructure.Data;
using Attendance.Models;
using Attendance.Providers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Attendance.Pages.VIQInfo
{
    public class IndexModel : PageModel
    {
        private readonly AttendanceContext _context;
        public PageAlertType AlertType = PageAlertType.Info;

        public IndexModel(AttendanceContext context)
        {
            _context = context;
        }
        public PaginatedList<VIQInfoModel> VIQInfoModel { get; set; }
        public List<VIQDetailItem> VIQDetail { get; set; }


        public async Task OnGetAsync(int? pageIndex)
        {
            // TODO:
            // 1. Remove all values from UserQuestionnaires table where UserId <> 2 ( Old legacy manager user )
            // 1. Remove UserId Field From UserQuestionnaires Table
            // 2. Remove UserId equality below from Any() closure (Get All Table Rows)
            var query = _context.VIQInfo
                .Where(x => x.UserQuestionnaires.Any(x => x.UserId == 2))
                .OrderByDescending(item => item.QId);
            int pageSize = Global.PageSize;
            VIQInfoModel = await PaginatedList<VIQInfoModel>.CreateAsync(
                query.AsNoTracking(), pageIndex ?? 1, pageSize);
        }

        // TODO: Remove Duplicate functionality (All Questionnaires are the same for all users)
        public async Task<IActionResult> OnGetDuplicateAsync(int? id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (id == null)
            {
                return NotFound();
            }

            var selectedViqInfo = await _context.VIQInfo.FirstOrDefaultAsync(m => m.QId == id);

            if (selectedViqInfo != null)
            {
                var duplicatedViq = new VIQInfoModel()
                {
                    Author = selectedViqInfo.Author,
                    Comments = selectedViqInfo.Comments,
                    EffectiveDate = selectedViqInfo.EffectiveDate,
                    Finalized = selectedViqInfo.Finalized,
                    NumOfQuestions = selectedViqInfo.NumOfQuestions,
                    RegistrationDate = selectedViqInfo.RegistrationDate,
                    SecurityColumn = selectedViqInfo.SecurityColumn,
                    Title = selectedViqInfo.Title + " Duplicated",
                    version = selectedViqInfo.version,
                    VIQGUI = Guid.NewGuid(),
                    Visible = selectedViqInfo.Visible
                };

                var duplicated = _context.VIQInfo.Add(duplicatedViq);
                await _context.SaveChangesAsync();


                var userQuestionnaireRelation = new UserQuestionnaire()
                {
                    VIQInfo = duplicated.Entity,
                    QId = duplicated.Entity.QId,
                    UserId = 2
                };

                duplicated.Entity.UserQuestionnaires.Add(userQuestionnaireRelation);
                await _context.SaveChangesAsync();

                var viqList = await _context.VIQ.Where(m => m.QId == id).ToListAsync();
                if (viqList != null && viqList.Count > 0)
                {
                    foreach (var item in viqList)
                    {
                        VIQ vIQ = new VIQ();
                        vIQ.QId = duplicated.Entity.QId;
                        vIQ.ObjectId = item.ObjectId;
                        vIQ.Children = item.Children;
                        vIQ.ObjectType = item.ObjectType;
                        vIQ.CategoryId = item.CategoryId;
                        vIQ.CommentId = item.CommentId;
                        vIQ.DisplayCode = item.DisplayCode;
                        vIQ.DisplayIndex = item.DisplayIndex;
                        vIQ.DisplayLevel = item.DisplayLevel;
                        vIQ.GlobalDisplayIndex = item.GlobalDisplayIndex;
                        vIQ.InternalDisplayCode = item.InternalDisplayCode;
                        vIQ.ParentCategory = item.ParentCategory;
                        vIQ.ParentId = item.ParentId;
                        vIQ.ParentType = item.ParentType;
                        vIQ.QuestionId = item.QuestionId;
                        _context.VIQ.Add(vIQ);

                    };
                    await _context.SaveChangesAsync();
                }
            }
            OnLog(AppMessages.Viq_DuplicateSucceeded, PageAlertType.Success);

            return RedirectToPage("./Index");
        }

        private void OnLog(string message, PageAlertType alertType)
        {
            AlertType = alertType;
            TempData["message"] = message;
            TempData["result"] = alertType.GetDescription();
        }
    }
}
