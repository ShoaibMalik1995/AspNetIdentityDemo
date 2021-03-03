using AspNetIdentityDemo.Models;
using AspNetIdentityDemo.Services.Interfaces;
using AspNetIdentityDemo.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetIdentityDemo.Controllers
{
    public class AdminController : BaseController
    {
        #region Properties
        private readonly ApplicationDbContext DbContext;
        private readonly IProjectRoleService _projectRoleService;
        private readonly IConfiguration Configuration;
        #endregion

        #region Constr
        public AdminController(IProjectRoleService projectRoleService, IConfiguration configuration, ApplicationDbContext applicationDbContext)
        {
            this.DbContext = applicationDbContext;
            this._projectRoleService = projectRoleService;
            this.Configuration = configuration;
        }
        #endregion

        #region API Methods
        [HttpGet("GetMainMenu")]
        public IActionResult GetMainMenu()
        {
            var result = this.DbContext.MainMenu.ToList<MainMenus>();
            
            return Ok(result);
        }

        [HttpGet("Role")]
        public IActionResult GetRoles()
        {
            var result = this._projectRoleService.GetAllRole();
            if (result.Count > 0)
                return Ok(result);

            return NotFound();
        }

        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreatRoleAsync(ProjectRole model)
        {
            if(string.IsNullOrEmpty(model.Name))
                return BadRequest(model);

            var result = await this._projectRoleService.CreateRoleAsync(model);
            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpPost("CreateRoleClaim")]
        public async Task<IActionResult> CreateRoleClaimAsync(string roleName, string claimName)
        {
            if (string.IsNullOrEmpty(roleName) || string.IsNullOrEmpty(claimName))
                return BadRequest();

            var result = await this._projectRoleService.CreateRoleClaimAsync(roleName, claimName);
            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }

        #endregion
    }
}
