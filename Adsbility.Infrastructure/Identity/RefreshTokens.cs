using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Adsbility.Infrastructure.Identity
{
    public class RefreshTokens
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Token { get; set; }
        [Required]
        public DateTime Expires { get; set; }
        public bool IsExpired => DateTime.UtcNow >= Expires;
        [Required]
        public DateTime Created { get; set; }
        public DateTime? Revoked { get; set; }
        public bool IsActive => Revoked == null && !IsExpired;

        [ForeignKey("UserId")]
        public ApplicationUser Users { get; set; }
        [Required]
        public string UserId { get; set; }
    }
}
