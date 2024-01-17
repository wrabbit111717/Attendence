using Attendance.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Attendance.Pages.QuestionPool
{
    public class DeleteModel : PageModel
    {
        private readonly AttendanceContext _context;

        public DeleteModel(AttendanceContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Attendance.Models.QuestionPool QuestionPool { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            if (id == null)
            {
                return NotFound();
            }

            QuestionPool = await _context.QuestionPoolNew.FirstOrDefaultAsync(m => m.questionid == id);

            if (QuestionPool == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //QuestionPool = await _context.QuestionPoolNew.FindAsync(id);
            QuestionPool = await _context.QuestionPoolNew.FirstOrDefaultAsync(m => m.questionid.ToString().Equals(id));


            if (QuestionPool != null)
            {
                _context.QuestionPoolNew.Remove(QuestionPool);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
