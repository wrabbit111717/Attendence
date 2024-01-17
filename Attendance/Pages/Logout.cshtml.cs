using Attendance.Services.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace Attendance.Pages
{
    public class LogoutModel : PageModel
    {
        private readonly IAuthenticationService _authenticationService;
        public LogoutModel(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }
        public async Task<IActionResult> OnGet()
        {
            if (User.Identity.IsAuthenticated)
            {
                await _authenticationService.Logout();
            }
            return RedirectToPage("/Login");
        }
    }
}
