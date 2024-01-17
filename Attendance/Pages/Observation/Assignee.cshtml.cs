using System;
using System.Threading.Tasks;
using Attendance.Services.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Attendance.Pages.Observation
{
    public class AssigneeModel : PageModel
    {
        public Models.ObservationAssigneeInfo ObservationAssigneeInfo { get; set; }
        private readonly IAttendanceService _attendanceService;
        [BindProperty(SupportsGet = true)]
        public int observationId { get; set; }
        [BindProperty(SupportsGet = true)]
        public int pageNumber { get; set; }
        public AssigneeModel(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                if (observationId == 0)
                {
                    return NotFound();
                }
                ObservationAssigneeInfo = await _attendanceService.GetObservationAssigneeAsync(observationId);
                if (ObservationAssigneeInfo == null)
                {
                    return NotFound();
                }
                ObservationAssigneeInfo.pageNumber = pageNumber;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Page();
        }
    }
}
