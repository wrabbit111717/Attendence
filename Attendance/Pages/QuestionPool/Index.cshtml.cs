using Attendance.Infrastructure.Data;
using Attendance.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Attendance.Pages.QuestionPool
{
    public class IndexModel : PageModel
    {
        private readonly AttendanceContext _context;

        public IndexModel(AttendanceContext context)
        {
            _context = context;
        }

        public PaginatedList<Attendance.Models.QuestionPool> QuestionPool { get; set; }

        public async Task OnGetAsync(int? pageIndex)
        {
            IQueryable<Attendance.Models.QuestionPool> iq = from s in _context.QuestionPoolNew
                                                            select s;

            iq = iq.Where(item => !item.questioncode.Trim().Equals("") && !item.question.Trim().Equals(""));

            int pageSize = Attendance.Data.Global.PageSize;
            QuestionPool = await PaginatedList<Attendance.Models.QuestionPool>.CreateAsync(
                iq.AsNoTracking(), pageIndex ?? 1, pageSize);

        }
    }
}
