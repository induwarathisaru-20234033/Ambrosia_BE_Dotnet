using AMB.Domain.Entities;

namespace AMB.Application.Interfaces.Repositories
{
    public interface ITableRepository
    {
        Task<Table?> GetByIdAsync(int id);
        Task<List<Table>> GetAllAvailableAsync();
    }
}