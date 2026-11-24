using ArthaNiti.Application.Authentication.DTOs;

namespace ArthaNiti.Api.Controllers
{
    internal class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<TenantUserResponseDto> Data { get; set; }
        public List<string> Errors { get; set; }
    }
}