using Attendance.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Attendance.Pages.VettingInfo
{
    public class EditModel : PageModel
    {
        private readonly AttendanceContext _context;

        public EditModel(AttendanceContext context)
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

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(VettingInfo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VettingInfoExists(VettingInfo.VetId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool VettingInfoExists(int id)
        {
            return _context.VettingInfo.Any(e => e.VetId == id);
        }
    }
}
