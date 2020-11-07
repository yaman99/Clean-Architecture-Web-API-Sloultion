using Adsbility.Appilication.Common.Models;
using Adsbility.Appilication.Common.Models.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Adsbility.Appilication.Common.Interfaces
{
    public interface IIdentityService
    {
        Task<Result> LoginUserAsync(LoginVM model);
        Task<Result> SignOutUserAsync(string token);
        Task<Result> CreateUserAsync(RegisterVM model);
        Task<Result> DeleteUserAsync(string userId);
        Task<Result> ConfirmEmailAsync(string userId, string token);
        Task<Result> ForgetPasswordAsync(string email);
        Task<Result> ResetPasswordAsync(ResetPasswordVM model);
        Task<Result> RefreshTokenAsync(JsonWebToken model);
        Task<Result> ResendEmailConfirmation(string email);
        Task<Result> ChangePasswordAsync(ChangeUserPassword model);
    }
}
