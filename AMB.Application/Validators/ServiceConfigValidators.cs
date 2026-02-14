using AMB.Application.Dtos;
using FluentValidation;

namespace AMB.Application.Validators
{
    public class CreateServiceRulesRequestDtoValidator: AbstractValidator<CreateServiceRulesRequestDto>
    {
        public CreateServiceRulesRequestDtoValidator()
        {
            RuleFor(x => x.TimeSlotLogic)
                .NotNull().WithMessage("Time Slot Logic is required.")
                .SetValidator(new TimeSlotLogicRequestDtoValidator());

            RuleFor(x => x.ServiceShiftPayload)
                .NotNull().WithMessage("Service Shift Payload is required.")
                .NotEmpty().WithMessage("At least one shift must be defined.");

            RuleForEach(x => x.ServiceShiftPayload)
                .SetValidator(new ServiceShiftPayloadDtoValidator());
        }
    }

    public class TimeSlotLogicRequestDtoValidator : AbstractValidator<TimeSlotLogicRequestDto>
    {
        public TimeSlotLogicRequestDtoValidator()
        {
            RuleFor(x => x.BufferTime)
                .NotEmpty().WithMessage("Buffer Time is required.");

            RuleFor(x => x.TurnTime)
                .NotEmpty().WithMessage("Turn Time is required.");

            RuleFor(x => x.BookingInterval)
                .NotEmpty().WithMessage("Booking Interval is required.");
        }
    }

    public class UpdateServiceRulesRequestDtoValidator : AbstractValidator<UpdateServiceRulesRequestDto>
    {
        public UpdateServiceRulesRequestDtoValidator()
        {
            RuleFor(x => x.UpdatedTimeSlotLogic)
                .NotNull().WithMessage("Time Slot Logic is required.")
                .SetValidator(new TimeSlotLogicRequestDtoValidator());

            RuleFor(x => x.UpdatedServiceShiftPayload)
                .NotNull().WithMessage("Service Shift Payload is required.")
                .NotEmpty().WithMessage("At least one shift must be defined.");

            RuleForEach(x => x.UpdatedServiceShiftPayload)
                .SetValidator(new ServiceShiftPayloadDtoValidator());
        }
    }

    public class ServiceShiftPayloadDtoValidator : AbstractValidator<ServiceShiftPayloadDto> 
    {
        public ServiceShiftPayloadDtoValidator()
        {
            RuleFor(x => x.Day)
                .NotEmpty().WithMessage("Day is required.");

            When(x => x.IsOpen, () =>
            {
                RuleFor(x => x.StartTime)
                .NotEmpty().WithMessage("Start Time is required when the shift is open.");

                RuleFor(x => x.EndTime)
                .NotEmpty().WithMessage("End Time is required when the shift is open.")
                .GreaterThan(x => x.StartTime)
                .WithMessage("End Time must be after Start Time.");
            });
        }
    }
}
