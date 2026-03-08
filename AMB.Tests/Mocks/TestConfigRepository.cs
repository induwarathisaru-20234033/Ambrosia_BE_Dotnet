using AMB.Application.Interfaces.Repositories;
using AMB.Domain.Entities;

namespace AMB.Tests.Mocks
{
    internal sealed class TestConfigRepository : IConfigRepository
    {
        public Dictionary<int, BookingSlot> BookingSlots { get; } = new();
        public ReservationSetting? ReservationSetting { get; set; }
        public List<ServiceHour> ServiceHours { get; set; } = new();

        public Task<ReservationSetting?> GetReservationSettingAsync()
        {
            return Task.FromResult(ReservationSetting);
        }

        public Task<List<ServiceHour>> GetAllServiceHoursAsync()
        {
            return Task.FromResult(ServiceHours);
        }

        public Task<List<BookingSlot>> GetAllBookingSlotsAsync()
        {
            return Task.FromResult(BookingSlots.Values.ToList());
        }

        public Task AddReservationSettingAsync(ReservationSetting setting)
        {
            if (setting.Id == 0)
            {
                setting.Id = 1;
            }
            ReservationSetting = setting;
            return Task.CompletedTask;
        }

        public Task AddServiceHoursAsync(List<ServiceHour> serviceHours)
        {
            ServiceHours = serviceHours;
            return Task.CompletedTask;
        }

        public Task AddBookingSlotsAsync(List<BookingSlot> bookingSlots)
        {
            foreach (var slot in bookingSlots)
            {
                if (slot.Id == 0)
                {
                    slot.Id = BookingSlots.Count + 1;
                }
                BookingSlots[slot.Id] = slot;
            }
            return Task.CompletedTask;
        }

        public Task UpdateReservationSettingAsync(int id, ReservationSetting setting)
        {
            setting.Id = id;
            ReservationSetting = setting;
            return Task.CompletedTask;
        }

        public Task RemoveServiceHoursAsync()
        {
            ServiceHours.Clear();
            return Task.CompletedTask;
        }

        public Task RemoveBookingSlotsAsync()
        {
            BookingSlots.Clear();
            return Task.CompletedTask;
        }
    }
}
