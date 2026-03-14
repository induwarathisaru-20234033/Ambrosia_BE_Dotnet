using AMB.Application.Dtos;
using FluentValidation;

namespace AMB.Application.Validators
{
    public class UpdateInventoryItemValidator : AbstractValidator<UpdateInventoryItemRequestDto>
    {
        public UpdateInventoryItemValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Valid Inventory Item Id is required");

            RuleFor(x => x.ItemNumber)
                .NotEmpty().WithMessage("Item Number is required")
                .MaximumLength(50).WithMessage("Item Number must be 50 characters or less");

            RuleFor(x => x.ItemName)
                .NotEmpty().WithMessage("Item Name is required")
                .MaximumLength(150).WithMessage("Item Name must be 150 characters or less");

            RuleFor(x => x.OpeningQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Opening Quantity cannot be negative");

            RuleFor(x => x.ItemType)
                .NotEmpty().WithMessage("Item Type is required")
                .MaximumLength(100).WithMessage("Item Type must be 100 characters or less");

            RuleFor(x => x.ItemCategory)
                .NotEmpty().WithMessage("Item Category is required")
                .MaximumLength(100).WithMessage("Item Category must be 100 characters or less");

            RuleFor(x => x.UoM)
                .NotEmpty().WithMessage("UoM is required")
                .MaximumLength(20).WithMessage("UoM must be 20 characters or less");

            RuleFor(x => x.UnitPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Unit Price cannot be negative");

            RuleFor(x => x.Currency)
                .NotEmpty().WithMessage("Currency is required")
                .Length(3).WithMessage("Currency must be a 3 letter ISO code");

            RuleFor(x => x.MinimumStockLevel)
                .GreaterThanOrEqualTo(0).WithMessage("Minimum Stock Level cannot be negative");

            RuleFor(x => x.MaximumStockLevel)
                .GreaterThanOrEqualTo(0).WithMessage("Maximum Stock Level cannot be negative")
                .GreaterThanOrEqualTo(x => x.MinimumStockLevel).WithMessage("Maximum Stock Level must be greater than or equal to Minimum Stock Level");

            RuleFor(x => x.ReOrderLevel)
                .GreaterThanOrEqualTo(0).WithMessage("ReOrder Level cannot be negative");

            RuleFor(x => x.ShelveLife)
                .GreaterThanOrEqualTo(0).WithMessage("Shelve Life cannot be negative");

            RuleFor(x => x.StorageLocation)
                .MaximumLength(150).WithMessage("Storage Location must be 150 characters or less");

            RuleFor(x => x.StorageConditions)
                .MaximumLength(250).WithMessage("Storage Conditions must be 250 characters or less");

            RuleFor(x => x.Sku)
                .MaximumLength(100).WithMessage("SKU must be 100 characters or less");

            RuleFor(x => x.ExpiryDate)
                .GreaterThan(DateTimeOffset.UtcNow).When(x => x.ExpiryDate != default)
                .WithMessage("Expiry Date must be in the future");
        }
    }
}
