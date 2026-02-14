namespace AMB.Application.Dtos
{
    public class CreateServiceRulesRequestDto
    {
        public TimeSlotLogicRequestDto TimeSlotLogic {  get; set; }
        public List<ServiceShiftPayloadDto> ServiceShiftPayload { get; set; }

    }

    public class TimeSlotLogicRequestDto
    {
        public int BufferTime {  get; set; }
        public int TurnTime { get; set; }
        public int BookingInterval { get; set; }
    }

    public class ServiceShiftPayloadDto
    {
        public int Day { get; set; }
        public bool IsOpen { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }
}
