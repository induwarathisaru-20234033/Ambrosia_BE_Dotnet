using AMB.Domain.Entities;

namespace AMB.Application.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(int id, OrderQueryOptions? options = null);
        Task<Order> AddAsync(Order order);
        Task<Order> UpdateAsync(Order order);
        Task<List<Order>> GetDraftOrdersByTableAsync(int tableId);
        Task<string> GenerateOrderNumberAsync();
    }

    public class OrderQueryOptions
    {
        public bool IncludeItems { get; set; }
        public bool IncludeMenuItem { get; set; }
        public bool IncludeTable { get; set; }
    }
}