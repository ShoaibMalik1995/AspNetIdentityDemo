using AspNetIdentityDemo.Shared.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AspNetIdentityDemo.Services.Interfaces
{
    public interface IProjectRoleService
    {
        IList<IdentityRole> GetAllRole();
        Task<UserManagerResponse> CreateRoleAsync(ProjectRole model);

        Task<UserManagerResponse> CreateRoleClaimAsync(string roleName, string claimName);
    }
}
