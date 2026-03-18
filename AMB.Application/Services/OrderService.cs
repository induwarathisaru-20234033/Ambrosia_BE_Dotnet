using AMB.Application.Dtos;
using AMB.Application.Interfaces.Repositories;
using AMB.Application.Interfaces.Services;
using AMB.Domain.Entities;
using AMB.Domain.Enums;
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
            var validator = _serviceProvider.GetRequiredService<IValidator<CreateOrderRequestDto>>();
            await validator.ValidateAndThrowAsync(request);

            var menuItemIds = request.Items.Select(i => i.MenuItemId).ToList();
            var menuItems = await _menuItemRepository.GetByIdsAsync(menuItemIds);

            if (menuItems.Count != menuItemIds.Count)
            {
                var foundIds = menuItems.Select(m => m.Id).ToList();
                var missingIds = menuItemIds.Except(foundIds).ToList();
                throw new InvalidOperationException($"Menu items not found: {string.Join(", ", missingIds)}");
            }

            if (!request.IsDraft)
            {
                var unavailableItems = menuItems.Where(m => !m.IsAvailable).ToList();
                if (unavailableItems.Any())
                {
                    throw new InvalidOperationException(
                        $"Some items are unavailable: {string.Join(", ", unavailableItems.Select(m => m.Name))}");
                }
            }

            var orderNumber = await _orderRepository.GenerateOrderNumberAsync();

            var order = new Order
            {
                OrderNumber = orderNumber,
                TableId = request.TableId,
                OrderStatus = request.IsDraft
                    ? (int)AMB.Domain.Enums.OrderStatus.Draft
                    : (int)AMB.Domain.Enums.OrderStatus.SentToKDS,
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

            var createdOrder = await _orderRepository.AddAsync(order);

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
            if (order == null || order.Status != 1)
            {
                throw new KeyNotFoundException($"Order with ID {id} not found");
            }

            return new OrderResponseDto
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                TableId = order.TableId,
                TableName = order.Table?.TableName,
                OrderStatus = (OrderStatus)order.OrderStatus,
                CreatedDate = order.CreatedDate,
                UpdatedDate = order.UpdatedDate,
                Items = order.OrderItems?
                    .Where(oi => oi.Status == 1)
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

        public async Task<OrderResponseDto> SendDraftToKdsAsync(SendOrderToKdsDto dto)
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<SendOrderToKdsDto>>();
            await validator.ValidateAndThrowAsync(dto);

            if (dto.TableId.HasValue)
            {
                var table = await _tableRepository.GetByIdAsync(dto.TableId.Value);
                if (table == null || table.Status != 1)
                {
                    throw new InvalidOperationException("Selected table is not available");
                }
            }

            var sent = await _orderRepository.SendDraftToKdsAsync(dto.OrderId, dto.TableId);
            if (!sent)
            {
                throw new InvalidOperationException("Failed to send order to KDS");
            }

            return await GetOrderByIdAsync(dto.OrderId);
        }

        public async Task<OrderResponseDto> UpdateOrderStatusAsync(UpdateOrderStatusDto dto)
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<UpdateOrderStatusDto>>();
            await validator.ValidateAndThrowAsync(dto);

            var updated = await _orderRepository.UpdateOrderStatusAsync(dto.OrderId, dto.Status, dto.Reason);
            if (!updated)
            {
                throw new InvalidOperationException("Failed to update order status");
            }

            return await GetOrderByIdAsync(dto.OrderId);
        }

        public async Task<List<OrderResponseDto>> GetOrdersByStatusAsync(OrderStatus status)
        {
            var orders = await _orderRepository.GetOrdersByStatusAsync(status);
            return orders.Select(o => MapToOrderResponseDto(o)).ToList();
        }

        public async Task<List<OrderResponseDto>> GetKitchenOrdersAsync()
        {
            var orders = await _orderRepository.GetKitchenOrdersAsync();
            return orders.Select(o => MapToOrderResponseDto(o)).ToList();
        }

        // search / filter / sort / paginate orders for FE DataTable
        public async Task<PagedResponseDto<OrderResponseDto>> SearchOrdersAsync(SearchOrderRequestDto request)
        {
            var pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
            var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;

            var (orders, totalCount) = await _orderRepository.SearchOrdersAsync(request);

            var mappedOrders = orders.Select(o => MapToOrderResponseDto(o)).ToList();

            var pageCount = (int)Math.Ceiling((double)totalCount / pageSize);

            return new PagedResponseDto<OrderResponseDto>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                PageCount = pageCount,
                TotalItemCount = totalCount,
                Items = mappedOrders
            };
        }

        private OrderResponseDto MapToOrderResponseDto(Order order)
        {
            return new OrderResponseDto
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                TableId = order.TableId,
                TableName = order.Table?.TableName,
                OrderStatus = (OrderStatus)order.OrderStatus,
                CreatedDate = order.CreatedDate,
                UpdatedDate = order.UpdatedDate,
                Items = order.OrderItems?
                    .Where(oi => oi.Status == 1)
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
    }
}