using AMB.API.Attributes;
using AMB.Application.Dtos;
using AMB.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace AMB.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableAuthorization]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDto<EmployeeDto>>> Create(CreateEmployeeRequestDto dto)
        {
            var result = await _employeeService.CreateEmployeeAsync(dto);

            var response = new BaseResponseDto<EmployeeDto>(result, "Employee created successfully!");

            return Ok(response);
        }

        //Detuni
        [HttpGet]
        public async Task<ActionResult<BaseResponseDto<PagedResponseDto<EmployeeDto>>>> GetEmployees(
            [FromQuery] EmployeeFilterRequestDto filter)
        {
            var result = await _employeeService.GetEmployeesAsync(filter);

            return Ok(new BaseResponseDto<PagedResponseDto<EmployeeDto>>(
                result,
                "Employees retrieved successfully"
            ));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BaseResponseDto<EmployeeDto>>> GetEmployeeById(int id)
        {
            var result = await _employeeService.GetEmployeeByIdAsync(id);

            return Ok(new BaseResponseDto<EmployeeDto>(result, "Employee retrieved successfully"));
        }

        [HttpGet("waiters/current-allocations")]
        public async Task<ActionResult<BaseResponseDto<List<WaiterAllocationDto>>>> GetWaitersWithCurrentAllocations()
        {
            var result = await _employeeService.GetWaitersWithCurrentAllocationsAsync();

            return Ok(new BaseResponseDto<List<WaiterAllocationDto>>(
                result,
                "Waiters with current allocations retrieved successfully"
            ));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<BaseResponseDto<EmployeeDto>>> UpdateEmployee(int id, [FromBody] UpdateEmployeeRequestDto dto)
        {
            dto.Id = id;
            var result = await _employeeService.UpdateEmployeeAsync(dto);

            return Ok(new BaseResponseDto<EmployeeDto>(result, "Employee updated successfully"));
        }

        [HttpPatch("{id}/online-status")]
        public async Task<ActionResult<BaseResponseDto<EmployeeDto>>> UpdateEmployeeOnlineStatus(int id, [FromBody] UpdateEmployeeOnlineStatusRequestDto dto)
        {
            dto.Id = id;
            var result = await _employeeService.UpdateEmployeeOnlineStatusAsync(dto);

            return Ok(new BaseResponseDto<EmployeeDto>(result, "Employee online status updated successfully"));
        }

        [HttpPatch("assign-roles")]
        public async Task<ActionResult<BaseResponseDto<object>>> AssignRoles([FromBody] AssignEmployeeRolesRequestDto request)
        {
            await _employeeService.AssignRolesAsync(request);

            return Ok(new BaseResponseDto<object>(
                null,
                "Roles assigned successfully"
            ));
        }

    }
}
