using Attendance.Data;
using Attendance.Infrastructure.Data;
using Attendance.Models;
using Attendance.Providers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Attendance.Pages.AddOverview
{
    public class AddCategoryParam
    {
        public int QId;
        public Guid guid;
        public int? showAfterId;
    }

    public class IndexModel : PageModel
    {
        private readonly AttendanceContext _context;
        public PageAlertType AlertType = PageAlertType.Info;

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
        public int pQId { get; set; }
        public int pPageIndex { get; set; }
        public int? ShowAfterId { get; set; }

        public async Task OnGetAsync(int? pageIndex, int? qPageIndex, Guid categoryNewID, Guid selectedCategoryNewID,
            int typeID, int selectedTypeID,
            string currentFilterCategory, string currentFilterQuestionPool,
            string searchCategory, string searchQuestionPool,
            int pQId, int pPageIndex, int? showAfterThisID
            )
        {
            this.pQId = pQId;
            ShowAfterId = showAfterThisID;
            this.pPageIndex = pPageIndex;

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

            iq = iq.Where(item => !item.questioncode.Trim().Equals("") && !item.question.Trim().Equals("") && item.CategoryNewID.Equals(SelectedCategoryNewID) && item.QuestionTypeID == SelectedTypeID);
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

            foreach (var item in QuestionPool)
            {
                VIQ checkForQuestViq = _context.VIQ.FirstOrDefault(m => m.ObjectId.Equals(item.questionid) && m.QId == pQId);
                if (checkForQuestViq != null)
                {
                    item.IsAlreadyAddedQuestion = true;
                }
                else
                {
                    item.IsAlreadyAddedQuestion = false;
                }
            }

            foreach (var item in Category)
            {
                if (SelectedCategoryNewID != Guid.Empty)
                {
                    VIQ checkForCatViq = _context.VIQ.FirstOrDefault(m => m.ObjectId.Equals(item.CategoryNewID) && m.QId == pQId);
                    if (checkForCatViq != null)
                    {
                        item.IsAlreadyAddedCategory = true;
                    }
                    else
                    {
                        item.IsAlreadyAddedCategory = false;
                    }
                }
            }

            //Category = await _context.Category.ToListAsync();
        }

        public ActionResult OnPostAddCategory()
        {

            string res = "failed";
            Guid uid = Guid.Empty;
            string message = string.Empty;
            MemoryStream stream = new MemoryStream();
            Request.Body.CopyTo(stream);
            stream.Position = 0;
            using (StreamReader reader = new StreamReader(stream))
            {
                string requestBody = reader.ReadToEnd();
                if (requestBody.Length > 0)
                {
                    var obj = JsonConvert.DeserializeObject<AddCategoryParam>(requestBody);
                    var showAfterId = obj.showAfterId;
                    Attendance.Models.Category category = _context.Category.FirstOrDefault(m => m.CategoryNewID.Equals(obj.guid));
                    if (category != null)
                    {
                        VIQ oldVIQ = _context.VIQ.FirstOrDefault(m => m.ObjectId.Equals(obj.guid) && m.QId == obj.QId);
                        if (oldVIQ != null)
                        {
                            res = "success";
                            uid = obj.guid;
                            message = AppMessages.Viq_AlreadyAddedCategory;
                            OnLog(AppMessages.Viq_AlreadyAddedCategory, PageAlertType.Success);
                        }
                        else
                        {
                            int? newGlobalDisplayIndex = 1;
                            int? viqWithMaxDisplayIndex = _context.VIQ.Where(m => m.QId == obj.QId).Max(x => x.GlobalDisplayIndex);
                            if (viqWithMaxDisplayIndex != null)
                            {
                                newGlobalDisplayIndex = viqWithMaxDisplayIndex + 1;
                            }



                            VIQ vIQ = new VIQ();
                            vIQ.QId = obj.QId;
                            vIQ.ObjectId = obj.guid;
                            vIQ.Children = category.Children;
                            vIQ.ObjectType = 2;
                            vIQ.GlobalDisplayIndex = newGlobalDisplayIndex;
                            if (showAfterId != null)
                            {
                                var lastExistingViq = _context.VIQ.Where(m => m.QId == obj.QId && m.ShowAfterId == showAfterId).FirstOrDefault();
                                if (lastExistingViq != null)
                                {
                                    vIQ.ShowAfterId = lastExistingViq.GlobalDisplayIndex;
                                }
                            }
                            // Below code probably never get called
                            else
                            {
                                var currentList = _context.VIQ.Where(m => m.QId == obj.QId && m.ShowAfterId.HasValue).Select(x => x.ShowAfterId.Value).ToList();
                                if (currentList.Count == 0)
                                {
                                    vIQ.ShowAfterId = newGlobalDisplayIndex - 1;
                                }
                                else
                                {
                                    HashSet<int> myRange = new HashSet<int>(Enumerable.Range(0, (int)newGlobalDisplayIndex));
                                    myRange.ExceptWith(currentList);
                                    vIQ.ShowAfterId = myRange.Count() > 0 ? myRange.Max() : newGlobalDisplayIndex - 1;
                                }
                            }
                            _context.VIQ.Add(vIQ);
                            _context.SaveChanges();

                            // if records exist with same global display 
                            var existingViqWithSameGlobal = _context.VIQ.Where(m => m.QId == obj.QId && m.ShowAfterId == vIQ.ShowAfterId && m.Id != vIQ.Id).FirstOrDefault();
                            if (existingViqWithSameGlobal != null)
                            {
                                existingViqWithSameGlobal.ShowAfterId = newGlobalDisplayIndex;
                                _context.VIQ.Update(existingViqWithSameGlobal);
                                _context.SaveChanges();
                            }
                            res = "success";
                            uid = obj.guid;
                            message = AppMessages.Viq_CategoryAddedSuccessfully;
                            OnLog(AppMessages.Viq_CategoryAddedSuccessfully, PageAlertType.Success);

                        }
                    }
                }
            }

            List<string> lstString = new List<string>
            {
                res,
                res == "success" ? "The category has been added successfully." : "Oops!, Operation was failed.",
                uid.ToString(),
                message
            };
            return new JsonResult(lstString);
        }

        public ActionResult OnPostAddQuestion()
        {
            string res = "failed";
            Guid uid = Guid.Empty;
            string message = string.Empty;

            MemoryStream stream = new MemoryStream();
            Request.Body.CopyTo(stream);
            stream.Position = 0;
            using (StreamReader reader = new StreamReader(stream))
            {
                string requestBody = reader.ReadToEnd();
                if (requestBody.Length > 0)
                {
                    var obj = JsonConvert.DeserializeObject<AddCategoryParam>(requestBody);
                    var showAfterId = obj.showAfterId;
                    Attendance.Models.QuestionPool question = _context.QuestionPoolNew.FirstOrDefault(m => m.questionid.Equals(obj.guid));
                    if (question != null)
                    {
                        VIQ oldVIQ = _context.VIQ.FirstOrDefault(m => m.ObjectId.Equals(obj.guid) && m.QId == obj.QId);

                        if (oldVIQ != null)
                        {
                            res = "success";
                            uid = obj.guid;
                            message = AppMessages.Viq_AlreadyAddedQuestion;
                            OnLog(AppMessages.Viq_AlreadyAddedQuestion, PageAlertType.Success);

                        }
                        else
                        {
                            int? newGlobalDisplayIndex = 1;
                            int? viqWithMaxDisplayIndex = _context.VIQ.Where(m => m.QId == obj.QId).Max(x => x.GlobalDisplayIndex);
                            if (viqWithMaxDisplayIndex != null)
                            {
                                newGlobalDisplayIndex = viqWithMaxDisplayIndex + 1;
                            }
                            VIQ vIQ = new VIQ();
                            vIQ.QId = obj.QId;
                            vIQ.ObjectId = obj.guid;
                            vIQ.Children = 0;
                            vIQ.ObjectType = 0;
                            vIQ.GlobalDisplayIndex = newGlobalDisplayIndex;
                            if (showAfterId != null)
                            {
                                var lastExistingViq = _context.VIQ.Where(m => m.QId == obj.QId && m.ShowAfterId == showAfterId).FirstOrDefault();
                                if (lastExistingViq != null)
                                {
                                    vIQ.ShowAfterId = lastExistingViq.GlobalDisplayIndex;
                                }
                            }
                            // Below code probably never get called
                            else
                            {
                                var currentList = _context.VIQ.Where(m => m.QId == obj.QId && m.ShowAfterId.HasValue).Select(x => x.ShowAfterId.Value).ToList();
                                if (currentList.Count == 0)
                                {
                                    vIQ.ShowAfterId = newGlobalDisplayIndex - 1;
                                }
                                else
                                {
                                    HashSet<int> myRange = new HashSet<int>(Enumerable.Range(0, (int)newGlobalDisplayIndex));
                                    myRange.ExceptWith(currentList);
                                    vIQ.ShowAfterId = myRange.Count() > 0 ? myRange.Max() : newGlobalDisplayIndex - 1;
                                }
                            }

                            _context.VIQ.Add(vIQ);
                            _context.SaveChanges();

                            // if records exist with same global display 
                            var existingViqWithSameGlobal = _context.VIQ.Where(m => m.QId == obj.QId && m.ShowAfterId == vIQ.ShowAfterId && m.Id != vIQ.Id).FirstOrDefault();
                            if (existingViqWithSameGlobal != null)
                            {
                                existingViqWithSameGlobal.ShowAfterId = newGlobalDisplayIndex;
                                _context.VIQ.Update(existingViqWithSameGlobal);
                                _context.SaveChanges();
                            }

                            res = "success";
                            uid = obj.guid;
                            message = AppMessages.Viq_QuestionAddedSuccessfully;
                            OnLog(AppMessages.Viq_QuestionAddedSuccessfully, PageAlertType.Success);
                        }
                    }
                }
            }

            List<string> lstString = new List<string>
            {
                res,
                res == "success" ? "The category has been added successfully." : "Oops!, Operation was failed.",
                uid.ToString(),
                message
            };
            return new JsonResult(lstString);
        }

        private void OnLog(string message, PageAlertType alertType)
        {
            AlertType = alertType;
            TempData["message"] = message;
            TempData["result"] = alertType.GetDescription();
        }
    }
}