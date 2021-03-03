using AspNetIdentityDemo.Services.Interfaces;
using AspNetIdentityDemo.Shared.Enums;
using AspNetIdentityDemo.Shared.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AspNetIdentityDemo.Services.Services
{
    public class ProjectRoleService : IProjectRoleService
    {
        #region Properties
        private readonly RoleManager<IdentityRole> _roleManager;
        #endregion

        #region Constr
        public ProjectRoleService(RoleManager<IdentityRole> roleManager)
        {
            this._roleManager = roleManager;
        }
        #endregion

        public IList<IdentityRole> GetAllRole()
        {
            var result = this._roleManager.Roles.ToList();
            return result;
        }

        public async Task<UserManagerResponse> CreateRoleAsync(ProjectRole model)
        {
            var roleExist = await this._roleManager.RoleExistsAsync(model.Name);
            if(!roleExist)
            {
                var result = await this._roleManager.CreateAsync(new IdentityRole { Name = model.Name });
                if (result.Succeeded)
                    return new UserManagerResponse
                    {
                        Message = "Role is created successfullly.",
                        IsSuccess = true,
                    };

                return new UserManagerResponse
                {
                    Message = "something went wrong.",
                    IsSuccess = false,
                    Errors = result.Errors.Select(e => e.Description)
                };
            }

            return new UserManagerResponse
            {
                Message = "Role name is already exists,",
                IsSuccess = false
            };
        }

        public async Task<UserManagerResponse> CreateRoleClaimAsync(string roleName, string claimName)
        {
            var role = await this._roleManager.FindByNameAsync(roleName);
            if(role != null)
            {
                var claimlist = await this._roleManager.GetClaimsAsync(role);
                if (claimlist.Any(e => e.Value == claimName))
                    return new UserManagerResponse
                    {
                        Message = "Role Claim is already exists.",
                        IsSuccess = false,
                    };

                var result = await this._roleManager.AddClaimAsync(role, new Claim(ClaimTypes.Role.ToString(), claimName));
                if (result.Succeeded)
                    return new UserManagerResponse
                    {
                        Message = "Role Claim Added Successfully.",
                        IsSuccess = true
                    };

                return new UserManagerResponse
                {
                    Message = "Something went wrong.",
                    IsSuccess = false
                };
            }

            return new UserManagerResponse
            {
                Message = "role is not exists.",
                IsSuccess = false
            };
        }
                
    }
}
