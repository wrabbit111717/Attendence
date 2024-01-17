using Attendance.Data;
using Attendance.Infrastructure.Data;
using Attendance.Models;
using Attendance.Providers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Attendance.Pages.VettingInfo
{
    public class IndexModel : PageModel
    {

        private readonly AttendanceContext _context;
        public PageAlertType AlertType = PageAlertType.Info;

        [BindProperty(SupportsGet = true)]
        public string Sort { get; set; }

        [BindProperty(SupportsGet = true)]
        public SortDirection Direction { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; }

        [BindProperty(SupportsGet = true)]
        public string CurrentFilter { get; set; }


        public IndexModel(AttendanceContext context)
        {
            _context = context;
        }

        public PaginatedList<VettingInfoDetail> VettingInfo { get; set; }

        public async Task OnGetAsync(int? pageIndex)
        {
            Sort = Sort ?? "vetId";
            if (SearchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                SearchString = CurrentFilter;
            }
            CurrentFilter = SearchString;
            //IQueryable<Attendance.Models.VettingInfo> vettingInfoIQ = from s in _context.VettingInfo
            //                                                          select s;
            //var majorName = from m in _context.Major
            //                where m.MajorId = 
            IQueryable<Attendance.Models.VettingInfoDetail> vettingInfoIQ = from vi in _context.VettingInfo
                                                                            join it in _context.InspectionTypes on vi.InspectionTypeId equals it.InspectionTypeId into s1
                                                                            from s in s1.DefaultIfEmpty()
                                                                            join viqinfo in _context.VIQInfo on vi.QId equals viqinfo.QId into s2
                                                                            from ss in s2.DefaultIfEmpty()
                                                                            join m in _context.Majors on
                                                                            vi.MajorId equals m.MajorId into m1
                                                                            from major in m1.DefaultIfEmpty()
                                                                            select new VettingInfoDetail()
                                                                            {
                                                                                VetId = vi.VetId,
                                                                                InspectorName = vi.InspectorName,
                                                                                InspectorSirName = vi.InspectorSirName,
                                                                                Port = vi.Port,
                                                                                Country = vi.Country,
                                                                                VetDate = vi.VetDate,
                                                                                VetShortDate = vi.VetDate.ToShortDateString(),
                                                                                VetTime = vi.VetTime,
                                                                                Password = vi.Password,
                                                                                Comments = vi.Comments,
                                                                                VesselName = vi.VesselName,
                                                                                VettingCode = vi.VettingCode,
                                                                                QId = vi.QId,
                                                                                VetGUI = vi.VetGUI,
                                                                                InspectionTypeId = vi.InspectionTypeId,
                                                                                VesselId = vi.VesselId,
                                                                                CountryId = vi.CountryId,
                                                                                PortId = vi.PortId,
                                                                                CompanyRepresentativeName = vi.CompanyRepresentativeName,
                                                                                RegistrationDate = vi.RegistrationDate,
                                                                                MajorId = vi.MajorId,
                                                                                Major = major.MajorName ?? vi.VettingCode.Substring(vi.VettingCode.LastIndexOf("-")).Trim().Trim('-'),
                                                                                RegisterName = vi.RegisterName,
                                                                                Answered = vi.Answered,
                                                                                Negative = vi.Negative,
                                                                                Positive = vi.Positive,
                                                                                SourceId = vi.SourceId,
                                                                                UserId = vi.UserId,
                                                                                InspectionTypeName = s.InspectionType,
                                                                                QuestTitle = ss.Title,
                                                                                QuestVersion = ss.version
                                                                            };

            if (!String.IsNullOrEmpty(SearchString))
            {
                vettingInfoIQ = vettingInfoIQ.Where(s => s.VesselName.Contains(SearchString)
                                       || s.InspectionTypeName.Contains(SearchString));
            }
            vettingInfoIQ = GetSortedVettingInfoIQ(vettingInfoIQ, Sort);

            int pageSize = Attendance.Data.Global.PageSize;
            VettingInfo = await PaginatedList<Attendance.Models.VettingInfoDetail>.CreateAsync(
                vettingInfoIQ.AsNoTracking(), pageIndex ?? 1, pageSize);

        }

        //public async Task<IActionResult> OnPostAsync(int? vetId, VettingInfoDetail item)
        //{
        //    // Comment update
        //    var vettingInfo = await _context.VettingInfo.Where(x => x.VetId == vetId).FirstOrDefaultAsync();
        //    vettingInfo.Comments = item.Comments;
        //    await _context.SaveChangesAsync();
        //    OnLog(AppMessages.Vetting_UpdateCommentSucceeded1, PageAlertType.Success);
        //    return RedirectToPage("/VettingInfo/Index", "");

        //}

        private void OnLog(string message, PageAlertType alertType)
        {
            AlertType = alertType;
            TempData["message"] = message;
            TempData["result"] = alertType.GetDescription();
        }

        public SortDirection GetNextSortDirection(string name, SortDirection defaultOrder)
        {
            if (Sort?.ToLowerInvariant() != name?.ToLowerInvariant())
            {
                return defaultOrder;
            }
            return Direction == SortDirection.Desc ? SortDirection.Asc : SortDirection.Desc;
        }

        private IQueryable<VettingInfoDetail> GetSortedVettingInfoIQ(IQueryable<VettingInfoDetail> vettingInfoIQ, string sort)
        {
            if (string.IsNullOrEmpty(Sort))
            {
                return Direction == SortDirection.Asc ? vettingInfoIQ.OrderBy(item => item.VetId) : vettingInfoIQ.OrderByDescending(item => item.VetId); // a.goulielmos 10/12/2020
            }

            switch (sort)
            {
                case "vesselName":
                    vettingInfoIQ = Direction == SortDirection.Asc ? vettingInfoIQ.OrderBy(item => item.VesselName) : vettingInfoIQ.OrderByDescending(item => item.VesselName);
                    break;
                case "vetDate":
                    vettingInfoIQ = Direction == SortDirection.Asc ? vettingInfoIQ.OrderBy(item => item.VetDate) : vettingInfoIQ.OrderByDescending(item => item.VetDate);
                    break;
                case "insType":
                    vettingInfoIQ = Direction == SortDirection.Asc ? vettingInfoIQ.OrderBy(item => item.InspectionTypeName) : vettingInfoIQ.OrderByDescending(item => item.InspectionTypeName);
                    break;
                //case "major":
                //    vettingInfoIQ = Direction == SortDirection.Asc ? vettingInfoIQ.OrderBy(item => item.Major) : vettingInfoIQ.OrderByDescending(item => item.Major);
                //    break;
                //case "vetCode":
                //    vettingInfoIQ = Direction == SortDirection.Asc ? vettingInfoIQ.OrderBy(item => item.VettingCode) : vettingInfoIQ.OrderByDescending(item => item.VettingCode);
                //    break;
                case "companyRepr":
                    vettingInfoIQ = Direction == SortDirection.Asc ? vettingInfoIQ.OrderBy(item => item.CompanyRepresentativeName) : vettingInfoIQ.OrderByDescending(item => item.CompanyRepresentativeName);
                    break;
                case "place":
                    vettingInfoIQ = Direction == SortDirection.Asc ? vettingInfoIQ.OrderBy(item => item.Port) : vettingInfoIQ.OrderByDescending(item => item.Port);
                    break;
                case "insName":
                    vettingInfoIQ = Direction == SortDirection.Asc ? vettingInfoIQ.OrderBy(item => item.InspectorName) : vettingInfoIQ.OrderByDescending(item => item.InspectorName);
                    break;
                case "title":
                    vettingInfoIQ = Direction == SortDirection.Asc ? vettingInfoIQ.OrderBy(item => item.QuestTitle) : vettingInfoIQ.OrderByDescending(item => item.QuestTitle);
                    break;
                //case "questVers":
                //    vettingInfoIQ = Direction == SortDirection.Asc ? vettingInfoIQ.OrderBy(item => item.QuestVersion) : vettingInfoIQ.OrderByDescending(item => item.QuestVersion); 
                //    break;
                //case "regDate":
                //    vettingInfoIQ = Direction == SortDirection.Asc ? vettingInfoIQ.OrderBy(item => item.RegistrationDate) : vettingInfoIQ.OrderByDescending(item => item.RegistrationDate); 
                //    break;
                //case "qId":
                //    vettingInfoIQ = Direction == SortDirection.Asc ? vettingInfoIQ.OrderBy(item => item.QId) : vettingInfoIQ.OrderByDescending(item => item.QId); 
                //    break;
                //case "numAnswer":
                //    vettingInfoIQ = Direction == SortDirection.Asc ? vettingInfoIQ.OrderBy(item => item.Answered) : vettingInfoIQ.OrderByDescending(item => item.Answered); 
                //    break;
                default:
                    vettingInfoIQ = Direction == SortDirection.Asc ? vettingInfoIQ.OrderBy(item => item.VetId) : vettingInfoIQ.OrderByDescending(item => item.VetId);
                    break;
            }

            return vettingInfoIQ;
        }
    }
}
