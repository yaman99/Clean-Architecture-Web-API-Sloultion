using Adsbility.Appilication.Common.Models;
using Adsbility.Appilication.Common.Models.Identity;
using Adsbility.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Adsbility.Infrastructure.Identity
{
    public interface IJwtHandler
    {
        public string CreateToken(ApplicationUser user , string role);
        public RefreshTokens CreateRefreshToken(string userId);
        public JwtSecurityToken ValidateToken(string token);
    }
}
