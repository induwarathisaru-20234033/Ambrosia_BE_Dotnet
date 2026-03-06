using AMB.Application.Dtos;
using AMB.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMB.Application.Mappers
{
    public static class ConfigMapper
    {
        public static ReservationSetting ToReservationSettingEntity(this TimeSlotLogicRequestDto dto)
        {
            return new ReservationSetting
            {
                TurnTime = dto.TurnTime,
                BufferTime = dto.BufferTime,
                BookingInterval = dto.BookingInterval,
            };
        }

        public static ServiceHour ToServiceHourEntity(this ServiceShiftPayloadDto dto)
        {
            TimeOnly? startTime = null;
            TimeOnly? endTime = null;

            // Only extract time from DateTimeOffset if IsOpen is true and the DateTimeOffset values are not null
            if (dto.IsOpen)
            {
                if (dto.StartTime.HasValue)
                {
                    startTime = TimeOnly.FromDateTime(dto.StartTime.Value.UtcDateTime);
                }

                if (dto.EndTime.HasValue)
                {
                    endTime = TimeOnly.FromDateTime(dto.EndTime.Value.UtcDateTime);
                }
            }

            return new ServiceHour
            {
                Day = dto.Day,
                IsOpen = dto.IsOpen,
                StartTime = startTime,
                EndTime = endTime,
            };
        }
    }
}
