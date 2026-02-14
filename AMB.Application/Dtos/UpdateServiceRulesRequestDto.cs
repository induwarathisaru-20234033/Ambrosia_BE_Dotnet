namespace AMB.Application.Dtos
{
    public class UpdateServiceRulesRequestDto
    {
        public UpdateTimeSlotLogicRequestDto UpdatedTimeSlotLogic {  get; set; }
        public List<UpdateServiceShiftPayloadDto> UpdatedServiceShiftPayload { get; set; }
    }

    public class UpdateTimeSlotLogicRequestDto : TimeSlotLogicRequestDto
    {
        public int Id { get; set; }
    }

    public class UpdateServiceShiftPayloadDto : ServiceShiftPayloadDto
    {
        public int Id { get; set; }
    }
}
