using Attendance.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Attendance.Pages.QuestionPool
{
    public class DetailsModel : PageModel
    {
        private readonly AttendanceContext _context;

        public DetailsModel(AttendanceContext context)
        {
            _context = context;
        }

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
    }
}
