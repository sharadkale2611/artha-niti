using System;
using System.Collections.Generic;
using System.Text;

namespace ArthaNiti.Application.Authentication.DTOs
{
    public class RegisterResponse
    {
        public string UserId { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public int TenantId { get; set; }

        public IList<string> Roles { get; set; } = new List<string>();
    }
}
