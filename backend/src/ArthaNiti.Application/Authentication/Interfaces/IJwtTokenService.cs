using System;
using System.Collections.Generic;
using System.Text;

namespace ArthaNiti.Application.Authentication.Interfaces
{
    public interface IJwtTokenService
    {
        Task<string> GenerateTokenAsync(
       string userId,
       string email,
       string fullName,
       int tenantId,
       IList<string> roles);
    }
}
