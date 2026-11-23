using System.ComponentModel.DataAnnotations;

namespace ArthaNiti.Application.Authentication.DTOs;

public class LogoutRequest
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
