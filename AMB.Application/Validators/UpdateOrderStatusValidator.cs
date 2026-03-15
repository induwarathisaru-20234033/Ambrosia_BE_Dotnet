using AMB.Application.Dtos;
using AMB.Application.Interfaces.Repositories;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AMB.Application.Validators
{
    public class UpdateOrderStatusValidator : AbstractValidator<UpdateOrderStatusDto>
    {
        private readonly IServiceProvider _serviceProvider;

        // Valid status values
        private readonly string[] ValidStatuses = new[]
        {
            "Preparing",    // From "Sent to KDS"
            "On Hold",      // From "Preparing" or "Sent to KDS"
            "Ready",        // From "Preparing" or "On Hold"
            "Served",       // From "Ready"
            "Cancelled"     // From any state
        };

        public UpdateOrderStatusValidator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            RuleFor(x => x.OrderId)
                .GreaterThan(0).WithMessage("Invalid order ID")
                .MustAsync(OrderExists).WithMessage("Order not found");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required")
                .Must(status => ValidStatuses.Contains(status))
                .WithMessage($"Status must be one of: {string.Join(", ", ValidStatuses)}")
                .MustAsync(BeValidStatusTransition).WithMessage("Invalid status transition for this order");

            RuleFor(x => x.Reason)
                .MaximumLength(200).WithMessage("Reason cannot exceed 200 characters")
                .When(x => x.Status == "On Hold" || x.Status == "Cancelled")
                .NotEmpty().WithMessage("Reason is required when cancelling or putting order on hold");
        }

        private async Task<bool> OrderExists(int orderId, CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
            var order = await orderRepository.GetByIdAsync(orderId);
            return order != null;
        }

        private async Task<bool> BeValidStatusTransition(UpdateOrderStatusDto dto, string newStatus, CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
            var order = await orderRepository.GetByIdAsync(dto.OrderId);

            if (order == null) return false;

            var currentStatus = order.OrderStatus;

            // Define valid transitions
            var validTransitions = new Dictionary<string, string[]>
            {
                ["Sent to KDS"] = new[] { "Preparing", "On Hold", "Cancelled" },
                ["Preparing"] = new[] { "On Hold", "Ready", "Cancelled" },
                ["On Hold"] = new[] { "Preparing", "Ready", "Cancelled" },
                ["Ready"] = new[] { "Served", "Cancelled" },
                ["Served"] = Array.Empty<string>(),
                ["Cancelled"] = Array.Empty<string>()
            };

            return validTransitions.ContainsKey(currentStatus) &&
                   validTransitions[currentStatus].Contains(newStatus);
        }
    }
}