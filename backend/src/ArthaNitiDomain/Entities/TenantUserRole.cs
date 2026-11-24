using System;
using System.Collections.Generic;
using System.Text;

namespace ArthaNiti.Domain.Entities
{
    public class TenantUserRole
    {
        public int TenantUserRoleId { get; set; }

        public int TenantId { get; set; }

        public int TenantUserId { get; set; }

        public int TenantRoleId { get; set; }

        public DateTime AssignedAt { get; set; }
    }
}
