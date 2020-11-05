using Adsbility.Appilication.Common.Interfaces;
using Adsbility.Appilication.Common.Models;
using Adsbility.Appilication.Common.Models.Identity;
using Adsbility.Appilication.Interfaces;
using Adsbility.Infrastructure.presistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Adsbility.Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {
        public readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        public readonly IConfiguration _configuration;
        private readonly IMailService _mailService;
        private readonly IJwtHandler _jwtHandler;
        private readonly AppilicationDbContext _context;
        private readonly TokenValidationParameters _tokenValidationParameters;

        public IdentityService(UserManager<ApplicationUser> userManager, IConfiguration configration, IMailService mailService, RoleManager<ApplicationRole> roleManager, IJwtHandler jwtHandler, AppilicationDbContext context, TokenValidationParameters tokenValidationParameters)
        {
            _userManager = userManager;
            _configuration = configration;
            _mailService = mailService;
            _roleManager = roleManager;
            _jwtHandler = jwtHandler;
            _context = context;
            _tokenValidationParameters = tokenValidationParameters;
        }

        public async Task<Result> CreateUserAsync(RegisterVM model)
        {
            if (model.Password != model.ConfirmPassword)
            {
                return Result.GeneralFailure("Check Your Password Confirmation");
            }

            var isRoleExist = await _roleManager.RoleExistsAsync(model.Role);

            if (!isRoleExist)
                return Result.GeneralFailure("This Role Doesn't Exist");

            var user = new ApplicationUser
            {
                Email = model.Email,
                UserName = model.UserName,
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            var role = await _userManager.AddToRoleAsync(user, model.Role);
            if (!result.Succeeded)
            {
                return Result.Failure(result.Errors.Select(x => x.Description));
            }

            await SendConfirmationEmail(user);

            return Result.Success("User Created Successfuly");
        }
        public async Task<Result> ResendEmailConfirmation(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return Result.GeneralFailure("This Email doesn't Exist");
            if (user.EmailConfirmed)
                return Result.GeneralFailure("This Email Has Confirmed Already");

            await SendConfirmationEmail(user);

            return Result.Success("Url Sent Successfully Please Check Your Email");
        }

        private async Task SendConfirmationEmail(ApplicationUser user)
        {
            var ConfirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var EncodedEmailtoken = Encoding.UTF8.GetBytes(ConfirmEmailToken);
            var ValidEmailToken = WebEncoders.Base64UrlEncode(EncodedEmailtoken);
            string url = $"{_configuration["AppUrl"]}/api/v1/auth/Confirmation?userid={user.Id}&token={ValidEmailToken}";

            await _mailService.SendEmailAsync(user.Email, "confirm Your Email", $"<h1>welcome to auth confirm email</h1> <p>please confirm your email <a href='{url}'>by click here</a></p>");
        }

        public async Task<Result> DeleteUserAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> LoginUserAsync(LoginVM model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            var result = await _userManager.CheckPasswordAsync(user, model.Password);

            if (!result || user.Email != model.Email || user == null)
                return Result.GeneralFailure("Email Or Password Wrong");


            if (!user.EmailConfirmed)
                return Result.GeneralFailure("please Check Your Email for Conformation");

            var userRoles = await _userManager.GetRolesAsync(user);

            var refreshToken = _jwtHandler.CreateRefreshToken(user.Id);
            var token = _jwtHandler.CreateToken(user , userRoles.FirstOrDefault());

            var jwt = new JsonWebToken
            {
                Token = token,
                RefreshToken = refreshToken.Token,
            };
            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return Result.ReturnToken(jwt);

        }

        public async Task<Result> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Result.GeneralFailure("User Not Found");
            if (user.EmailConfirmed)
                return Result.GeneralFailure("This Email Has Confirmed Already");

            var decodedToken = WebEncoders.Base64UrlDecode(token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);
            var result = await _userManager.ConfirmEmailAsync(user, normalToken);
            if (result.Succeeded)
            {
                return Result.Success("Email Confirmed Successfully");
            }
            return Result.Failure(result.Errors.Select(x => x.Description));
        }

        public async Task<Result> ForgetPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return Result.GeneralFailure("No user With This Email");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodeToken = Encoding.UTF8.GetBytes(token);
            var ValidToken = WebEncoders.Base64UrlEncode(encodeToken);

            string url = $"{_configuration["AppUrl"]}/api/v1/auth/ResetPassword?token={ValidToken}&email={email}";
            await _mailService.SendEmailAsync(email, "ResetPassword", "<h1> Reseting Password </h1>" + $"<p>to Reset Password <a href={url}>Press Here</a></p> <p>Token String : {ValidToken}</p>");

            return Result.Success("Reset Password Link Sent To Your Email" + $"({email}) Successfully");
        }

        public async Task<Result> ResetPasswordAsync(ResetPasswordVM model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (model.NewPassword != model.ConfirmPassword)
                return Result.GeneralFailure("Password Doesn't match its Confirmation");

            var decodedToken = WebEncoders.Base64UrlDecode(model.Token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);

            var result = await _userManager.ResetPasswordAsync(user, normalToken, model.NewPassword);

            if (result.Succeeded)
                return Result.Success("Password Reseted Successfully");
            return Result.Failure(result.Errors.Select(x => x.Description));
            
        }

        public async Task<Result> RefreshTokenAsync(JsonWebToken model)
        {

            var validatedToken = _jwtHandler.ValidateToken(model.Token);

            if (validatedToken == null)
                return Result.GeneralFailure("Invalid Token");

            if (validatedToken.ValidTo > DateTime.UtcNow)
                return Result.GeneralFailure("This Token Has Not Expired Yet");

            var userId = validatedToken.Claims.First(claim => claim.Type == TokenClaims.UserId).Value;

            var userRefreshToken = await _context.RefreshTokens.Where(x => x.Token == model.RefreshToken)
                                    .Include(c => c.Users)
                                    .Where(v =>v.Users.Id == userId)
                                    .FirstOrDefaultAsync();


            if(userRefreshToken == null)
                return Result.GeneralFailure("No refresh token for this user");

            if(userRefreshToken.Token != model.RefreshToken)
                return Result.GeneralFailure("Invalid Refresh Token");

            if(userRefreshToken.IsExpired)
                return Result.GeneralFailure("Expired Token");

            if (userRefreshToken.Revoked != null)
                return Result.GeneralFailure("This Token has Been Used and Revoked");

            if (userRefreshToken.IsActive)
            {
                var userRole = await _userManager.GetRolesAsync(userRefreshToken.Users);
                var newRefreshToken = _jwtHandler.CreateRefreshToken(userId);
                var token = _jwtHandler.CreateToken(userRefreshToken.Users, userRole.FirstOrDefault());

                var jwt = new JsonWebToken
                {
                    Token = token,
                    RefreshToken = newRefreshToken.Token,
                };

                userRefreshToken.Revoked = DateTime.UtcNow;
                _context.Update(userRefreshToken);
                await _context.AddAsync(newRefreshToken);
                await _context.SaveChangesAsync();

                return Result.ReturnToken(jwt);
            }
            return Result.GeneralFailure("SomeThing Wrong");
        }

        public async Task<Result> SignOutUserAsync(string token)
        {
            var validatedToken = _jwtHandler.ValidateToken(token);

            if (validatedToken == null)
                return Result.GeneralFailure("Invalid Token");

            var userId = validatedToken.Claims.First(claim => claim.Type == TokenClaims.UserId).Value;

            var refreshToken = await _context.RefreshTokens.Where(x => x.UserId == userId && x.Revoked == null).OrderBy(y => y.Created).LastOrDefaultAsync();

            if (refreshToken != null)
            {
                refreshToken.Revoked = DateTime.UtcNow;
                _context.RefreshTokens.Update(refreshToken);
                await _context.SaveChangesAsync();
            }
            
            return Result.Success();
        }
    }
}
