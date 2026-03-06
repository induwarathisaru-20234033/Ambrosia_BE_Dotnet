using AMB.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AMB.Application.Interfaces.Repositories
{
    public interface IConfigRepository
    {
        Task AddReservationSettingAsync(ReservationSetting setting);
        Task UpdateReservationSettingAsync(int id, ReservationSetting setting);
        Task<ReservationSetting?> GetReservationSettingAsync();
        Task AddServiceHoursAsync(List<ServiceHour> serviceHours);
        Task RemoveServiceHoursAsync();
        Task<List<ServiceHour>> GetAllServiceHoursAsync();
    }
}
