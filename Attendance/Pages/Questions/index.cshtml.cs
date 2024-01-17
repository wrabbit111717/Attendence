using Attendance.Data;
using Attendance.Infrastructure.Data;
using Attendance.Models;
using Attendance.Providers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Attendance.Pages.Overview
{
    public class IndexModel : PageModel
    {
        public PageAlertType AlertType = PageAlertType.Info;

        private readonly AttendanceContext _context;

        public IndexModel(AttendanceContext context)
        {
            _context = context;
        }

        public PaginatedList<Attendance.Models.Category> Category { get; set; }
        public IList<Attendance.Models.QuestionType> QuestionType { get; set; }
        public PaginatedList<Attendance.Models.QuestionPool> QuestionPool { get; set; }
        public PaginatedList<Attendance.Models.QuestionPool> MngQuestionPool { get; set; }

        public string CurrentFilterCategory { get; set; }
        public string CurrentFilterQuestionPool { get; set; }
        public string CurrentFilterMngQuestionPool { get; set; }

        public Guid SelectedCategoryNewID { get; set; }
        public int SelectedTypeID { get; set; }

        public async Task OnGetAsync(int? pageIndex, int? qPageIndex, Guid categoryNewID, Guid selectedCategoryNewID,
            int typeID, int selectedTypeID,
            string currentFilterCategory, string currentFilterQuestionPool,
            string searchCategory, string searchQuestionPool)
        {
            if (searchCategory != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchCategory = currentFilterCategory;
            }

            CurrentFilterCategory = searchCategory;
            IQueryable<Attendance.Models.Category> categoryIQ = from s in _context.Category
                                                                select s;

            categoryIQ = categoryIQ.Where(cat => !cat.CategoryCode.Trim().Equals("") && !cat.CategoryDescription.Trim().Equals(""));
            if (!String.IsNullOrEmpty(searchCategory))
            {
                categoryIQ = categoryIQ.Where(s => s.CategoryCode.Equals(searchCategory)
                                       || s.CategoryDescription.ToLower().Contains(searchCategory.ToLower()));
            }

            int pageSize = Attendance.Data.Global.PageSize;
            Category = await PaginatedList<Attendance.Models.Category>.CreateAsync(
                categoryIQ.AsNoTracking(), pageIndex ?? 1, pageSize);


            QuestionType = await _context.QuestionTypes.ToListAsync();

            if (typeID != 0)
                SelectedTypeID = typeID;
            else
            {
                SelectedTypeID = selectedTypeID;

                if (SelectedTypeID == 0)
                    SelectedTypeID = QuestionType.ElementAt(0).TypeId;
            }

            ////////// SIRE Question Pool ///////////
            if (searchQuestionPool != null || (categoryNewID != Guid.Empty && selectedCategoryNewID != Guid.Empty))
            {
                qPageIndex = 1;
            }
            else
            {
                searchQuestionPool = currentFilterQuestionPool;
            }
            CurrentFilterQuestionPool = searchQuestionPool;
            IQueryable<Attendance.Models.QuestionPool> iq = from s in _context.QuestionPoolNew
                                                            select s;

            if (categoryNewID != Guid.Empty)
            {
                SelectedCategoryNewID = categoryNewID;
            }
            else
            {
                SelectedCategoryNewID = selectedCategoryNewID;

                if (SelectedCategoryNewID == Guid.Empty && Category.Count > 0)
                {
                    SelectedCategoryNewID = Category.First().CategoryNewID;
                }
            }

            iq = iq.Where(item => !item.questioncode.Trim().Equals("") && !item.question.Trim().Equals("") && item.CategoryNewID.Equals(SelectedCategoryNewID) && item.QuestionTypeID == SelectedTypeID && item.ParentId == null).Include(x => x.Children);
            if (!String.IsNullOrEmpty(searchQuestionPool))
            {
                iq = iq.Where(s => s.CategoryCode.Equals(searchQuestionPool)
                                       || s.CategoryID.ToString().Equals(searchQuestionPool)
                                       || s.questioncode.Equals(searchQuestionPool)
                                       || s.Origin.ToString().Equals(searchQuestionPool)
                                       || s.question.ToLower().Contains(searchQuestionPool.ToLower()));

            }

            int pageSize1 = Attendance.Data.Global.PageSize / 2;
            QuestionPool = await PaginatedList<Attendance.Models.QuestionPool>.CreateAsync(
                iq.AsNoTracking(), qPageIndex ?? 1, pageSize1);


            //Category = await _context.Category.ToListAsync();
        }

        public async Task<IActionResult> OnGetDuplicateAsync(Guid id, Guid categoryId, int qTypeID, int pageIndex, string currentFilterCategory, bool fromChild)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var selectedQuestion = await _context.QuestionPoolNew.Where(x => x.questionid == id).Include(x => x.Parent).FirstOrDefaultAsync();
            Attendance.Models.QuestionPool newQuestionPool = new Attendance.Models.QuestionPool();
            if (selectedQuestion != null)
            {
                if (fromChild)
                {
                    newQuestionPool.Parent = selectedQuestion.Parent;
                    newQuestionPool.ParentId = selectedQuestion.ParentId;
                }
                else
                {
                    newQuestionPool.Parent = selectedQuestion;
                    newQuestionPool.ParentId = selectedQuestion.questionid;
                }

                newQuestionPool.Origin = selectedQuestion.Origin;
                newQuestionPool.question = selectedQuestion.question + " DUPLICATED";
                newQuestionPool.questioncode = selectedQuestion.questioncode;
                newQuestionPool.questionid = Guid.NewGuid();
                newQuestionPool.QuestionTypeID = selectedQuestion.QuestionTypeID;
                newQuestionPool.CategoryCode = selectedQuestion.CategoryCode;
                newQuestionPool.CategoryID = selectedQuestion.CategoryID;
                newQuestionPool.CategoryNewID = selectedQuestion.CategoryNewID;
                newQuestionPool.comment = selectedQuestion.comment;
                _context.QuestionPoolNew.Add(newQuestionPool);
                await _context.SaveChangesAsync();
            }
            OnLog(AppMessages.QuestionPool_DuplicateSucceeded, PageAlertType.Success);
            return RedirectToPage("./Index", "", new { pageIndex = pageIndex, qPageIndex = 1, selectedCategoryNewID = categoryId.ToString(), selectedTypeID = qTypeID, currentFilterCategory = currentFilterCategory, currentFilterQuestionPool = "" });

        }

        public async Task<IActionResult> OnGetDeleteAsync(Guid id, Guid categoryId, int qTypeID, int pageIndex, string currentFilterCategory, bool fromChild)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var selectedQuestion = await _context.QuestionPoolNew.Where(x => x.questionid == id).Include(x => x.Children).FirstOrDefaultAsync();
            if (selectedQuestion != null)
            {
                //If deleted item is parent then its children also will be deleting with parent
                if (!fromChild && selectedQuestion.Children != null && selectedQuestion.Children.Count > 0)
                {
                    foreach (var item in selectedQuestion.Children)
                    {
                        _context.QuestionPoolNew.Remove(item);
                    }
                    await _context.SaveChangesAsync();
                }

                _context.QuestionPoolNew.Remove(selectedQuestion);
                await _context.SaveChangesAsync();
            }
            OnLog(AppMessages.QuestionPool_DeleteSucceeded, PageAlertType.Success);
            return RedirectToPage("./Index", "", new { pageIndex = pageIndex, qPageIndex = 1, selectedCategoryNewID = categoryId.ToString(), selectedTypeID = qTypeID, currentFilterCategory = currentFilterCategory, currentFilterQuestionPool = "" });

        }


        private void OnLog(string message, PageAlertType alertType)
        {
            AlertType = alertType;
            TempData["message"] = message;
            TempData["result"] = alertType.GetDescription();
        }

    }
}