using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Adsbility.Appilication.Common.Models.Identity
{
    public class ChangeUserPassword
    {
        [Required]
        public string userId { get; set; }
        [Required, DataType(DataType.Password)]
        public string OldPassword { get; set; }
        [Required ,DataType(DataType.Password)]
        public string NewPassword{ get; set; }
        [Required, DataType(DataType.Password), Compare("NewPassword")]
        public string ConfirmPassword{ get; set; }
    }
}
