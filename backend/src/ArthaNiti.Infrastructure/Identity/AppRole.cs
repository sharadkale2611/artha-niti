using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace ArthaNiti.Infrastructure.Identity
{
    public class AppRole : IdentityRole
    {
        public int TenantId { get; set; }
    }
}
