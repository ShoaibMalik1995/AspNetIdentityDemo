using AspNetIdentityDemo.Shared.Models;
using AspNetIdentityDemo.Shared.ViewModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetIdentityDemo.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserManagerResponse> RegisterUserAsync(RegisterViewModel model);
        IEnumerable<IdentityUser> GetAllUsers();
        Task<IdentityUser> GetUserByEmailAsync(string email);
        Task<UserManagerResponse> LoginUserAsync(LoginViewModel model);
        Task<UserManagerResponse> ConfirmEmailAsync(string userId, string token);
        Task<UserManagerResponse> ForgetPasswordAsync(string email);
        Task<UserManagerResponse> ResetPasswordAsync(ResetPasswordViewModel model);

        Task<UserManagerResponse> AddUserRole(string userId, string roleName);
        Task<IList<string>> GetUserRoleByEmailAsync(string email);
    }
}
