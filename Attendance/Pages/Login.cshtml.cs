using Attendance.Data;
using Attendance.Providers;
using Attendance.Services.Services;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace Attendance.Pages
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly IAuthenticationService _authenticationService;

        public PageAlertType AlertType = PageAlertType.Info;

        [Required]
        [Column("Email")]
        [BindProperty]
        public string UserName { get; set; }

        [Required]
        [Column("Password")]
        [BindProperty]
        public string Password { get; set; }

        [BindProperty]
        public bool RememberMe { get; set; }

        public LoginModel(
            IAuthenticationService authenticationService

            )
        {
            _authenticationService = authenticationService;
        }

        public IActionResult OnGet()
        {
            if (User.Identity.IsAuthenticated)
            {
                var returnUrl = "~/";
                if (!string.IsNullOrEmpty(HttpContext.Request.Query["returnUrl"]))
                {
                    returnUrl = HttpContext.Request.Query["returnUrl"].ToString();
                }
                return Redirect(returnUrl);
            }
            return Page();
        }


        public async Task<IActionResult> OnPostAsync(string returnUrl)
        {
            if (!ModelState.IsValid)
                return Page();

            try
            {
                await _authenticationService.Login(UserName, Password, RememberMe);

                // TODO: from query
                if (!string.IsNullOrEmpty(HttpContext.Request.Query["returnUrl"]))
                {
                    returnUrl = HttpContext.Request.Query["returnUrl"].ToString();
                }
                else
                {
                    returnUrl ??= "~/";
                }
                return Redirect(returnUrl);
            }
            catch (Exception e)
            {
                OnLog(e.Message, PageAlertType.Danger);
            }


            // Check existing User and correct Password
            //User user = await _userManager.FindByNameAsync(UserName);
            //string message;
            //if (user == null)
            //{
            //    message = AppMessages.Login_UserNotExist;
            //}
            //else
            //{
            //    var checkPassword = await _userManager.CheckPasswordAsync(user, Password);
            //    if (!checkPassword)
            //        message = AppMessages.Login_PasswordIncorrect;
            //    else
            //    {
            //        await _signinManager.SignInAsync(user, RememberMe, "CookieAuthentication");

            //    }
            //}


            return Page();
        }

        private void OnLog(string message, PageAlertType alertType)
        {
            AlertType = alertType;
            TempData["message"] = message;
            TempData["result"] = alertType.GetDescription();
        }
    }
}