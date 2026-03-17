using AMB.Application.Dtos;
using AMB.Application.Interfaces.Repositories;
using AMB.Domain.Enums;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AMB.Application.Validators
{
    public class SendOrderToKdsValidator : AbstractValidator<SendOrderToKdsDto>
    {
        private readonly IServiceProvider _serviceProvider;

        public SendOrderToKdsValidator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            RuleFor(x => x.OrderId)
                .GreaterThan(0).WithMessage("Invalid order ID")
                .MustAsync(BeDraftOrder).WithMessage("Order must be in Draft status to send to KDS");

            RuleFor(x => x.OrderId)
                .MustAsync(HaveItems).WithMessage("Order must have at least one item");
        }

        private async Task<bool> BeDraftOrder(int orderId, CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
            var order = await orderRepository.GetByIdAsync(orderId);

            return order != null && (OrderStatus)order.OrderStatus == OrderStatus.Draft;
        }

        private async Task<bool> HaveItems(int orderId, CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
            var options = new OrderQueryOptions { IncludeItems = true };
            var order = await orderRepository.GetByIdAsync(orderId, options);

            return order != null && order.OrderItems != null && order.OrderItems.Any();
        }
    }
}