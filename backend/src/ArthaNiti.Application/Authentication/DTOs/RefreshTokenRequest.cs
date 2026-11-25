using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ArthaNiti.Application.Authentication.DTOs
{
    public class RefreshTokenRequest
    {

        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
