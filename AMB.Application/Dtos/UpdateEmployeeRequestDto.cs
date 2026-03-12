using AMB.Domain.Enums;

namespace AMB.Application.Dtos
{
    public class UpdateEmployeeRequestDto : CreateEmployeeRequestDto
    {
        public int Id { get; set; }
        public EntityStatus Status { get; set; }
    }
}
