using AMB.Application.Dtos;
using AMB.Application.Interfaces.Repositories;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AMB.Application.Validators
{
    public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequestDto>
    {
        private readonly IServiceProvider _serviceProvider;

        public CreateOrderRequestValidator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            // When firing order (not draft), validate table and items
            When(x => !x.IsDraft, () =>
            {

                RuleFor(x => x.Items)
                    .NotEmpty().WithMessage("Please add at least one item to the order");

                RuleForEach(x => x.Items)
                    .MustAsync(BeAvailableMenuItem)
                    .WithMessage("One or more items are unavailable");
            });

            // Always validate items
            RuleForEach(x => x.Items)
                .ChildRules(item =>
                {
                    item.RuleFor(i => i.MenuItemId)
                        .GreaterThan(0).WithMessage("Invalid menu item");

                    item.RuleFor(i => i.Quantity)
                        .GreaterThan(0).WithMessage("Quantity must be at least 1");
                });

            // No duplicate menu items
            RuleFor(x => x.Items)
                .Must(HaveUniqueMenuItems)
                .WithMessage("Duplicate menu items are not allowed. Please combine quantities.");
        }

        private async Task<bool> BeValidAndActiveTable(int? tableId, CancellationToken cancellationToken)
        {
            if (!tableId.HasValue) return false;

            using var scope = _serviceProvider.CreateScope();
            var tableRepository = scope.ServiceProvider.GetRequiredService<ITableRepository>();
            var table = await tableRepository.GetByIdAsync(tableId.Value);

            // Check if table exists and is active (Status = 1 from BaseEntity)
            return table != null && table.Status == 1;
        }

        private async Task<bool> BeAvailableMenuItem(CreateOrderRequestDto order, OrderItemDto item, CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var menuItemRepository = scope.ServiceProvider.GetRequiredService<IMenuItemRepository>();
            var menuItem = await menuItemRepository.GetByIdAsync(item.MenuItemId);

            // For fired orders, check availability
            if (!order.IsDraft)
            {
                return menuItem != null && menuItem.IsAvailable;
            }

            // For draft orders, just check existence
            return menuItem != null;
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