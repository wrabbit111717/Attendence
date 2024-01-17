using Attendance.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace Attendance.Pages.VettingInfo
{
    public class CreateModel : PageModel
    {
        private readonly AttendanceContext _context;

        public CreateModel(AttendanceContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Attendance.Models.VettingInfo VettingInfo { get; set; }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.VettingInfo.Add(VettingInfo);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
