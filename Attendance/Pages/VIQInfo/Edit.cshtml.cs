using Attendance.Data;
using Attendance.Infrastructure.Data;
using Attendance.Models;
using Attendance.Providers;
using Attendance.Services.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Attendance.Pages.VIQInfo
{
    public class EditModel : PageModel
    {
        private readonly AttendanceContext _context;
        private readonly IAttendanceService _attendanceService;
        public PageAlertType AlertType = PageAlertType.Info;

        public EditModel(AttendanceContext context, IAttendanceService attendanceService)
        {
            _context = context;
            _attendanceService = attendanceService;
        }

        [BindProperty]
        public VIQInfoModel VIQInfoModel { get; set; }
        public int PageIndex { get; set; }
        public int QId { get; set; }

        public List<VIQDetailItem> VIQDetail { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id, int pageIndex, int? deleteID, int? moveDownID, int? moveUpID)
        {
            TempData["message"] = null;
            if (id == null)
            {
                return NotFound();
            }

            if (deleteID != null && deleteID != 0)
            {
                VIQ viq = await _context.VIQ.FirstOrDefaultAsync(m => m.Id.Equals(deleteID));
                if (viq != null)
                {
                    var oneRecordAbove = await _context.VIQ.FirstOrDefaultAsync(m => m.QId == id && m.GlobalDisplayIndex == viq.ShowAfterId);
                    var oneRecordBelow = await _context.VIQ.FirstOrDefaultAsync(m => m.QId == id && m.ShowAfterId == viq.GlobalDisplayIndex);
                    // It is only 1 row 
                    if (oneRecordAbove == null && oneRecordBelow == null)
                    {
                        // Do nothing just delete
                    }
                    // If it is first row with showAfterId = 0
                    else if (oneRecordAbove == null && oneRecordBelow != null)
                    {
                        oneRecordBelow.ShowAfterId = 0;
                        _context.VIQ.Update(oneRecordBelow);
                    }
                    // If it is last record
                    else if (oneRecordBelow == null && oneRecordAbove != null)
                    {
                        //Do nothing just delete
                    }
                    else if (oneRecordBelow != null && oneRecordAbove != null)
                    {
                        oneRecordBelow.ShowAfterId = oneRecordAbove.GlobalDisplayIndex;
                        _context.VIQ.Update(oneRecordBelow);
                    }
                    _context.VIQ.Remove(viq);
                    await _context.SaveChangesAsync();
                }
                OnLog(AppMessages.Viq_Edit_QuestionDeleted, PageAlertType.Success);
            }

            if (moveDownID != null)
            {
                VIQ firstViq = await _context.VIQ.FirstOrDefaultAsync(m => m.QId == id && m.ShowAfterId == moveDownID);
                if (firstViq != null)
                {
                    //var firstViqShowAfter = firstViq.ShowAfterId;
                    VIQ secondViq = await _context.VIQ.Where(m => m.QId == id && m.ShowAfterId == firstViq.GlobalDisplayIndex).FirstOrDefaultAsync();
                    if (secondViq != null)
                    {
                        secondViq.ShowAfterId = firstViq.ShowAfterId;
                        firstViq.ShowAfterId = secondViq.GlobalDisplayIndex;
                        _context.VIQ.Update(firstViq);
                        _context.VIQ.Update(secondViq);
                        await _context.SaveChangesAsync();

                        // if there is a third record we need update that one, too
                        VIQ thirdViq = await _context.VIQ.Where(m => m.QId == id && m.ShowAfterId == firstViq.ShowAfterId && m.Id != firstViq.Id).FirstOrDefaultAsync();
                        if (thirdViq != null)
                        {
                            thirdViq.ShowAfterId = firstViq.GlobalDisplayIndex;
                            _context.VIQ.Update(thirdViq);
                            await _context.SaveChangesAsync();
                        }
                    }

                }

                OnLog(AppMessages.Viq_Edit_QuestionMovedDown, PageAlertType.Success);
            }

            if (moveUpID != null && moveUpID != 0)
            {
                VIQ firstViq = await _context.VIQ.FirstOrDefaultAsync(m => m.QId == id && m.ShowAfterId == moveUpID);
                if (firstViq != null)
                {
                    VIQ secondViq = await _context.VIQ.Where(m => m.QId == id && m.GlobalDisplayIndex == firstViq.ShowAfterId).FirstOrDefaultAsync();
                    if (secondViq != null)
                    {
                        firstViq.ShowAfterId = secondViq.ShowAfterId;
                        secondViq.ShowAfterId = firstViq.GlobalDisplayIndex;
                        _context.VIQ.Update(firstViq);
                        _context.VIQ.Update(secondViq);
                        await _context.SaveChangesAsync();

                        // if there is a third record we need update that one, too
                        VIQ thirdViq = await _context.VIQ.Where(m => m.QId == id && m.ShowAfterId == secondViq.ShowAfterId && m.Id != secondViq.Id).FirstOrDefaultAsync();
                        if (thirdViq != null)
                        {
                            thirdViq.ShowAfterId = secondViq.GlobalDisplayIndex;
                            _context.VIQ.Update(thirdViq);
                            await _context.SaveChangesAsync();
                        }
                    }
                }
                OnLog(AppMessages.Viq__Edit_QuestionMovedUp, PageAlertType.Success);
            }

            VIQInfoModel = await _context.VIQInfo.FirstOrDefaultAsync(m => m.QId == id);
            if (VIQInfoModel == null)
            {
                return NotFound();
            }

            QId = id ?? 0;
            PageIndex = pageIndex;

            VIQDetail = _attendanceService.GetVIQDetail(id ?? 0);

            return Page();
        }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(int pageIndex)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(VIQInfoModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VIQInfoModelExists(VIQInfoModel.QId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index", "", new { pageIndex = pageIndex });
        }

        private bool VIQInfoModelExists(int id)
        {
            return _context.VIQInfo.Any(e => e.QId == id);
        }

        private void OnLog(string message, PageAlertType alertType)
        {
            AlertType = alertType;
            TempData["message"] = message;
            TempData["result"] = alertType.GetDescription();
        }
    }
}
