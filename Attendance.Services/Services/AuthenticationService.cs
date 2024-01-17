using ActiveDirectory;

using Attendance.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Attendance.Services.Services
{
    public interface IAuthenticationService
    {
        public Task Login(string userName, string password, bool rememberMe = false);
        public Task Logout();
    }
    public class AuthenticationService : IAuthenticationService
    {
        private readonly SignInManager<User> _signInManager;
        private readonly IUserRepository _userRepository;
        private readonly string Manager = "MANAGER";
        private readonly string Admin = "ADMIN";
        public AuthenticationService(
                IConfiguration config,
                SignInManager<User> signInManager,
                IUserRepository userRepository
        )
        {
            _signInManager = signInManager;
            _userRepository = userRepository;
        }
        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
        }
        //public async Task Login(string userName, string password, bool persists = false)
        //{
        //    var user = await _userRepository.FindByName(userName);
        //    var userExists = true;
        //    if (user == null)
        //    {
        //        user = new User();
        //        userExists = false;
        //    }

        //    user.UserName = !userExists ? userName : user?.UserName;
        //    user.FirstName = user?.FirstName ?? "";
        //    user.LastName = user?.LastName ?? "";
        //    user.Email = user?.Email;
        //    user.NormalizedUserName = !string.IsNullOrEmpty(user.NormalizedUserName) ? user?.NormalizedUserName.ToUpper() : string.Empty;
        //    user.NormalizedEmail = !string.IsNullOrEmpty(user.NormalizedEmail) ? user?.NormalizedEmail.ToUpper() : string.Empty;
        //    user.LockoutEnabled = true;

        //    IdentityResult result = null;

        //    if (!userExists)
        //    {
        //        result = await _userRepository.CreateUser(user);
        //    }
        //    else
        //    {
        //        result = await _userRepository.UpdateUser(user.Id, user);
        //    }


        //    if (!result.Succeeded)
        //    {
        //        throw new System
        //            .ComponentModel
        //            .DataAnnotations
        //            .ValidationException(string.Join(",", result.Errors.Select(_ => _.Description).ToArray()));
        //    }

        //    var urerRole = "Manager";
        //    await _userRepository.UpdateRoles(user, new List<string>() { urerRole });


        //    var rolesList = await _userRepository.GetRoles(user);
        //    var isMasterRole = "0";
        //    foreach (var item in rolesList)
        //    {
        //        if (!string.IsNullOrEmpty(item))
        //        {
        //            if (item.ToUpper().Equals(Manager) || item.ToUpper().Equals(Admin))
        //            {
        //                isMasterRole = "1";
        //                break;
        //            }
        //        }
        //    }

        //    var claims = new List<Claim>
        //        {
        //            new Claim(ClaimTypes.Name, user.LastName + " " + user.FirstName),
        //            new Claim("UserName", user?.UserName),
        //            new Claim("IsMasterRole", isMasterRole),
        //        };



        //    await _userRepository.AddClaims(user, claims);
        //    await _signInManager.SignInAsync(user, persists, "CookieAuthentication");
        //}

        public async Task Login(string userName, string password, bool persists = false)
        {
            // TODO : Allow Manager User
            var ad = new ActiveDirectoryManager();
            var userAD = ad.GetUserByLoginName(userName);
            if (userAD == null)
            {
                throw new KeyNotFoundException("Username not found in Active Directory!");
            }
            if (!ad.canBind("LDAP://ionia.gr", userName, password))
            {
                throw new KeyNotFoundException("Password mismatch error!");
            }
            var user = await _userRepository.FindByName(userName);
            var userExists = true;
            if (user == null)
            {
                user = new User();
                userExists = false;
            }
            user.UserName = userAD.LoginName;
            user.FirstName = userAD.FirstName ?? "";
            user.LastName = userAD.LastName ?? "";
            user.Email = userAD.EmailAddress;
            user.NormalizedUserName = user.UserName.ToUpper();
            user.NormalizedEmail = user.Email.ToUpper();
            user.LockoutEnabled = true;

            IdentityResult result = null;

            if (!userExists)
            {
                result = await _userRepository.CreateUser(user);
            }
            else
            {
                result = await _userRepository.UpdateUser(user.Id, user);
            }

            if (!result.Succeeded)
            {
                throw new System
                    .ComponentModel
                    .DataAnnotations
                    .ValidationException(string.Join(",", result.Errors.Select(_ => _.Description).ToArray()));
            }

            var urerRole = "User";
            await _userRepository.UpdateRoles(user, new List<string>() { urerRole });

            var rolesList = await _userRepository.GetRoles(user);
            var isMasterRole = "0";
            foreach (var item in rolesList)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    if (item.ToUpper().Equals(Manager) || item.ToUpper().Equals(Admin))
                    {
                        isMasterRole = "1";
                        break;
                    }
                }
            }

            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.LastName + " " + user.FirstName),
                    new Claim("UserName", user?.UserName),
                    new Claim("IsMasterRole", isMasterRole),
                };

            await _userRepository.AddClaims(user, claims);

            await _signInManager.SignInAsync(user, persists, "CookieAuthentication");
        }
    }
}
