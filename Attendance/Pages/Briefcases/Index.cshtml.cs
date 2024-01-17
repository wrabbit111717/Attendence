using Attendance.Infrastructure.Data;
using Attendance.Services.DTOs;
using Attendance.Services.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Attendance.Pages.Briefcases
{
    public class IndexModel : PageModel
    {
        private readonly AttendanceContext _context;
        private readonly INewBriefcaseService _newBriefcaseService;
        private readonly string _userId;
        public IndexModel(AttendanceContext context, INewBriefcaseService newBriefcaseService, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _newBriefcaseService = newBriefcaseService;
            _userId = httpContextAccessor.HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier).Value;
            //_userId = User?.FindFirst(ClaimTypes.NameIdentifier).Value;
        }
        public void OnGet()
        {
            Page();
        }
        public async Task<IActionResult> OnGetBriefcasesAsync()
        {
            var briefcases = await _newBriefcaseService.GetBriefcases(_userId);
            return new JsonResult(briefcases);
        }
        public async Task<IActionResult> OnGetBriefcaseAsync(int? briefCaseId)
        {
            try
            {
                var model = await _newBriefcaseService.GetBriefcase(_userId, briefCaseId);
                return new JsonResult(model);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }

        }
        public async Task<IActionResult> OnGetPortListAsync([FromQuery] string query)
        {
            var ports = await _newBriefcaseService.GetPortList(query);
            return new JsonResult(ports);
        }
        public async Task<IActionResult> OnGetPortAsync(int portId)
        {
            var port = await _newBriefcaseService.GetPort(portId);
            return new JsonResult(port);
        }

        public async Task<IActionResult> OnPostCreateAsync([FromBody] BriefcasePayloadDTO briefcase)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestResult();
            }
            var result = await _newBriefcaseService.CreateBriefcase(_userId, briefcase);
            return new JsonResult(result);
        }
        public async Task<IActionResult> OnPutUpdateAsync(int briefcaseId, [FromBody] BriefcasePayloadDTO briefcase)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestResult();
            }
            await _newBriefcaseService.UpdateBriefcase(_userId, briefcaseId, briefcase);
            return new NoContentResult();
        }

    }
}
