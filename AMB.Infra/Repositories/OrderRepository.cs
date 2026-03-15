using AMB.Application.Interfaces.Repositories;
using AMB.Domain.Entities;
using AMB.Infra.DBContexts;
using Microsoft.EntityFrameworkCore;

namespace AMB.Infra.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AMBContext _context;

        public OrderRepository(AMBContext context)
        {
            _context = context;
        }

        public async Task<Order?> GetByIdAsync(int id, OrderQueryOptions? options = null)
        {
            options ??= new OrderQueryOptions();

            IQueryable<Order> query = _context.Orders;

            if (options.IncludeItems)
            {
                query = query.Include(o => o.OrderItems!);

                if (options.IncludeMenuItem)
                {
                    query = query.Include(o => o.OrderItems!)
                        .ThenInclude(oi => oi.MenuItem);
                }
            }

            if (options.IncludeTable)
            {
                query = query.Include(o => o.Table);
            }

            return await query.FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Order> AddAsync(Order order)
        {
            // BaseEntity fields are set in DbContext SaveChangesAsync
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order> UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<List<Order>> GetDraftOrdersByTableAsync(int tableId)
        {
            return await _context.Orders
                .Where(o => o.TableId == tableId && o.OrderStatus == "Draft" && o.Status == 1)
                .Include(o => o.OrderItems)
                .ToListAsync();
        }

        public async Task<string> GenerateOrderNumberAsync()
        {
            var today = DateTime.UtcNow.ToString("yyyyMMdd");
            var lastOrderToday = await _context.Orders
                .Where(o => o.OrderNumber.StartsWith(today))
                .OrderByDescending(o => o.OrderNumber)
                .FirstOrDefaultAsync();

            if (lastOrderToday == null)
            {
                return $"{today}-001";
            }

            var lastNumber = int.Parse(lastOrderToday.OrderNumber.Split('-')[1]);
            return $"{today}-{(lastNumber + 1):D3}";
        }

        public async Task<bool> SendDraftToKdsAsync(int orderId, int? tableId = null)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return false;

            order.OrderStatus = "Sent to KDS";
            order.SentToKitchenAt = DateTime.UtcNow;
            order.UpdatedDate = DateTime.UtcNow;

            if (tableId.HasValue && tableId.Value > 0)
            {
                order.TableId = tableId.Value;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Order>> GetOrdersByStatusAsync(string status)
        {
            return await _context.Orders
                .Where(o => o.OrderStatus == status && o.Status == 1)
                .Include(o => o.OrderItems!)
                    .ThenInclude(oi => oi.MenuItem)
                .Include(o => o.Table)
                .OrderBy(o => o.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<Order>> GetKitchenOrdersAsync()
        {
            var kitchenStatuses = new[] { "Sent to KDS", "Preparing", "On Hold" };

            return await _context.Orders
                .Where(o => kitchenStatuses.Contains(o.OrderStatus) && o.Status == 1)
                .Include(o => o.OrderItems!)
                    .ThenInclude(oi => oi.MenuItem)
                .Include(o => o.Table)
                .OrderBy(o => o.CreatedDate)
                .ToListAsync();
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string newStatus, string? reason = null)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return false;

            order.OrderStatus = newStatus;
            order.UpdatedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Order?> GetOrderWithDetailsAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.OrderItems!)
                    .ThenInclude(oi => oi.MenuItem)
                .Include(o => o.Table)
                .FirstOrDefaultAsync(o => o.Id == id && o.Status == 1);
        }
    }
}