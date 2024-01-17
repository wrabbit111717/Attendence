using Attendance.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Attendance.Pages.QuestionType
{
    public class DetailsModel : PageModel
    {
        private readonly AttendanceContext _context;

        public DetailsModel(AttendanceContext context)
        {
            _context = context;
        }

        public Attendance.Models.QuestionType QuestionType { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            QuestionType = await _context.QuestionTypes.FirstOrDefaultAsync(m => m.TypeId == id);

            if (QuestionType == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
