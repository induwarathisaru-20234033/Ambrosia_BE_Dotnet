using AMB.Domain.Entities;
using System.Threading.Tasks;

namespace AMB.Application.Interfaces.Repositories
{
    public interface IWastageRecordRepository
    {
        Task<WastageRecord> AddAsync(WastageRecord wastageRecord);
        // Additional methods (get, update, etc.) can be added here later
    }
}
