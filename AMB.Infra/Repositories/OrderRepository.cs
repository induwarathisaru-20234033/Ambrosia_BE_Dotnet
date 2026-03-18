using AMB.Application.Interfaces.Repositories;
using AMB.Domain.Entities;
using AMB.Domain.Enums;
using AMB.Infra.DBContexts;
using Microsoft.EntityFrameworkCore;
using AMB.Application.Dtos;

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
                .Where(o => o.TableId == tableId && o.OrderStatus == (int)OrderStatus.Draft && o.Status == 1)
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

            order.OrderStatus = (int)OrderStatus.SentToKDS;
            order.SentToKitchenAt = DateTime.UtcNow;
            order.UpdatedDate = DateTime.UtcNow;

            if (tableId.HasValue && tableId.Value > 0)
            {
                order.TableId = tableId.Value;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Order>> GetOrdersByStatusAsync(OrderStatus status)
        {
            return await _context.Orders
                .Where(o => o.OrderStatus == (int)status && o.Status == 1)
                .Include(o => o.OrderItems!)
                    .ThenInclude(oi => oi.MenuItem)
                .Include(o => o.Table)
                .OrderBy(o => o.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<Order>> GetKitchenOrdersAsync()
        {
            var kitchenStatuses = new[]
            {
                (int)OrderStatus.SentToKDS,
                (int)OrderStatus.Preparing,
                (int)OrderStatus.OnHold
            };

            return await _context.Orders
                .Where(o => kitchenStatuses.Contains(o.OrderStatus) && o.Status == 1)
                .Include(o => o.OrderItems!)
                    .ThenInclude(oi => oi.MenuItem)
                .Include(o => o.Table)
                .OrderBy(o => o.CreatedDate)
                .ToListAsync();
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus, string? reason = null)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return false;

            order.OrderStatus = (int)newStatus;
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
        public async Task<(List<Order> Items, int TotalCount)> SearchOrdersAsync(SearchOrderRequestDto request)
        {
            var pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
            var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;

            IQueryable<Order> query = _context.Orders
                .AsNoTracking()
                .Where(o => o.Status == 1)
                .Include(o => o.OrderItems!)
                    .ThenInclude(oi => oi.MenuItem)
                .Include(o => o.Table);

            // Filter by category
            if (!string.IsNullOrWhiteSpace(request.Category))
            {
                if (request.Category.Equals("ongoing", StringComparison.OrdinalIgnoreCase))
                {
                    var ongoingStatuses = new[]
                    {
                (int)OrderStatus.SentToKDS,
                (int)OrderStatus.Preparing,
                (int)OrderStatus.OnHold
            };

                    query = query.Where(o => ongoingStatuses.Contains(o.OrderStatus));
                }
                else if (request.Category.Equals("completed", StringComparison.OrdinalIgnoreCase))
                {
                    var completedStatuses = new[]
                    {
                (int)OrderStatus.Served,
                (int)OrderStatus.Cancelled
            };

                    query = query.Where(o => completedStatuses.Contains(o.OrderStatus));
                }
            }

            // Filter by exact status
            if (request.Status.HasValue)
            {
                query = query.Where(o => o.OrderStatus == (int)request.Status.Value);
            }

            // Filter by order number
            if (!string.IsNullOrWhiteSpace(request.OrderNumber))
            {
                query = query.Where(o => o.OrderNumber.Contains(request.OrderNumber));
            }

            // Filter by table name
            if (!string.IsNullOrWhiteSpace(request.TableName))
            {
                query = query.Where(o => o.Table != null && o.Table.TableName.Contains(request.TableName));
            }

            // Filter by date range
            if (request.OrderDateFrom.HasValue)
            {
                query = query.Where(o => o.CreatedDate >= request.OrderDateFrom.Value);
            }

            if (request.OrderDateTo.HasValue)
            {
                query = query.Where(o => o.CreatedDate <= request.OrderDateTo.Value);
            }

            // Sorting
            query = request.SortField?.ToLower() switch
            {
                "ordernumber" => request.SortOrder == -1
                    ? query.OrderByDescending(o => o.OrderNumber)
                    : query.OrderBy(o => o.OrderNumber),

                "tablename" => request.SortOrder == -1
                    ? query.OrderByDescending(o => o.Table != null ? o.Table.TableName : "")
                    : query.OrderBy(o => o.Table != null ? o.Table.TableName : ""),

                "createddate" => request.SortOrder == -1
                    ? query.OrderByDescending(o => o.CreatedDate)
                    : query.OrderBy(o => o.CreatedDate),

                "orderstatus" => request.SortOrder == -1
                    ? query.OrderByDescending(o => o.OrderStatus)
                    : query.OrderBy(o => o.OrderStatus),

                _ => query.OrderByDescending(o => o.CreatedDate)
            };

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<bool> UpdateDraftOrderItemsAsync(int orderId, List<OrderItemDto> items)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o => o.Id == orderId);

                if (order == null) return false;

                // Get menu items for pricing
                var menuItemIds = items.Select(i => i.MenuItemId).ToList();
                var menuItems = await _context.MenuItems
                    .Where(m => menuItemIds.Contains(m.Id))
                    .ToDictionaryAsync(m => m.Id, m => m.Price);

                // Group items by MenuItemId from request (combine duplicates)
                var requestItems = items.GroupBy(i => i.MenuItemId)
                    .Select(g => new
                    {
                        MenuItemId = g.Key,
                        Quantity = g.Sum(i => i.Quantity),
                        SpecialInstructions = string.Join("; ", g.Select(i => i.SpecialInstructions).Where(s => !string.IsNullOrEmpty(s)))
                    })
                    .ToList();

                // Track which items we've processed
                var processedMenuItemIds = new HashSet<int>();

                // Update existing items or add new ones
                foreach (var requestItem in requestItems)
                {
                    var existingItem = order.OrderItems
                        .FirstOrDefault(oi => oi.MenuItemId == requestItem.MenuItemId);

                    if (existingItem != null)
                    {
                        // Update existing item quantity and instructions
                        existingItem.Quantity = requestItem.Quantity;
                        existingItem.SpecialInstructions = requestItem.SpecialInstructions;
                        existingItem.UpdatedDate = DateTime.UtcNow;
                        processedMenuItemIds.Add(requestItem.MenuItemId);
                    }
                    else
                    {
                        // Add new item
                        order.OrderItems.Add(new OrderItem
                        {
                            OrderId = orderId,
                            MenuItemId = requestItem.MenuItemId,
                            SpecialInstructions = requestItem.SpecialInstructions,
                            Quantity = requestItem.Quantity,
                            UnitPrice = menuItems[requestItem.MenuItemId],
                            Status = 1,
                            CreatedDate = DateTime.UtcNow
                        });
                        processedMenuItemIds.Add(requestItem.MenuItemId);
                    }
                }

                order.UpdatedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}