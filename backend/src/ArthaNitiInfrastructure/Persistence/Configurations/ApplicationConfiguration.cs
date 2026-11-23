using FirmApplication = ArthaNiti.Domain.Entities.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ArthaNiti.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ArthaNiti.Infrastructure.Persistence.Configurations;

public class ApplicationConfiguration: IEntityTypeConfiguration<FirmApplication>
{
    public void Configure(EntityTypeBuilder<FirmApplication> builder)
    {
        builder.ToTable("Applications");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .UseIdentityColumn();

        builder.Property(x => x.TenantId)
            .IsRequired();

        builder.Property(x => x.ApplicationName)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.ApplicationCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.ClientId)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.ClientSecretHash)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.AuthenticationType)
            .HasMaxLength(50)
            .HasDefaultValue("OAuth2")
            .IsRequired();

        builder.Property(x => x.Status)
            .HasMaxLength(30)
            .HasDefaultValue("Active")
            .IsRequired();

        builder.Property(x => x.AllowedOrigins)
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.WebhookUrl)
            .HasMaxLength(500);

        builder.Property(x => x.RateLimitPerMinute)
            .HasDefaultValue(100)
            .IsRequired();

        builder.Property(x => x.DailyReminderLimit)
            .HasDefaultValue(5000)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(x => x.IsDeleted)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnType("datetime")
            .HasDefaultValueSql("GETDATE()")
            .IsRequired();

        builder.Property(x => x.CreatedBy)
            .HasMaxLength(450);

        builder.Property(x => x.UpdatedAt)
            .HasColumnType("datetime");

        builder.Property(x => x.UpdatedBy)
            .HasMaxLength(450);

        builder.Property(x => x.ExpiresAt)
            .HasColumnType("datetime");

        builder.HasIndex(x => x.ClientId)
            .IsUnique()
            .HasDatabaseName("UQ_Applications_ClientId");

        builder.HasIndex(x => new { x.TenantId, x.ApplicationCode })
            .IsUnique()
            .HasDatabaseName("UQ_Applications_TenantId_ApplicationCode");

        builder.HasIndex(x => x.TenantId)
            .HasDatabaseName("IX_Applications_TenantId");
    }


}
