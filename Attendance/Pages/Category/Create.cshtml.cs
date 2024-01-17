using Attendance.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Threading.Tasks;

namespace Attendance.Pages.Category
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
            Guid.NewGuid();
            return Page();
        }

        [BindProperty]
        public Attendance.Models.Category Category { get; set; }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Category.Add(Category);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
