using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Adsbility.Appilication.Common.Interfaces;
using Adsbility.Appilication.Common.Models;
using Adsbility.Appilication.Common.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Adsbility.Api.Controllers.V1
{
    [Produces("application/json")]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IIdentityService _identityService;
        private readonly IMailService _mailService;
        private readonly IConfiguration _configuration;

        public AuthController(IIdentityService identityService, IMailService mailService, IConfiguration configuration)
        {
            _identityService = identityService;
            _mailService = mailService;
            _configuration = configuration;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> CreateUserAsync([FromBody]RegisterVM model)
        {
            if(model == null)
            {
                throw new NullReferenceException("Register Model Is Null");
            }
             var result = await _identityService.CreateUserAsync(model);
            if (result.Succeeded)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        /// <remarks>
        /// Sample request:
        ///
        ///     POST /sign-in with Agent Role
        ///     {
        ///        "email": "rakolee1999sawan@gmail.com",
        ///        "password": "asd.A123"
        ///     }
        ///     POST /sign-in with Admin Role
        ///     {
        ///        "email": "administrator@localhost",
        ///        "password": "asd.A123"
        ///     }
        ///     POST /sign-in with Company Role
        ///     {
        ///        "email": "company@company.com",
        ///        "password": "asd.A123"
        ///     }
        ///
        /// </remarks>
        [HttpPost("sign-in")]
        public async Task<IActionResult> SignIn([FromBody] LoginVM model)
        {
            if (ModelState.IsValid)
            {
                var result = await _identityService.LoginUserAsync(model);
                if (result.Succeeded)
                {
                    //await _mailService.SendEmailAsync(model.Email , "New Login", "<h1>HI You loged in</h1>");
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest("something wrong");
        }

        [Authorize]
        [HttpPost("sign-out")]
        public async Task<IActionResult> SignOut([FromBody] string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                var result = await _identityService.SignOutUserAsync(token);
                if (result.Succeeded)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return NotFound();
        }

        [Authorize]
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody]JsonWebToken model)
        {
            if (ModelState.IsValid)
            {
                var result = await _identityService.RefreshTokenAsync(model);
                if (result.Succeeded)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest("something wrong");
        }
        [HttpGet("Confirmation")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
                return NotFound();
            var result = await _identityService.ConfirmEmailAsync(userId, token);

            if (result.Succeeded)
            {
                return Redirect($"{_configuration["AppUrl"]}/ConfirmEmail.html");
            }
            return BadRequest(result);

        }

        [HttpPost("ResendConfirmation/{email}")]
        public async Task<IActionResult> ResendConfirmation(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return NotFound();
            var result = await _identityService.ResendEmailConfirmation(email);

            if (result.Succeeded)
            {
                return Ok(result);
            }
            return BadRequest(result);

        }

        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
                return NotFound();
            var result = await _identityService.ForgetPasswordAsync(email);

            if (result.Succeeded)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordVM model)
        {
            var result = await _identityService.ResetPasswordAsync(model);
            if (result.Succeeded)
                return Ok(result);
            return BadRequest(result);
        }
        
        [Authorize]
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangeUserPassword model)
        {
            if (!ModelState.IsValid)
                return NotFound();

            var result = await _identityService.ChangePasswordAsync(model);
            if (result.Succeeded)
                return Ok(result);
            return BadRequest(result);
        }
    }
}
