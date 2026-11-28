using ArthaNiti.Application.DTOs;
using ArthaNiti.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ArthaNiti.Api.Controllers
{
    [ApiController]
    [Authorize(Roles = "FirmAdmin,SuperAdmin")]
    [Route("api/[controller]")]
    public class MenuPermissionsController : ControllerBase
    {
        private readonly IMenuPermissionService _service;
        private readonly ILogger<MenuPermissionsController> _logger;

        public MenuPermissionsController(
            IMenuPermissionService service,
            ILogger<MenuPermissionsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // POST /api/MenuPermissions
        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        
        public async Task<IActionResult> Create(
            [FromBody] MenuPermissionCreateDto dto,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _service.CreateAsync(dto, cancellationToken);

                _logger.LogInformation(
                    "Assigned permission {PermissionId} to menu {MenuId}. Assignment ID {MenuPermissionId}",
                    result.PermissionId, result.MenuId, result.MenuPermissionId);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = result.MenuPermissionId },
                    Success("Menu permission assigned successfully.", result));
            }
            catch (InvalidOperationException ex) when (IsDuplicateAssignment(ex))
            {
                return Conflict(Failure(ex.Message));
            }
            catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
            {
                return ServerError(ex);
            }
        }

        // GET /api/MenuPermissions
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _service.GetAllAsync(cancellationToken);
                return Ok(Success("Menu permissions retrieved successfully.", result));
            }
            catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
            {
                return ServerError(ex);
            }
        }

        // GET /api/MenuPermissions/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(
            int id,
            CancellationToken cancellationToken)
        {
            if (id <= 0)
                return InvalidId();

            try
            {
                var result = await _service.GetByIdAsync(id, cancellationToken);

                if (result is null)
                    return AssignmentNotFound(id);

                return Ok(Success("Menu permission retrieved successfully.", result));
            }
            catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
            {
                return ServerError(ex);
            }
        }

        // PUT /api/MenuPermissions/5
        [HttpPut("{id:int}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] MenuPermissionUpdateDto dto,
            CancellationToken cancellationToken)
        {
            if (id <= 0)
                return InvalidId();

            try
            {
                var result = await _service.UpdateAsync(id, dto, cancellationToken);

                if (result is null)
                    return AssignmentNotFound(id);

                _logger.LogInformation(
                    "Updated menu permission {MenuPermissionId}. Menu {MenuId}, permission {PermissionId}",
                    id, result.MenuId, result.PermissionId);

                return Ok(Success("Menu permission updated successfully.", result));
            }
            catch (InvalidOperationException ex) when (IsDuplicateAssignment(ex))
            {
                return Conflict(Failure(ex.Message));
            }
            catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
            {
                return ServerError(ex);
            }
        }

        // PATCH /api/MenuPermissions/5/status?isActive=false
        //[HttpPatch("{id:int}/status")]
        //[Authorize(Roles = "SuperAdmin")]
        //public async Task<IActionResult> UpdateStatus(
        //    int id,
        //    [FromQuery, BindRequired] bool isActive,
        //    CancellationToken cancellationToken)
        //{
        //    if (id <= 0)
        //        return InvalidId();

        //    try
        //    {
        //        bool updated = await _service.UpdateStatusAsync(
        //            id, isActive, cancellationToken);

        //        if (!updated)
        //            return AssignmentNotFound(id);

        //        _logger.LogInformation(
        //            "Set IsActive to {IsActive} for menu permission {MenuPermissionId}",
        //            isActive, id);

        //        return Ok(Success(
        //            "Menu permission status updated successfully.",
        //            new { menuPermissionId = id, isActive }));
        //    }
        //    catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
        //    {
        //        return ServerError(ex);
        //    }
        //}

        // DELETE /api/MenuPermissions/5
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(
            int id,
            CancellationToken cancellationToken)
        {
            if (id <= 0)
                return InvalidId();

            try
            {
                bool deleted = await _service.DeleteAsync(id, cancellationToken);

                if (!deleted)
                    return AssignmentNotFound(id);

                _logger.LogInformation(
                    "Soft-deleted menu permission {MenuPermissionId}", id);

                return Ok(Success("Menu permission deleted successfully.", true));
            }
            catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
            {
                return ServerError(ex);
            }
        }

        private IActionResult InvalidId()
        {
            return BadRequest(Failure("Menu permission ID must be greater than zero."));
        }

        private IActionResult AssignmentNotFound(int id)
        {
            return NotFound(Failure($"Menu permission {id} was not found."));
        }

        private IActionResult ServerError(Exception exception)
        {
            string traceId = HttpContext.TraceIdentifier;

            _logger.LogError(
                exception,
                "MenuPermissions API failed for {Method} {Path}. Trace ID {TraceId}",
                Request.Method, Request.Path, traceId);

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                Failure($"Request failed. Trace ID {traceId}."));
        }

        private static bool IsDuplicateAssignment(InvalidOperationException exception)
        {
            return exception.Message is
                "This permission is already assigned to the menu." or
                "An assignment already exists for this menu and permission.";
        }

        private static ApiResponse<T> Success<T>(string message, T data)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data,
                Errors = new List<string>()
            };
        }

        private static ApiResponse<object> Failure(string message)
        {
            return new ApiResponse<object>
            {
                Success = false,
                Message = message,
                Data = null,
                Errors = new List<string> { message }
            };
        }
    }
}
