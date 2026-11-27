
using ArthaNiti.Domain.Entites;
using ArthaNiti.Domain.Entities;
using ArthaNiti.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ArthaNiti.Infrastructure.Persistence;

public class ArthaNitiDbContext : IdentityDbContext<AppUser, AppRole, string>
{
    public ArthaNitiDbContext(
        DbContextOptions<ArthaNitiDbContext> options)
        : base(options)
    {
    }

    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantRole> TenantRoles { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<AppRole>().ToTable("AspNetRoles");

        builder.Entity<Tenant>()
            .HasQueryFilter(t => t.IsDeleted == false);
    }

    //public DbSet<Tenant> Tenants { get; set; } = null!;
    public DbSet<TenantUserRole> TenantUserRoles { get; set; }
    public DbSet<TenantUser> TenantUsers { get; set; }
    public DbSet<TenantSetting> TenantSettings { get; set; }
<<<<<<< HEAD
    public DbSet<TenantRolePermission> TenantRolePermissions { get; set; }
    public DbSet<Menu> Menus { get; set; }
    public DbSet<TenantAuditLog> TenantAuditLogs { get; set; }

=======
    public DbSet<Permission> Permissions { get; set; }
>>>>>>> 1129377fa0e6c466ca25372a70ee69c263dd1055

    // DbSet-End
}
