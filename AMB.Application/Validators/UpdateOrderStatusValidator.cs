using AMB.Application.Dtos;
using AMB.Application.Interfaces.Repositories;
using AMB.Domain.Enums;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AMB.Application.Validators
{
    public class UpdateOrderStatusValidator : AbstractValidator<UpdateOrderStatusDto>
    {
        private readonly IServiceProvider _serviceProvider;

        public UpdateOrderStatusValidator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            RuleFor(x => x.OrderId)
                .GreaterThan(0).WithMessage("Invalid order ID")
                .MustAsync(OrderExists).WithMessage("Order not found");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Invalid status value")
                .Must(status => status != OrderStatus.Draft && status != OrderStatus.SentToKDS)
                .WithMessage("Cannot update to Draft or SentToKDS status directly")
                .MustAsync(BeValidStatusTransition).WithMessage("Invalid status transition for this order");

            RuleFor(x => x.Reason)
                .MaximumLength(200).WithMessage("Reason cannot exceed 200 characters")
                .When(x => x.Status == OrderStatus.OnHold || x.Status == OrderStatus.Cancelled)
                .NotEmpty().WithMessage("Reason is required when cancelling or putting order on hold");
        }

        private async Task<bool> OrderExists(int orderId, CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
            var order = await orderRepository.GetByIdAsync(orderId);
            return order != null;
        }

        private async Task<bool> BeValidStatusTransition(UpdateOrderStatusDto dto, OrderStatus newStatus, CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
            var order = await orderRepository.GetByIdAsync(dto.OrderId);

            if (order == null) return false;

            var currentStatus = (OrderStatus)order.OrderStatus;

            // Define valid transitions
            var validTransitions = new Dictionary<OrderStatus, OrderStatus[]>
            {
                [OrderStatus.SentToKDS] = new[] { OrderStatus.Preparing, OrderStatus.OnHold, OrderStatus.Cancelled },
                [OrderStatus.Preparing] = new[] { OrderStatus.OnHold, OrderStatus.Ready, OrderStatus.Cancelled },
                [OrderStatus.OnHold] = new[] { OrderStatus.Preparing, OrderStatus.Ready, OrderStatus.Cancelled },
                [OrderStatus.Ready] = new[] { OrderStatus.Served, OrderStatus.Cancelled },
                [OrderStatus.Served] = Array.Empty<OrderStatus>(),
                [OrderStatus.Cancelled] = Array.Empty<OrderStatus>()
            };

            return validTransitions.ContainsKey(currentStatus) &&
                   validTransitions[currentStatus].Contains(newStatus);
        }
    }
}