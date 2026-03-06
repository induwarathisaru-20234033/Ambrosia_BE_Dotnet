using AMB.Application.Dtos;
using AMB.Application.Interfaces.Repositories;
using AMB.Application.Interfaces.Services;
using AMB.Application.Mappers;
using AMB.Domain.Entities;
using AMB.Domain.Enums;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AMB.Application.Services
{
    public class ConfigService : IConfigService
    {
        private const string TimeFormat = "HH:mm";
        private readonly IConfigRepository _configRepository;
        private readonly IServiceProvider _serviceProvider;

        public ConfigService(IConfigRepository configRepository, IServiceProvider serviceProvider)
        {
            _configRepository = configRepository;
            _serviceProvider = serviceProvider;
        }

        public async Task AddConfigurationsAsync(CreateServiceRulesRequestDto request)
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<CreateServiceRulesRequestDto>>();
            await validator.ValidateAndThrowAsync(request);

            var reservationSettingModel = request.TimeSlotLogic.ToReservationSettingEntity();

            var serviceHourRequests = request.ServiceShiftPayload;
            ValidateServiceHours(serviceHourRequests);

            var serviceHourModels = new List<ServiceHour>();

            for (var i = 0; i < serviceHourRequests.Count; i++)
            {
                var model = serviceHourRequests[i].ToServiceHourEntity();
                serviceHourModels.Add(model);
            }

            await _configRepository.AddReservationSettingAsync(reservationSettingModel);

            await _configRepository.AddServiceHoursAsync(serviceHourModels);
        }

        public async Task UpdateConfigurationsAsync(UpdateServiceRulesRequestDto request)
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<UpdateServiceRulesRequestDto>>();
            await validator.ValidateAndThrowAsync(request);

            var reservationSettingModel = request.UpdatedTimeSlotLogic.ToReservationSettingEntity();
            var reservationSettingId = request.UpdatedTimeSlotLogic.Id;

            if (reservationSettingId <= 0)
            {
                var existingSetting = await _configRepository.GetReservationSettingAsync();

                if (existingSetting == null)
                {
                    throw new InvalidOperationException("Reservation settings not found to update.");
                }

                reservationSettingId = existingSetting.Id;
            }

            var serviceHourRequests = request.UpdatedServiceShiftPayload;
            ValidateServiceHours(serviceHourRequests.Cast<ServiceShiftPayloadDto>().ToList());

            var serviceHourModels = new List<ServiceHour>();

            for (var i = 0; i < serviceHourRequests.Count; i++)
            {
                var model = serviceHourRequests[i].ToServiceHourEntity();
                serviceHourModels.Add(model);
            }

            await _configRepository.UpdateReservationSettingAsync(reservationSettingId, reservationSettingModel);
            await _configRepository.AddServiceHoursAsync(serviceHourModels);
        }

        public async Task<ServiceRulesResponseDto?> GetConfigurationsAsync()
        {
            var reservationSetting = await _configRepository.GetReservationSettingAsync();
            var serviceHours = await _configRepository.GetAllServiceHoursAsync();

            if (reservationSetting == null && (serviceHours == null || serviceHours.Count == 0))
            {
                return null;
            }

            var response = new ServiceRulesResponseDto
            {
                TimeSlotLogic = reservationSetting == null
                    ? null
                    : new TimeSlotLogicRequestDto
                    {
                        BufferTime = reservationSetting.BufferTime,
                        TurnTime = reservationSetting.TurnTime,
                        BookingInterval = reservationSetting.BookingInterval
                    }
            };

            if (serviceHours != null && serviceHours.Count > 0)
            {
                for (var i = 0; i < serviceHours.Count; i++)
                {
                    var hour = serviceHours[i];

                    // Convert TimeOnly back to DateTimeOffset for the response
                    DateTimeOffset? startTime = null;
                    DateTimeOffset? endTime = null;

                    if (hour.StartTime.HasValue)
                    {
                        var startDateTime = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day,
                            hour.StartTime.Value.Hour, hour.StartTime.Value.Minute, hour.StartTime.Value.Second, DateTimeKind.Utc);
                        startTime = new DateTimeOffset(startDateTime, TimeSpan.Zero);
                    }

                    if (hour.EndTime.HasValue)
                    {
                        var endDateTime = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day,
                            hour.EndTime.Value.Hour, hour.EndTime.Value.Minute, hour.EndTime.Value.Second, DateTimeKind.Utc);
                        endTime = new DateTimeOffset(endDateTime, TimeSpan.Zero);
                    }

                    response.ServiceShiftPayload.Add(new ServiceShiftPayloadDto
                    {
                        Day = hour.Day,
                        IsOpen = hour.IsOpen,
                        StartTime = startTime,
                        EndTime = endTime
                    });
                }
            }

            return response;
        }


        private static void ValidateServiceHours(List<ServiceShiftPayloadDto> serviceHourRequests)
        {
            foreach (var shift in serviceHourRequests)
            {
                if (!Enum.IsDefined(typeof(Day), shift.Day))
                {
                    throw new ArgumentException($"Invalid day: {shift.Day}.");
                }
            }

            var groupedByDay = serviceHourRequests.GroupBy(shift => shift.Day);

            foreach (var dayGroup in groupedByDay)
            {
                var dayLabel = Enum.IsDefined(typeof(Day), dayGroup.Key)
                    ? ((Day)dayGroup.Key).ToString()
                    : $"Day {dayGroup.Key}";

                var openShifts = new List<ServiceShiftPayloadDto>();

                foreach (var shift in dayGroup)
                {
                    if (!shift.IsOpen)
                    {
                        // When IsOpen is false, both StartTime and EndTime should be null
                        if ((shift.StartTime.HasValue && shift.StartTime.Value != default) ||
                            (shift.EndTime.HasValue && shift.EndTime.Value != default))
                        {
                            var startTimeStr = shift.StartTime.HasValue ? shift.StartTime.Value.UtcDateTime.ToString(TimeFormat) : "null";
                            var endTimeStr = shift.EndTime.HasValue ? shift.EndTime.Value.UtcDateTime.ToString(TimeFormat) : "null";
                            throw new ArgumentException(
                                $"{dayLabel} is closed but has service times {startTimeStr}-{endTimeStr}.");
                        }

                        continue;
                    }

                    // When IsOpen is true, both StartTime and EndTime must be provided
                    if (!shift.StartTime.HasValue || !shift.EndTime.HasValue)
                    {
                        throw new ArgumentException(
                            $"{dayLabel} is open but is missing StartTime or EndTime.");
                    }

                    if (shift.EndTime <= shift.StartTime)
                    {
                        var startTimeStr = shift.StartTime.Value.UtcDateTime.ToString(TimeFormat);
                        var endTimeStr = shift.EndTime.Value.UtcDateTime.ToString(TimeFormat);
                        throw new ArgumentException(
                            $"{dayLabel} has invalid service time {startTimeStr}-{endTimeStr}. EndTime must be after StartTime.");
                    }

                    openShifts.Add(shift);
                }

                if (openShifts.Count == 0)
                {
                    continue;
                }

                var ordered = openShifts.OrderBy(shift => shift.StartTime).ToList();

                for (var i = 1; i < ordered.Count; i++)
                {
                    var previous = ordered[i - 1];
                    var current = ordered[i];

                    if (current.StartTime < previous.EndTime)
                    {
                        var prevStartStr = previous.StartTime.Value.UtcDateTime.ToString(TimeFormat);
                        var prevEndStr = previous.EndTime.Value.UtcDateTime.ToString(TimeFormat);
                        var currStartStr = current.StartTime.Value.UtcDateTime.ToString(TimeFormat);
                        var currEndStr = current.EndTime.Value.UtcDateTime.ToString(TimeFormat);
                        throw new ArgumentException(
                            $"Service hours overlap on {dayLabel}: {prevStartStr}-{prevEndStr} and {currStartStr}-{currEndStr}.");
                    }
                }
            }
        }
    }
}
