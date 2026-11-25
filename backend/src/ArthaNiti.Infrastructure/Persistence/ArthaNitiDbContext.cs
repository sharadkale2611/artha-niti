
//using ArthaNiti.Domain.Entities;
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

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<AppRole>().ToTable("AspNetRoles");

        // Keep your existing entity configurations here.
    }

    //public DbSet<Tenant> Tenants { get; set; } = null!;


    // DbSet-End
}
