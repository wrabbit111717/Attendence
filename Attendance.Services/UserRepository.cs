using Attendance.Infrastructure.Data;
using Attendance.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Attendance.Services
{

    public interface IUserRepository
    {
        public Task<IdentityResult> CreateUser(User user);
        public Task<IdentityResult> UpdateUser(string userId, User user);
        public Task<User> FindByName(string userName);
        public Task<IdentityResult> UpdateRoles(User user, List<string> roles);
        public Task AddClaims(User user, List<Claim> claims);
        public Task<IList<string>> GetRoles(User user);
    }
    public class UserRepository : IUserRepository
    {
        private readonly AttendanceContext _context;
        private readonly UserManager<User> _userManager;
        public UserManager<User> UserManager
        {
            get { return _userManager; }
        }
        public UserRepository(AttendanceContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IdentityResult> CreateUser(User user)
        {
            return await _userManager.CreateAsync(user);
        }
        public async Task<IdentityResult> UpdateUser(string userId, User user)
        {
            return await _userManager.UpdateAsync(user);
        }
        public async Task<User> FindByName(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }
        public async Task<IdentityResult> UpdateRoles(User user, List<string> roles)
        {
            return await _userManager.AddToRolesAsync(user, roles);
        }
        public async Task AddClaims(User user, List<Claim> claims)
        {
            await _userManager.AddClaimsAsync(user, claims);
        }
        public async Task<IList<string>> GetRoles(User user)
        {
            return await _userManager.GetRolesAsync(user);
        }

    }
}
