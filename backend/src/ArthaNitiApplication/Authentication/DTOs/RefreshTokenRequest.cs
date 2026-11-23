using System.ComponentModel.DataAnnotations;

namespace ArthaNiti.Application.Authentication.DTOs;

public class RefreshTokenRequest
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
