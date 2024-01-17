using Attendance.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Attendance.Pages.QuestionPool
{
    public class EditModel : PageModel
    {
        private readonly AttendanceContext _context;

        public EditModel(AttendanceContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public Attendance.Models.QuestionPool QuestionPool { get; set; }

        public int CategoryID { get; set; }
        public string CategoryCode { get; set; }
        public Guid CategoryNewID { get; set; }
        public int PageIndex { get; set; }

        public int QuestionTypeID { get; set; }

        //Category PageIndex        
        public string CurrentFilterCategory { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id, Guid categoryId, int qTypeID, int pageIndex, string currentFilterCategory)
        {
            Attendance.Models.Category Category = await _context.Category.FirstOrDefaultAsync(m => m.CategoryNewID == categoryId);

            CategoryID = Category.CategoryID;
            CategoryCode = Category.CategoryCode;
            CategoryNewID = categoryId;
            PageIndex = pageIndex;
            QuestionTypeID = qTypeID;
            CurrentFilterCategory = currentFilterCategory;

            if (id == null)
            {
                return NotFound();
            }

            QuestionPool = await _context.QuestionPoolNew.Where(m => m.questionid == id).Include(c => c.Parent).Include(x => x.Children).FirstOrDefaultAsync();

            if (QuestionPool == null)
            {
                return NotFound();
            }
            return Page();
        }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(int PageIndex, string currentFilterCategory, Guid selectedCategoryNewID, int selectedTypeID)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            _context.Attach(QuestionPool).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuestionPoolExists(QuestionPool.questionid))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("/Questions/Index", "", new { pageIndex = PageIndex, qPageIndex = 1, selectedCategoryNewID = selectedCategoryNewID.ToString(), selectedTypeID = selectedTypeID, currentFilterCategory = currentFilterCategory, currentFilterQuestionPool = "" });
        }

        private bool QuestionPoolExists(Guid id)
        {
            return _context.QuestionPoolNew.Any(e => e.questionid == id);
        }
    }
}
