using Adsbility.Appilication.Common.Interfaces;
using Adsbility.Appilication.Common.Models;
using Adsbility.Appilication.Common.Models.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Adsbility.Infrastructure.Identity
{
    public class JwtHandler : IJwtHandler
    {
        private readonly IConfiguration _configuration;

        public JwtHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreateToken(ApplicationUser user , string role)
        {

            var claims = new[]
            {
                new Claim(TokenClaims.Email , user.Email),
                new Claim(TokenClaims.UserId, user.Id),
                new Claim(TokenClaims.UserName, user.UserName),
                new Claim(TokenClaims.Role , role)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["ApplicationSettings:JWT_Secret"].ToString()));
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                );

            var tokenHandler = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenHandler;
        }
        public RefreshTokens CreateRefreshToken(string userId)
        {
            var refToken = new byte[32];
            using (var randomNumber = RandomNumberGenerator.Create())
            {
                randomNumber.GetBytes(refToken);
                var token = Convert.ToBase64String(refToken)
                    .Replace("=", string.Empty)
                    .Replace("+", string.Empty)
                    .Replace("/", string.Empty);
                return new RefreshTokens
                {
                    UserId = userId,
                    Token = token,
                    Expires = DateTime.UtcNow.AddDays(7),
                    Created = DateTime.UtcNow
                };
            }
        }

        public JwtSecurityToken ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                if (!JwtValidateSecureAlgorithm(securityToken))
                {
                    return null;
                }
                return securityToken;

            }
            catch (Exception)
            {

                return null;
            }
        }
        private bool JwtValidateSecureAlgorithm(SecurityToken validatedToken)
        {
            return (validatedToken is JwtSecurityToken jwtSecurityToken) &&
                jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
        }

    }
}
