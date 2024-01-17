using Attendance.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Attendance.Pages.QuestionPool
{
    public class CreateModel : PageModel
    {
        private readonly AttendanceContext _context;

        public CreateModel(AttendanceContext context)
        {
            _context = context;
        }

        //public IActionResult OnGet()
        //{
        //    return Page();
        //}

        [BindProperty]
        public Attendance.Models.QuestionPool QuestionPool { get; set; }

        public int CategoryID { get; set; }
        public string CategoryCode { get; set; }
        public int QuestionTypeID { get; set; }
        public Guid CategoryNewID { get; set; }

        //Category PageIndex
        public int PageIndex { get; set; }
        public string CurrentFilterCategory { get; set; }

        public async Task OnGetAsync(Guid categoryId, int qTypeID, int pageIndex, string currentFilterCategory)
        {
            Attendance.Models.Category Category = await _context.Category.FirstOrDefaultAsync(m => m.CategoryNewID == categoryId);

            CategoryID = Category.CategoryID;
            CategoryCode = Category.CategoryCode;
            QuestionTypeID = qTypeID;
            CategoryNewID = Category.CategoryNewID;

            PageIndex = pageIndex;
            CurrentFilterCategory = currentFilterCategory;

        }


        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(int PageIndex, string currentFilterCategory, Guid selectedCategoryNewID, int selectedTypeID)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.QuestionPoolNew.Add(QuestionPool);
            await _context.SaveChangesAsync();

            //return RedirectToPage("/Overview/Index");
            return RedirectToPage("/Questions/Index", "", new { pageIndex = PageIndex, qPageIndex = 1, selectedCategoryNewID = selectedCategoryNewID.ToString(), selectedTypeID = selectedTypeID, currentFilterCategory = currentFilterCategory, currentFilterQuestionPool = "" });
        }
    }
}
