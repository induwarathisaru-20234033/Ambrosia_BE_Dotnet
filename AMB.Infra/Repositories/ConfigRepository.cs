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
    }
}
