using AMB.Application.Dtos;
using AMB.Domain.Entities;

namespace AMB.Application.Mappers
{
    public static class ReservationMapper
    {
        public static ReservationDto ToReservationDto(this Reservation entity)
        {
            if (entity == null) return null;

            return new ReservationDto
            {
                Id = entity.Id,
                ReservationCode = entity.ReservationCode,
                PartySize = entity.PartySize,
                ReservationStatus = entity.ReservationStatus,
                ReservationDate = entity.ReservationDate,
                Occasion = entity.Occasion,
                SpecialRequests = entity.SpecialRequests,
                ArrivedAt = entity.ArrivedAt,
                NoShowMarkedAt = entity.NoShowMarkedAt,
                CancelledAt = entity.CancelledAt,
                CustomerDetail = entity.CustomerDetail?.ToCustomerDetailDto(),
                BookingSlot = entity.BookingSlot?.ToBookingSlotDto(),
                Table = entity.Table?.ToTableDto(),
                AssignedWaiterId = entity.AssignedWaiterId,
                AssignedWaiterName = entity.AssignedWaiter != null
                    ? $"{entity.AssignedWaiter.FirstName} {entity.AssignedWaiter.LastName}"
                    : null
            };
        }

        public static CustomerDetailDto ToCustomerDetailDto(this CustomerDetail entity)
        {
            if (entity == null) return null;

            return new CustomerDetailDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Email = entity.Email,
                PhoneNumber = entity.PhoneNumber
            };
        }

        public static BookingSlotDto ToBookingSlotDto(this BookingSlot entity)
        {
            if (entity == null) return null;

            return new BookingSlotDto
            {
                Id = entity.Id,
                SlotId = entity.SlotId,
                StartTime = entity.StartTime,
                EndTime = entity.EndTime,
                Day = entity.Day
            };
        }
    }
}
