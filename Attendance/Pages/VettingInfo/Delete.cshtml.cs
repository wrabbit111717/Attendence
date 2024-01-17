using Attendance.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Attendance.Pages.VettingInfo
{
    public class DeleteModel : PageModel
    {
        private readonly AttendanceContext _context;

        public DeleteModel(AttendanceContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Attendance.Models.VettingInfo VettingInfo { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            VettingInfo = await _context.VettingInfo.FirstOrDefaultAsync(m => m.VetId == id);

            if (VettingInfo == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            VettingInfo = await _context.VettingInfo.FindAsync(id);

            if (VettingInfo != null)
            {
                _context.VettingInfo.Remove(VettingInfo);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
