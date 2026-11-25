
using ArthaNiti.Application.DTOs;
using ArthaNiti.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ArthaNiti.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TenantController : ControllerBase
    {
        private readonly ITenantService _service;
        private readonly ILogger<TenantController> _logger;

        public TenantController(
            ITenantService service,
            ILogger<TenantController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _service.GetAllAsync(cancellationToken);

                return Ok(new ApiResponse<IEnumerable<TenantResponseDto>>
                {
                    Success = true,
                    Message = "Records retrieved successfully.",
                    Data = result,
                    Errors = new List<string>()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Tenantt records.");

                return StatusCode(500, new ApiResponse<IEnumerable<TenantResponseDto>>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving records.",
                    Data = null,
                    Errors = new List<string>()
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(
            int id,
            CancellationToken cancellationToken)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new ApiResponse<TenantResponseDto>
                    {
                        Success = false,
                        Message = "Invalid record ID.",
                        Data = null,
                        Errors = new List<string>
                        {
                            "ID must be greater than zero."
                        }
                    });
                }

                var result = await _service.GetByIdAsync(id, cancellationToken);

                if (result == null)
                {
                    return NotFound(new ApiResponse<TenantResponseDto>
                    {
                        Success = false,
                        Message = "Record not found.",
                        Data = null,
                        Errors = new List<string>
                        {
                            "No record exists with the specified ID."
                        }
                    });
                }

                return Ok(new ApiResponse<TenantResponseDto>
                {
                    Success = true,
                    Message = "Record retrieved successfully.",
                    Data = result,
                    Errors = new List<string>()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Tenantt record with ID {Id}.", id);

                return StatusCode(500, new ApiResponse<TenantResponseDto>
                {
                    Success = false,
                    Message = "An unexpected error occurred while retrieving the record.",
                    Data = null,
                    Errors = new List<string>()
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] TenantCreateDto dto,
            CancellationToken cancellationToken)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(new ApiResponse<TenantResponseDto>
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
                    var errors = new List<string>();

                    foreach (var entry in ModelState.Values)
                    {
                        foreach (var error in entry.Errors)
                        {
                            errors.Add(string.IsNullOrWhiteSpace(error.ErrorMessage)
                                ? "A validation error occurred."
                                : error.ErrorMessage);
                        }
                    }

                    return BadRequest(new ApiResponse<TenantResponseDto>
                    {
                        Success = false,
                        Message = "Validation failed.",
                        Data = null,
                        Errors = errors
                    });
                }

                var result = await _service.CreateAsync(dto, cancellationToken);

                if (result == null)
                {
                    return StatusCode(500, new ApiResponse<TenantResponseDto>
                    {
                        Success = false,
                        Message = "Unable to create the record.",
                        Data = null,
                        Errors = new List<string>()
                    });
                }

                return StatusCode(201, new ApiResponse<TenantResponseDto>
                {
                    Success = true,
                    Message = "Record created successfully.",
                    Data = result,
                    Errors = new List<string>()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Tenantt record.");

                return StatusCode(500, new ApiResponse<TenantResponseDto>
                {
                    Success = false,
                    Message = "An unexpected error occurred while creating the record.",
                    Data = null,
                    Errors = new List<string>()
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] TenantUpdateDto dto,
            CancellationToken cancellationToken)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new ApiResponse<TenantResponseDto>
                    {
                        Success = false,
                        Message = "Invalid record ID.",
                        Data = null,
                        Errors = new List<string>
                        {
                            "ID must be greater than zero."
                        }
                    });
                }

                if (dto == null)
                {
                    return BadRequest(new ApiResponse<TenantResponseDto>
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
                    var errors = new List<string>();

                    foreach (var entry in ModelState.Values)
                    {
                        foreach (var error in entry.Errors)
                        {
                            errors.Add(string.IsNullOrWhiteSpace(error.ErrorMessage)
                                ? "A validation error occurred."
                                : error.ErrorMessage);
                        }
                    }

                    return BadRequest(new ApiResponse<TenantResponseDto>
                    {
                        Success = false,
                        Message = "Validation failed.",
                        Data = null,
                        Errors = errors
                    });
                }

                var updated = await _service.UpdateAsync(id, dto, cancellationToken);

                if (!updated)
                {
                    return NotFound(new ApiResponse<TenantResponseDto>
                    {
                        Success = false,
                        Message = "Record not found.",
                        Data = null,
                        Errors = new List<string>
                        {
                            "No record exists with the specified ID."
                        }
                    });
                }

                var result = await _service.GetByIdAsync(id, cancellationToken);

                return Ok(new ApiResponse<TenantResponseDto>
                {
                    Success = true,
                    Message = "Record updated successfully.",
                    Data = result,
                    Errors = new List<string>()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating Tenant record with ID {Id}.", id);

                return StatusCode(500, new ApiResponse<TenantResponseDto>
                {
                    Success = false,
                    Message = "An unexpected error occurred while updating the record.",
                    Data = null,
                    Errors = new List<string>()
                });
            }
        }

        [HttpDelete("{id}")]
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
                        Message = "Invalid record ID.",
                        Data = false,
                        Errors = new List<string>
                        {
                            "ID must be greater than zero."
                        }
                    });
                }

                var deleted = await _service.DeleteAsync(id, cancellationToken);

                if (!deleted)
                {
                    return NotFound(new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "Record not found.",
                        Data = false,
                        Errors = new List<string>
                        {
                            "No record exists with the specified ID."
                        }
                    });
                }

                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Message = "Record deleted successfully.",
                    Data = true,
                    Errors = new List<string>()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Tenantt record with ID {Id}.", id);

                return StatusCode(500, new ApiResponse<bool>
                {
                    Success = false,
                    Message = "An unexpected error occurred while deleting the record.",
                    Data = false,
                    Errors = new List<string>()
                });
            }
        }
    }
}
