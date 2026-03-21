using AMB.Domain.Entities;
using System.Threading.Tasks;

namespace AMB.Application.Interfaces.Repositories
{
    public interface IWastageRecordRepository
    {
        Task<WastageRecord> AddAsync(WastageRecord wastageRecord);
        Task<WastageRecord?> UpdateAsync(WastageRecord wastageRecord);
        Task<WastageRecord?> GetByIdAsync(int id);
        // Additional methods (get, update, etc.) can be added here later
    }
}
