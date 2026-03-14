using AMB.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AMB.Application.Interfaces.Repositories
{
    public interface IConfigRepository
    {
        Task AddReservationSettingAsync(ReservationSetting setting);
        Task UpdateReservationSettingAsync(int id, ReservationSetting setting);
        Task RemoveReservationSettingAsync();
        Task<ReservationSetting?> GetReservationSettingAsync();
        Task AddServiceHoursAsync(List<ServiceHour> serviceHours);
        Task RemoveServiceHoursAsync();
        Task<List<ServiceHour>> GetAllServiceHoursAsync();
        Task AddBookingSlotsAsync(List<BookingSlot> bookingSlots);
        Task RemoveBookingSlotsAsync();
        Task<List<BookingSlot>> GetAllBookingSlotsAsync();
        Task<List<BookingSlot>> GetBookingSlotsByDayAsync(int day);
    }
}
