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
                RuleFor(x => x.TableId)
                    .NotNull().WithMessage("Please select a table")
                    .MustAsync(BeValidTable).WithMessage("Selected table is not available");

                RuleFor(x => x.Items)
                    .NotEmpty().WithMessage("Please add at least one item to the order");

                //RuleForEach(x => x.Items)
                //    .MustAsync(BeAvailableMenuItem).WithMessage("One or more items are unavailable");
            });

            // Always validate items have valid menu items
            RuleForEach(x => x.Items)
                .ChildRules(item =>
                {
                    item.RuleFor(i => i.MenuItemId)
                        .GreaterThan(0).WithMessage("Invalid menu item");

                    item.RuleFor(i => i.Quantity)
                        .GreaterThan(0).WithMessage("Quantity must be at least 1");
                });
        }

        private async Task<bool> BeValidTable(int? tableId, CancellationToken cancellationToken)
        {
            if (!tableId.HasValue) return false;

            using var scope = _serviceProvider.CreateScope();
            var tableRepository = scope.ServiceProvider.GetRequiredService<ITableRepository>();
            var table = await tableRepository.GetByIdAsync(tableId.Value);

            return table != null && table.Status == "Available";
        }

        //private async Task<bool> BeAvailableMenuItem(CreateOrderRequestDto order, OrderItemDto item, CancellationToken cancellationToken)
        //{
        //    using var scope = _serviceProvider.CreateScope();
        //    var menuItemRepository = scope.ServiceProvider.GetRequiredService<IMenuItemRepository>();
        //    return await menuItemRepository.CheckAvailabilityAsync(item.MenuItemId);
        //}
    }
}