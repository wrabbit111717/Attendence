using Attendance.Infrastructure.Data;
using Attendance.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace Attendance.Pages.VIQInfo
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
        public VIQInfoModel VIQInfoModel { get; set; }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _context.VIQInfo.AddAsync(VIQInfoModel);
            await _context.SaveChangesAsync();

            var questionnaire = new UserQuestionnaire()
            {
                VIQInfo = VIQInfoModel,
                QId = VIQInfoModel.QId,
                UserId = 2 // TODO: Remove user field at all
            };

            VIQInfoModel.UserQuestionnaires.Add(questionnaire);
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}
