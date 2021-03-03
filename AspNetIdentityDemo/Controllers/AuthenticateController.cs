using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetIdentityDemo.Services.Interfaces;
using AspNetIdentityDemo.Shared.ViewModels;
using Microsoft.Extensions.Configuration;
using AspNetIdentityDemo.Shared.Extensions;

namespace AspNetIdentityDemo.Controllers
{

    public class AuthenticateController : BaseController
    {
        #region Properties
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
        private readonly IConfiguration Configuration; 
        #endregion

        #region Constr
        public AuthenticateController(IUserService userService, IEmailService emailService, IConfiguration configuration)
        {
            this._userService = userService;
            this._emailService = emailService;
            this.Configuration = configuration;
        }
        #endregion

        #region API Methods        

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.RegisterUserAsync(model);

                if (result.IsSuccess)
                    return Ok(result); // Status Code 200

                return BadRequest(result);
            }
            return BadRequest(model); // Status Code 400
        }

        [HttpGet("GetAllUsers")]
        public IActionResult GetAllUsers()
        {
            var result = this._userService.GetAllUsers();
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("GetUserByEmail")]
        public async Task<IActionResult> GetUserByEmailAsync(string email)
        {
            if (!email.IsValidEmail())
                return BadRequest("Invalid email address");

            var user = await this._userService.GetUserByEmailAsync(email);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.LoginUserAsync(model);
                if (result.IsSuccess)
                {
                    await _emailService.SendEmailAsync(model.Email, "New Login", "<h1>Hey!, New Login to your acount noticed</h1><p>New login to your account at "+DateTime.Now+".</p>");
                    return Ok(result);
                }

                return BadRequest(result);
            }

            return BadRequest(model);
        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                return NotFound();

            var result = await _userService.ConfirmEmailAsync(userId, token);
            if (result.IsSuccess)
            {
                return Redirect($"{Configuration["appsettings:AppURL"]}/EmailTemplates/confirmemail.html");
            }

            return BadRequest(result);
        }

        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
                return NotFound();

            var result = await _userService.ForgetPasswordAsync(email);
            if (result.IsSuccess)
                return Ok(result); // Status Code 200

            return BadRequest(result); // Status Code 400

        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.ResetPasswordAsync(model);
                if (result.IsSuccess)
                    Ok(result);

                return BadRequest(result);
            }

            return BadRequest(model);
        }

        [HttpPost("AddUserRole")]
        public async Task<IActionResult> AddUserRole(string userId, string roleName)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(roleName))
                return BadRequest("UserId and Role Name is required.");

            var result = await this._userService.AddUserRole(userId, roleName);
            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpGet("GetUserRoleByEmail")]
        public async Task<IActionResult> GetUserRoleByEmailAsync(string email)
        {
            if (!email.IsValidEmail())
                BadRequest("email is Invalid");

            var result = await this._userService.GetUserRoleByEmailAsync(email);
            if (result == null)
                return NotFound();

            return Ok(string.Join(",", result));
        }
        #endregion
    }
}
