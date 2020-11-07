using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Adsbility.Appilication.Common.Models
{
    public class ResetPasswordVM
    {
        [Required]
        public string Token { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required, DataType(DataType.Password)]
        [StringLength(50 , MinimumLength = 8)]
        public string NewPassword { get; set; }
        [Required, DataType(DataType.Password)]
        [StringLength(50, MinimumLength = 8)]
        public string ConfirmPassword { get; set; }
    }
}
