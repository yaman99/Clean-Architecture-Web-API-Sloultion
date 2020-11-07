using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Adsbility.Appilication.Common.Models
{
    public static class TokenClaims
    {
        public const string UserId = "userId";
        public const string UserName = "userName";
        public const string Email = "email";
        public const string Role = ClaimTypes.Role;
    }
}
