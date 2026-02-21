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
            return new ServiceHour
            {
                Day = dto.Day,
                IsOpen = dto.IsOpen,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
            };
        }
    }
}
