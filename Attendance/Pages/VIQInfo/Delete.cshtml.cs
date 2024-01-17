using Attendance.Infrastructure.Data;
using Attendance.Models;
using Attendance.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Attendance.Pages.VIQInfo
{
    public class DeleteModel : PageModel
    {
        private readonly AttendanceContext _context;
        private readonly IUserRepository _userRepository;

        public DeleteModel(AttendanceContext context, IUserRepository userRepository)
        {
            _context = context;
            _userRepository = userRepository;
        }

        [BindProperty]
        public VIQInfoModel VIQInfoModel { get; set; }
        public int PageIndex { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id, int pageIndex)
        {
            if (id == null)
            {
                return NotFound();
            }

            VIQInfoModel = await _context.VIQInfo.FirstOrDefaultAsync(m => m.QId == id);

            if (VIQInfoModel == null)
            {
                return NotFound();
            }
            PageIndex = pageIndex;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            VIQInfoModel = await _context.VIQInfo.Include(x => x.UserQuestionnaires).FirstOrDefaultAsync(m => m.QId == id);

            //I put a check => if the logged in user is Admin than he can delete the original viqInfo record
            //but the user is not in Admin rights, then user can just remove record from UserQuestionnarie table, not from the original viqInfo
            //Everyone is admin (meanwhile)
            if (VIQInfoModel != null)
            {
                // Better with include 
                VIQInfoModel.UserQuestionnaires.Clear();
                await _context.SaveChangesAsync();
                _context.VIQInfo.Remove(VIQInfoModel);
            }
            //delete records which are related with that QId from VIQ Table, too
            var viqList = await _context.VIQ.Where(x => x.QId == id).ToListAsync();
            if (viqList != null && viqList.Count > 0)
            {
                _context.VIQ.RemoveRange(viqList);
            }
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}
