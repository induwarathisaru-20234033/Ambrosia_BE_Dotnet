using AMB.API.Attributes;
using AMB.Application.Dtos;
using AMB.Application.Interfaces.Services;
using AMB.Application.Interfaces.Repositories;
using FluentValidation; 
using Microsoft.AspNetCore.Mvc;

namespace AMB.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableAuthorization]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly ILogger<RolesController> _logger;

        public RolesController(IRoleService roleService, ILogger<RolesController> logger)
        {
            _roleService = roleService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDto<RoleDto>>> CreateRole([FromBody] CreateRoleRequestDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(new BaseResponseDto<RoleDto>(
                        "Validation failed",
                        errors
                    ));
                }

                // Validate at least one permission selected
                if (dto.PermissionIds == null || dto.PermissionIds.Count == 0)
                {
                    return BadRequest(new BaseResponseDto<RoleDto>(
                        "Validation failed",
                        new List<string> { "Please select at least one permission for this role" }
                    ));
                }

                var result = await _roleService.CreateRoleAsync(dto);

                var response = new BaseResponseDto<RoleDto>(
                    result,
                    $"Role '{result.Name}' created successfully"
                );

                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new BaseResponseDto<RoleDto>(
                    ex.Message,
                    new List<string> { ex.Message }
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating role");
                return StatusCode(500, new BaseResponseDto<RoleDto>(
                    "Failed to create role. Please try again.",
                    new List<string> { "Internal server error" }
                ));
            }
        }

        [HttpGet("permissions/grouped")]
        public async Task<ActionResult<BaseResponseDto<List<PermissionGroupDto>>>> GetPermissionsGrouped()
        {
            try
            {
                var result = await _roleService.GetPermissionsGroupAsync();
                var response = new BaseResponseDto<List<PermissionGroupDto>>(
                    result,
                    "Permissions retrieved successfully"
                );

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving permissions");
                return StatusCode(500, new BaseResponseDto<List<PermissionGroupDto>>(
                    "Failed to retrieve permissions",
                    new List<string> { "Internal server error" }
                ));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BaseResponseDto<RoleDetailDto>>> GetRoleById(
        int id,
        [FromQuery] bool includePermissions = false,
        [FromQuery] bool includeFeatures = false)
        {
            try
            {
                var result = await _roleService.GetRoleByIdAsync(id, includePermissions, includeFeatures);

                var response = new BaseResponseDto<RoleDetailDto>(
                    result,
                    "Role retrieved successfully"
                );

                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new BaseResponseDto<RoleDetailDto>(
                    ex.Message,
                    new List<string> { ex.Message }
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving role");
                return StatusCode(500, new BaseResponseDto<RoleDetailDto>(
                    "Failed to retrieve role",
                    new List<string> { "Internal server error" }
                ));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<BaseResponseDto<object>>> UpdateRole(int id, [FromBody] EditRoleRequestDto dto)
        {
            try
            {
                if (id != dto.Id)
                {
                    return BadRequest(new BaseResponseDto<object>(
                        "ID mismatch",
                        new List<string> { "URL ID does not match request body ID" }
                    ));
                }

                // Validate 
                if (dto.PermissionIds == null || dto.PermissionIds.Count == 0)
                {
                    return BadRequest(new BaseResponseDto<object>(
                        "Validation failed",
                        new List<string> { "Please select at least one permission for this role" }
                    ));
                }

                await _roleService.UpdateRoleAsync(dto);

                return NoContent();
            }
            catch (ValidationException ex)
            {
                return BadRequest(new BaseResponseDto<object>(
                    "Validation failed",
                    ex.Errors.Select(e => e.ErrorMessage).ToList()
                ));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new BaseResponseDto<object>(
                    ex.Message,
                    new List<string> { ex.Message }
                ));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new BaseResponseDto<object>(
                    ex.Message,
                    new List<string> { ex.Message }
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating role");
                return StatusCode(500, new BaseResponseDto<object>(
                    "Failed to update role. Please try again.",
                    new List<string> { "Internal server error" }
                ));
            }
        }
    }
}