using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace ArthaNiti.Infrastructure.Identity
{
    public class AppUser : IdentityUser
    {
        public int TenantId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
