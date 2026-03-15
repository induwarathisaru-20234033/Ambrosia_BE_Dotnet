using AMB.Application.Dtos;
using AMB.Application.Interfaces.Repositories;
using AMB.Application.Interfaces.Services;
using AMB.Domain.Entities;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AMB.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMenuItemRepository _menuItemRepository;
        private readonly ITableRepository _tableRepository;
        private readonly IServiceProvider _serviceProvider;

        public OrderService(
            IOrderRepository orderRepository,
            IMenuItemRepository menuItemRepository,
            ITableRepository tableRepository,
            IServiceProvider serviceProvider)
        {
            _orderRepository = orderRepository;
            _menuItemRepository = menuItemRepository;
            _tableRepository = tableRepository;
            _serviceProvider = serviceProvider;
        }

        public async Task<OrderResponseDto> CreateOrderAsync(CreateOrderRequestDto request)
        {
            // Validate using FluentValidation
            var validator = _serviceProvider.GetRequiredService<IValidator<CreateOrderRequestDto>>();
            await validator.ValidateAndThrowAsync(request);

            // Get menu items for price and validation
            var menuItemIds = request.Items.Select(i => i.MenuItemId).ToList();
            var menuItems = await _menuItemRepository.GetByIdsAsync(menuItemIds);

            // Check if all items exist
            if (menuItems.Count != menuItemIds.Count)
            {
                var foundIds = menuItems.Select(m => m.Id).ToList();
                var missingIds = menuItemIds.Except(foundIds).ToList();
                throw new InvalidOperationException($"Menu items not found: {string.Join(", ", missingIds)}");
            }

            // Check availability for fired orders
            if (!request.IsDraft)
            {
                var unavailableItems = menuItems.Where(m => !m.IsAvailable).ToList();
                if (unavailableItems.Any())
                {
                    throw new InvalidOperationException(
                        $"Some items are unavailable: {string.Join(", ", unavailableItems.Select(m => m.Name))}");
                }
            }

            // Generate order number
            var orderNumber = await _orderRepository.GenerateOrderNumberAsync();

            // Create order entity 
            var order = new Order
            {
                OrderNumber = orderNumber,
                TableId = request.TableId,
                OrderStatus = request.IsDraft ? "Draft" : "Sent to KDS",
                SentToKitchenAt = request.IsDraft ? null : DateTime.UtcNow,
                Status = 1, 
                OrderItems = request.Items.Select(item =>
                {
                    var menuItem = menuItems.First(m => m.Id == item.MenuItemId);
                    return new OrderItem
                    {
                        MenuItemId = item.MenuItemId,
                        SpecialInstructions = item.SpecialInstructions,
                        Quantity = item.Quantity,
                        UnitPrice = menuItem.Price,
                        Status = 1, 
                    };
                }).ToList()
            };

            // Save to database
            var createdOrder = await _orderRepository.AddAsync(order);

            // Return response with complete data
            return await GetOrderByIdAsync(createdOrder.Id);
        }

        public async Task<OrderResponseDto> GetOrderByIdAsync(int id)
        {
            var options = new OrderQueryOptions
            {
                IncludeItems = true,
                IncludeMenuItem = true,
                IncludeTable = true
            };

            var order = await _orderRepository.GetByIdAsync(id, options);
            if (order == null || order.Status != 1) // Check if active
            {
                throw new KeyNotFoundException($"Order with ID {id} not found");
            }

            return new OrderResponseDto
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                TableId = order.TableId,
                TableName = order.Table?.TableName,
                OrderStatus = order.OrderStatus,
                CreatedDate = order.CreatedDate,
                UpdatedDate = order.UpdatedDate,
                Items = order.OrderItems?
                    .Where(oi => oi.Status == 1) // Only active items
                    .Select(oi => new OrderItemResponseDto
                    {
                        Id = oi.Id,
                        MenuItemId = oi.MenuItemId,
                        MenuItemName = oi.MenuItem?.Name ?? string.Empty,
                        SpecialInstructions = oi.SpecialInstructions,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice,
                        IsAvailable = oi.MenuItem?.IsAvailable ?? false,
                        CreatedDate = oi.CreatedDate
                    }).ToList() ?? new()
            };
        }


        public async Task<List<MenuItemDto>> SearchMenuItemsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return new List<MenuItemDto>();

            var query = _menuItemRepository.GetQuery()
                .Where(m => m.Name.Contains(searchTerm) && m.IsAvailable)
                .OrderBy(m => m.Name)
                .Take(20);

            var menuItems = await Task.Run(() => query.ToList());

            return menuItems.Select(m => new MenuItemDto
            {
                Id = m.Id,
                Name = m.Name,
                Price = m.Price,
                Category = m.Category,
                IsAvailable = m.IsAvailable
            }).ToList();
        }
    }
}