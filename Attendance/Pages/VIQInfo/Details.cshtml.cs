using Attendance.Infrastructure.Data;
using Attendance.Models;
using Attendance.Services.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Attendance.Pages.VIQInfo
{
    public class DetailsModel : PageModel
    {
        private readonly AttendanceContext _context;
        private readonly IAttendanceService _attendanceService;

        public DetailsModel(AttendanceContext context, IAttendanceService attendanceService)
        {
            _context = context;
            _attendanceService = attendanceService;

        }

        public VIQInfoModel VIQInfoModel { get; set; }
        public int PageIndex { get; set; }
        public List<Attendance.Models.VIQDetailItem> VIQDetail { get; set; }


        public async Task<IActionResult> OnGetAsync(int? id, int pageIndex)
        {
            if (id == null)
            {
                return NotFound();
            }

            VIQInfoModel = await _context.VIQInfo.FirstOrDefaultAsync(m => m.QId == id);

            if (VIQInfoModel == null)
            {
                return NotFound();
            }

            PageIndex = pageIndex;

            VIQDetail = _attendanceService.GetVIQDetail(id ?? 0);

            return Page();
        }
    }
}
