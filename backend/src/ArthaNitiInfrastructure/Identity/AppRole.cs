using Microsoft.AspNetCore.Identity;

namespace ArthaNiti.Infrastructure.Identity;

public class AppRole : IdentityRole
{
    public int TenantId { get; set; }
}
