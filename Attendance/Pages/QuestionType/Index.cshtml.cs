using Attendance.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Attendance.Pages.QuestionType
{
    public class IndexModel : PageModel
    {
        private readonly AttendanceContext _context;

        public IndexModel(AttendanceContext context)
        {
            _context = context;
        }

        public IList<Attendance.Models.QuestionType> QuestionType { get; set; }

        public async Task OnGetAsync()
        {
            QuestionType = await _context.QuestionTypes.ToListAsync();
        }
    }
}
