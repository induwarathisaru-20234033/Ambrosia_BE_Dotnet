using AMB.API.Attributes;
using AMB.Application.Dtos;
using AMB.Application.Interfaces.Services;
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

        [HttpGet("check-code")]
        public async Task<ActionResult<BaseResponseDto<bool>>> CheckRoleCodeUnique([FromQuery] string code)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code))
                {
                    return BadRequest(new BaseResponseDto<bool>(
                        "Code is required",
                        new List<string> { "Role code is required" }
                    ));
                }

                var exists = await _roleService.CheckRoleCodeExistsAsync(code);
                var response = new BaseResponseDto<bool>(
                    !exists,
                    exists ? "Role code already exists" : "Role code is available"
                );

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking role code uniqueness");
                return StatusCode(500, new BaseResponseDto<bool>(
                    "Unable to connect to server. Please check your connection.",
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
    }
}