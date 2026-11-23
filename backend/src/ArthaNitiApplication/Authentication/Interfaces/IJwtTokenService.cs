namespace ArthaNiti.Application.Authentication.Interfaces;

public interface IJwtTokenService
{
    Task<string> GenerateTokenAsync(
        string userId,
        string email,
        string fullName,
        int tenantId,
        IList<string> roles);
}
