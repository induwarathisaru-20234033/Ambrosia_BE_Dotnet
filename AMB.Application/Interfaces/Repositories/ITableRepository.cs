using AMB.Domain.Entities;
using AMB.Domain.Enums;

namespace AMB.Application.Interfaces.Repositories
{
    public interface ITableRepository
    {
        Task<Table> AddAsync(Table table);
        Task UpdateStatusAsync(int id, EntityStatus status);
        Task<List<Table>> GetAllAsync();
        IQueryable<Table> GetSearchQuery();
    }
}
