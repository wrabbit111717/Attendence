using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Attendance.Data;
using Attendance.Infrastructure.Data;
using Attendance.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Attendance.Pages.VettingInfo
{
    public class CrewEvaluationModel : PageModel
    {
        private readonly AttendanceContext _context;
        [BindProperty(SupportsGet = true)]
        public string Sort { get; set; }

        [BindProperty(SupportsGet = true)]
        public SortDirection Direction { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; }

        [BindProperty(SupportsGet = true)]
        public string CurrentFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? VetID { get; set; }
        public CrewEvaluationModel(AttendanceContext context)
        {
            _context = context;
        }
        public PaginatedList<CrewEvaluationInfo> CrewEvaluationDetails { get; set; }
        public async Task OnGetAsync(int? id)
        {
            VetID = id;
            Sort = Sort ?? "crewrank";
            IQueryable<CrewEvaluationInfo> CrewEvaluationInfo = (from frmDtl in _context.FormDetail
                                                                 join cwRank in _context.CrewRank on frmDtl.crewrank equals cwRank.CrewRankDesc
                                                                 where frmDtl.vetid == id
                                                                 select new CrewEvaluationInfo()
                                                                 {
                                                                     vetid = frmDtl.vetid,
                                                                     crewname = frmDtl.crewname,
                                                                     crewcomment = frmDtl.crewcomment,
                                                                     characterscore = frmDtl.characterscore,
                                                                     workingexperience = frmDtl.workingexperience,
                                                                     motivation = frmDtl.motivation,
                                                                     smscompliance = frmDtl.smscompliance,
                                                                     teamwork = frmDtl.teamwork,
                                                                     communication = frmDtl.communication,
                                                                     decisionmaking = frmDtl.decisionmaking,
                                                                     managerialskill = frmDtl.managerialskill,
                                                                     crewrank = frmDtl.crewrank,
                                                                     potentialcareerdevelopment = frmDtl.potentialcareerdevelopment,
                                                                     rank = cwRank.Rank
                                                                 });

            CrewEvaluationInfo = GetSortedVettingInfoIQ(CrewEvaluationInfo, Sort);

            int pageSize = Data.Global.All;
            CrewEvaluationDetails = await PaginatedList<CrewEvaluationInfo>.CreateAsyncAllRecords(
               CrewEvaluationInfo.AsNoTracking(), -1, pageSize);
        }

        public SortDirection GetNextSortDirection(string name, SortDirection defaultOrder)
        {
            if (Sort?.ToLowerInvariant() != name?.ToLowerInvariant())
            {
                return defaultOrder;
            }
            return Direction == SortDirection.Desc ? SortDirection.Asc : SortDirection.Desc;
        }

        private IQueryable<CrewEvaluationInfo> GetSortedVettingInfoIQ(IQueryable<CrewEvaluationInfo> vettingInfoIQ, string sort)
        {
            if (string.IsNullOrEmpty(Sort))
            {
                return Direction == SortDirection.Asc ? vettingInfoIQ.OrderBy(item => item.rank) : vettingInfoIQ.OrderByDescending(item => item.rank); // a.goulielmos 10/12/2020
            }

            switch (sort)
            {
                case "crewrank":
                    vettingInfoIQ = Direction == SortDirection.Asc ? vettingInfoIQ.OrderBy(item => item.rank) : vettingInfoIQ.OrderByDescending(item => item.rank);
                    break;
                default:
                    vettingInfoIQ = Direction == SortDirection.Asc ? vettingInfoIQ.OrderBy(item => item.rank) : vettingInfoIQ.OrderByDescending(item => item.rank);
                    break;
            }

            return vettingInfoIQ;
        }
    }
}
