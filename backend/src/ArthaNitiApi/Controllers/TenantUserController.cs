
using ArthaNiti.Application.Authentication.DTOs;
using ArthaNiti.Application.Authentication.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArthaNiti.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TenantUserController : ControllerBase
    {
        private readonly ITenantUserService _service;
        private readonly ILogger<TenantUserController> _logger;

        public TenantUserController(
            ITenantUserService service,
            ILogger<TenantUserController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAll(
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _service.GetAllAsync(cancellationToken);

                return Ok(new ApiResponse<IEnumerable<TenantUserResponseDto>>
                {
                    Success = true,
                    Message = "Tenant users retrieved successfully.",
                    Data = result,
                    Errors = new List<string>()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving TenantUser records.");

                return StatusCode(500,
                    new ApiResponse<IEnumerable<TenantUserResponseDto>>
                    {
                        Success = false,
                        Message = "An unexpected error occurred while retrieving tenant users.",
                        Data = null,
                        Errors = new List<string>()
                    });
            }
        }

        // GET BY ID
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(
            int id,
            CancellationToken cancellationToken)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new ApiResponse<TenantUserResponseDto>
                    {
                        Success = false,
                        Message = "Invalid tenant user ID.",
                        Data = null,
                        Errors = new List<string>
                        {
                            "ID must be greater than zero."
                        }
                    });
                }

                var result = await _service.GetByIdAsync(
                    id, cancellationToken);

                if (result == null)
                {
                    return NotFound(new ApiResponse<TenantUserResponseDto>
                    {
                        Success = false,
                        Message = "Tenant user not found.",
                        Data = null,
                        Errors = new List<string>
                        {
                            "No tenant user exists with the specified ID."
                        }
                    });
                }

                return Ok(new ApiResponse<TenantUserResponseDto>
                {
                    Success = true,
                    Message = "Tenant user retrieved successfully.",
                    Data = result,
                    Errors = new List<string>()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error retrieving TenantUser record with ID {Id}.", id);

                return StatusCode(500,
                    new ApiResponse<TenantUserResponseDto>
                    {
                        Success = false,
                        Message = "An unexpected error occurred while retrieving the tenant user.",
                        Data = null,
                        Errors = new List<string>()
                    });
            }
        }

        // CREATE
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] TenantUserCreateDto dto,
            CancellationToken cancellationToken)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(new ApiResponse<TenantUserResponseDto>
                    {
                        Success = false,
                        Message = "Invalid request.",
                        Data = null,
                        Errors = new List<string>
                        {
                            "Request body cannot be null."
                        }
                    });
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .SelectMany(x => x.Value!.Errors)
                        .Select(x => string.IsNullOrWhiteSpace(x.ErrorMessage)
                            ? "A validation error occurred."
                            : x.ErrorMessage)
                        .ToList();

                    return BadRequest(new ApiResponse<TenantUserResponseDto>
                    {
                        Success = false,
                        Message = "Validation failed.",
                        Data = null,
                        Errors = errors
                    });
                }

                var result = await _service.CreateAsync(
                    dto, cancellationToken);

                return StatusCode(201,
                    new ApiResponse<TenantUserResponseDto>
                    {
                        Success = true,
                        Message = "Tenant user created successfully.",
                        Data = result,
                        Errors = new List<string>()
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating TenantUser record.");

                return StatusCode(500,
                    new ApiResponse<TenantUserResponseDto>
                    {
                        Success = false,
                        Message = "An unexpected error occurred while creating the tenant user.",
                        Data = null,
                        Errors = new List<string>()
                    });
            }
        }

        // UPDATE
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] TenantUserUpdateDto dto,
            CancellationToken cancellationToken)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new ApiResponse<TenantUserResponseDto>
                    {
                        Success = false,
                        Message = "Invalid tenant user ID.",
                        Data = null,
                        Errors = new List<string>
                        {
                            "ID must be greater than zero."
                        }
                    });
                }

                if (dto == null)
                {
                    return BadRequest(new ApiResponse<TenantUserResponseDto>
                    {
                        Success = false,
                        Message = "Invalid request.",
                        Data = null,
                        Errors = new List<string>
                        {
                            "Request body cannot be null."
                        }
                    });
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .SelectMany(x => x.Value!.Errors)
                        .Select(x => string.IsNullOrWhiteSpace(x.ErrorMessage)
                            ? "A validation error occurred."
                            : x.ErrorMessage)
                        .ToList();

                    return BadRequest(new ApiResponse<TenantUserResponseDto>
                    {
                        Success = false,
                        Message = "Validation failed.",
                        Data = null,
                        Errors = errors
                    });
                }

                var updated = await _service.UpdateAsync(
                    id, dto, cancellationToken);

                if (!updated)
                {
                    return NotFound(new ApiResponse<TenantUserResponseDto>
                    {
                        Success = false,
                        Message = "Tenant user not found.",
                        Data = null,
                        Errors = new List<string>
                        {
                            "No tenant user exists with the specified ID."
                        }
                    });
                }

                var result = await _service.GetByIdAsync(
                    id, cancellationToken);

                return Ok(new ApiResponse<TenantUserResponseDto>
                {
                    Success = true,
                    Message = "Tenant user updated successfully.",
                    Data = result,
                    Errors = new List<string>()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error updating TenantUser record with ID {Id}.", id);

                return StatusCode(500,
                    new ApiResponse<TenantUserResponseDto>
                    {
                        Success = false,
                        Message = "An unexpected error occurred while updating the tenant user.",
                        Data = null,
                        Errors = new List<string>()
                    });
            }
        }

        // DELETE (SOFT DELETE)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(
            int id,
            CancellationToken cancellationToken)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "Invalid tenant user ID.",
                        Data = false,
                        Errors = new List<string>
                        {
                            "ID must be greater than zero."
                        }
                    });
                }

                var deleted = await _service.DeleteAsync(
                    id, cancellationToken);

                if (!deleted)
                {
                    return NotFound(new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "Tenant user not found.",
                        Data = false,
                        Errors = new List<string>
                        {
                            "No tenant user exists with the specified ID."
                        }
                    });
                }

                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Message = "Tenant user deleted successfully.",
                    Data = true,
                    Errors = new List<string>()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error deleting TenantUser record with ID {Id}.", id);

                return StatusCode(500,
                    new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "An unexpected error occurred while deleting the tenant user.",
                        Data = false,
                        Errors = new List<string>()
                    });
            }
        }
    }
}
