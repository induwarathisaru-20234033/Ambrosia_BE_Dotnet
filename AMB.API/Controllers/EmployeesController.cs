using AMB.Application.Dtos;
using AMB.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace AMB.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
            try
            {
                var result = await _employeeService.CreateEmployeeAsync(dto);

                var response = new BaseResponseDto<EmployeeDto>(result, "Employee created successfully!");

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
