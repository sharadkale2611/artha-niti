using FirmApplication = ArthaNiti.Domain.Entities.Application;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ArthaNiti.Domain.Entities;
using ArthaNiti.Infrastructure.Identity;
using ArthaNiti.Infrastructure.Persistence.Configurations;

namespace ArthaNiti.Infrastructure.Persistence;

public class ArthaNitiDbContext
    : IdentityDbContext<AppUser, AppRole, string>
{
    public ArthaNitiDbContext(
        DbContextOptions<ArthaNitiDbContext> options)
        : base(options)
    {
    }

    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<Tenant>(tenant =>
        {
            tenant.ToTable("Tenants");
            tenant.HasKey(x => x.TenantId);
            tenant.Property(x => x.Name).HasMaxLength(150).IsRequired();
            tenant.HasData(new Tenant { TenantId = 1, Name = "Development CA Firm" });
        });
        builder.Entity<AppUser>().HasOne<Tenant>().WithMany().HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<AppRole>().HasOne<Tenant>().WithMany().HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Restrict);

        builder.ApplyConfiguration(new ApplicationConfiguration());
    }

}
