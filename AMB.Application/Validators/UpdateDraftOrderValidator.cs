using AMB.Application.Dtos;
using AMB.Application.Interfaces.Repositories;
using AMB.Domain.Enums;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AMB.Application.Validators
{
    public class UpdateDraftOrderValidator : AbstractValidator<UpdateOrderRequestDto>
    {
        private readonly IServiceProvider _serviceProvider;

        public UpdateDraftOrderValidator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            RuleFor(x => x.OrderId)
                .GreaterThan(0).WithMessage("Invalid order ID")
                .MustAsync(BeDraftOrder).WithMessage("Order must be in Draft status to update");

            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("Order must have at least one item");

            RuleForEach(x => x.Items)
                .ChildRules(item =>
                {
                    item.RuleFor(i => i.MenuItemId)
                        .GreaterThan(0).WithMessage("Invalid menu item");

                    item.RuleFor(i => i.Quantity)
                        .GreaterThan(0).WithMessage("Quantity must be greater than 0");
                });

            // Warn if all items have quantity 0 (would remove everything)
            RuleFor(x => x.Items)
                .Must(items => items.Any(i => i.Quantity > 0))
                .WithMessage("At least one item must have quantity greater than 0")
                .When(x => x.Items.Any()); // Only check if there are items
        }

        private async Task<bool> BeDraftOrder(int orderId, CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
            var order = await orderRepository.GetByIdAsync(orderId);

            return order != null && (OrderStatus)order.OrderStatus == OrderStatus.Draft;
        }

        private bool HaveUniqueMenuItems(List<OrderItemDto> items)
        {
            var duplicates = items
                .GroupBy(x => x.MenuItemId)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            return !duplicates.Any();
        }
    }
}