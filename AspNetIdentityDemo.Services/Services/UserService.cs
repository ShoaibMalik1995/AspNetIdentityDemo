using AspNetIdentityDemo.Services.Interfaces;
using AspNetIdentityDemo.Shared.Models;
using AspNetIdentityDemo.Shared.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AspNetIdentityDemo.Services.Services
{
    public class UserService : IUserService
    {
        #region Properties
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration Configuration;
        #endregion

        #region Constr
        public UserService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IEmailService emailService, IConfiguration configuration)
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
            this._emailService = emailService;
            this.Configuration = configuration;
        }
        #endregion

        #region User Methods
        public async Task<UserManagerResponse> RegisterUserAsync(RegisterViewModel model)
        {
            if(model == null)
                throw new NullReferenceException("Register Model is null.");

            if (model.Password != model.ConfirmPassword)
                return new UserManagerResponse
                {
                    Message = "Confirm password doesn't mathc the password",
                    IsSuccess = false
                };

            var identityUser = new IdentityUser
            {
                Email = model.Email,
                UserName = model.Email,
            };

            var result = await _userManager.CreateAsync(identityUser, model.Password);

            if (result.Succeeded)
            {
                // TODO: Send A Confirmation Email 
                var confirmationEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(identityUser);
                var encodedEmailToken = Encoding.UTF8.GetBytes(confirmationEmailToken);
                var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);

                string url = $"{Configuration["appSettings:ApiURL"]}confirmemail?userid={identityUser.Id}&token={validEmailToken}";

                await _emailService.SendEmailAsync(identityUser.Email, "Confirm Your Email", "<h1>Welcome To Auth Demo</h1>" + 
                    $"<p>Please confirm your email <a href='{url}'>Click Here</a></p>");

                return new UserManagerResponse
                {
                    Message = "User Created Successfull.",
                    IsSuccess = true,
                };
            }

            return new UserManagerResponse
            {
                Message = "User didn't created.",
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description)
            };
        }

        public IEnumerable<IdentityUser> GetAllUsers()
        {
            var result = this._userManager.Users.ToList<IdentityUser>();
            return result;
        }

        public async Task<IdentityUser> GetUserByEmailAsync(string email)
        {
            var user = await this._userManager.FindByEmailAsync(email);
            return user;
        }

        public async Task<UserManagerResponse> LoginUserAsync(LoginViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if(user == null)
            {
                return new UserManagerResponse
                {
                    Message = "There is not user with that email address.",
                    IsSuccess = false,
                };
            }

            var result = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!result)
                return new UserManagerResponse
                {
                    Message = "Invalid Password",
                    IsSuccess = false,
                };

            var claims = new[]
            {
                new Claim(ClaimTypes.Email,model.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
            };

            var secret_Key = Configuration.GetSection("AppSettings:JWT_Secret").Value;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret_Key));

            var tokenDescriptor = new JwtSecurityToken(
                issuer: Configuration["AppSettings:Issuer"],
                audience: Configuration["AppSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(2),
                signingCredentials: new SigningCredentials(key,SecurityAlgorithms.HmacSha256));

            string token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

            return new UserManagerResponse
            {
                Message = token,
                IsSuccess = true,
                ExpireDate = tokenDescriptor.ValidTo
            };

        }

        public async Task<UserManagerResponse> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new UserManagerResponse
                {
                    Message = "User not found.",
                    IsSuccess = false,
                };

            // TODO: Send A Confirmation Email 
            var decodedtoken = WebEncoders.Base64UrlDecode(token);
            string normalToken = Encoding.UTF8.GetString(decodedtoken);

            var result = await _userManager.ConfirmEmailAsync(user, normalToken);
            if (result.Succeeded)
                return new UserManagerResponse
                {
                    Message = "Email confirmed successfully!",
                    IsSuccess = true,

                };

            return new UserManagerResponse
            {
                Message = "User did not confirm.",
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description),
            };
        }

        public async Task<UserManagerResponse> ForgetPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return new UserManagerResponse
                {
                    Message = "No user associated with email",
                    IsSuccess = false
                };

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = Encoding.UTF8.GetBytes(token);
            var validToken = WebEncoders.Base64UrlEncode(encodedToken);

            string url = $"{Configuration["appSettings:ApiURL"]}/ResetPassword?email={email}&token={validToken}";

            await _emailService.SendEmailAsync(email, "Reset Password", "<h1>Follow the instruction to reset your password</h1>" +
                $"<p>To reset your password <a href='{url}'>Click here</a>");

            return new UserManagerResponse
            {
                Message = "Reset password URL has been sent to the email successfully.",
                IsSuccess = true,
            };
        }

        public async Task<UserManagerResponse> ResetPasswordAsync(ResetPasswordViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return new UserManagerResponse
                {
                    Message = "No user associated with the email.",
                    IsSuccess = false
                };

            if (model.Password != model.ConfirmPassword)
                return new UserManagerResponse
                {
                    Message = "Password doesn't match it's confirmation",
                    IsSuccess = false
                };

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
                return new UserManagerResponse
                {
                    Message = "Password has been reset successfull.",
                    IsSuccess = true
                };

            return new UserManagerResponse
            {
                Message = "Somthing went wrong.",
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description)
            };
        }

        public async Task<UserManagerResponse> AddUserRole(string userId, string roleName)
        {
            var user = await this._userManager.FindByIdAsync(userId);
            if (user == null)
                return new UserManagerResponse
                {
                    Message = "User is not exists.",
                    IsSuccess = false
                };

            var role = await this._roleManager.FindByNameAsync(roleName);
            if (await this._userManager.IsInRoleAsync(user, role.Name))
                return new UserManagerResponse
                {
                    Message = "Role is already exists for this user.",
                    IsSuccess = false
                };

            var result =  await this._userManager.AddToRoleAsync(user, role.Name);
            if (result.Succeeded)
                return new UserManagerResponse
                {
                    Message = "User role added successfully.",
                    IsSuccess = true
                };


            return new UserManagerResponse
            {
                Message = "Something went wrong",
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description)
            };
        }

        public async Task<IList<string>> GetUserRoleByEmailAsync(string email)
        {
            IdentityRole identityRole = new IdentityRole();
            IList<Claim> roleclaims;
            IList<string> claimslist = new List<string>();
            var user = await this._userManager.FindByEmailAsync(email);
            if (user == null)
                return null;

            var userRole = await this._userManager.GetRolesAsync(user);
            foreach(var role in userRole)
            {
                identityRole = await this._roleManager.FindByNameAsync(role);
                roleclaims = await this._roleManager.GetClaimsAsync(identityRole);
                foreach (var item in roleclaims)
                    claimslist.Add(item.Value);
            }


            return claimslist;
        }
        #endregion
    }
}
