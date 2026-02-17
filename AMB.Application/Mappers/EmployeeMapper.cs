using AMB.Application.Dtos;
using AMB.Domain.Entities;

namespace AMB.Application.Mappers
{
    public static class EmployeeMapper
    {
        public static Employee ToEmployeeEntity(this CreateEmployeeRequestDto dto)
        {
            if (dto == null) return null;

            return new Employee
            {
                EmployeeId = dto.EmployeeId,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Username = dto.Username,
                MobileNumber = dto.MobileNumber,
                Address = dto.Address,
            };
        }

        public static EmployeeDto ToEmployeeDto(this Employee entity)
        {
            if (entity == null) return null;

            return new EmployeeDto
            {
                Id = entity.Id,
                EmployeeId = entity.EmployeeId,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                Email = entity.Email,
                Username = entity.Username,
                MobileNumber = entity.MobileNumber,
                Address = entity.Address,
                CreatedDate = entity.CreatedDate,
            };
        }
    }
}
