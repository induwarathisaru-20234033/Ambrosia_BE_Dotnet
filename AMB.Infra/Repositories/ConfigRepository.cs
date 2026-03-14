using AMB.Application.Interfaces.Repositories;
using AMB.Domain.Entities;
using AMB.Infra.DBContexts;
using Microsoft.EntityFrameworkCore;

namespace AMB.Infra.Repositories
{
    public class ConfigRepository : IConfigRepository
    {
        private readonly AMBContext _context;

        public ConfigRepository(AMBContext context)
        {
            _context = context;
        }

        public async Task AddReservationSettingAsync(ReservationSetting setting)
        {
            _context.ReservationSettings.Add(setting);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateReservationSettingAsync(int id, ReservationSetting setting)
        {
            setting.Id = id;
            _context.ReservationSettings.Update(setting);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveReservationSettingAsync()
        {
            var existingSettings = await _context.ReservationSettings.ToListAsync();

            if (existingSettings.Count > 0)
            {
                _context.ReservationSettings.RemoveRange(existingSettings);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ReservationSetting?> GetReservationSettingAsync()
        {
            return await _context.ReservationSettings.AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task AddServiceHoursAsync(List<ServiceHour> serviceHours)
        {
            var existingHours = await _context.ServiceHours.ToListAsync();

            if (existingHours.Count > 0)
            {
                _context.ServiceHours.RemoveRange(existingHours);
            }

            if (serviceHours != null && serviceHours.Count > 0)
            {
                _context.ServiceHours.AddRange(serviceHours);
            }

            await _context.SaveChangesAsync();
        }

        public async Task RemoveServiceHoursAsync()
        {
            var existingHours = await _context.ServiceHours.ToListAsync();

            if (existingHours.Count > 0)
            {
                _context.ServiceHours.RemoveRange(existingHours);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<ServiceHour>> GetAllServiceHoursAsync()
        {
            return await _context.ServiceHours.AsNoTracking().ToListAsync();
        }

        public async Task AddBookingSlotsAsync(List<BookingSlot> bookingSlots)
        {
            var existingSlots = await _context.BookingSlots.ToListAsync();

            if (existingSlots.Count > 0)
            {
                _context.BookingSlots.RemoveRange(existingSlots);
            }

            if (bookingSlots != null && bookingSlots.Count > 0)
            {
                _context.BookingSlots.AddRange(bookingSlots);
            }

            await _context.SaveChangesAsync();
        }

        public async Task RemoveBookingSlotsAsync()
        {
            var existingSlots = await _context.BookingSlots.ToListAsync();

            if (existingSlots.Count > 0)
            {
                _context.BookingSlots.RemoveRange(existingSlots);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<BookingSlot>> GetAllBookingSlotsAsync()
        {
            return await _context.BookingSlots.AsNoTracking().ToListAsync();
        }

        public async Task<List<BookingSlot>> GetBookingSlotsByDayAsync(int day)
        {
            return await _context.BookingSlots.AsNoTracking().Where(s => s.Day == day).ToListAsync();
        }
    }
}
