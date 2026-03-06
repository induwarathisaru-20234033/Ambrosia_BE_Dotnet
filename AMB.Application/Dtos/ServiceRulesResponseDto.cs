using System.Collections.Generic;

namespace AMB.Application.Dtos
{
    public class ServiceRulesResponseDto
    {
        public TimeSlotLogicRequestDto? TimeSlotLogic { get; set; }
        public List<ServiceShiftPayloadDto> ServiceShiftPayload { get; set; } = new List<ServiceShiftPayloadDto>();
    }
}
