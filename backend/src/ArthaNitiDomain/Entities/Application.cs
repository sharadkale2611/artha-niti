using System;
using System.Collections.Generic;
using System.Text;

namespace ArthaNiti.Domain.Entities
{
    public class Application
    {
        public long Id { get; set; }

        public int TenantId { get; set; }

        public string ApplicationName { get; set; } = string.Empty;

        public string ApplicationCode { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string ClientId { get; set; } = string.Empty;

        public string ClientSecretHash { get; set; } = string.Empty;

        public string AuthenticationType { get; set; } = "OAuth2";

        public string Status { get; set; } = "Active";

        public string? AllowedOrigins { get; set; }

        public string? WebhookUrl { get; set; }

        public int RateLimitPerMinute { get; set; } = 100;

        public int DailyReminderLimit { get; set; } = 5000;

        public bool IsActive { get; set; } = true;

        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedAt { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTime? ExpiresAt { get; set; }
    }
}
