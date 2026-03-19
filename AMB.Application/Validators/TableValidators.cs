using AMB.Application.Dtos;
using AMB.Domain.Enums;
using FluentValidation;

namespace AMB.Application.Validators
{
    public class CreateTableValidator : AbstractValidator<CreateTableRequestDto>
    {
        public CreateTableValidator()
        {
            RuleFor(x => x.TableName)
                .NotEmpty()
                .WithMessage("Table Name is Required");
            RuleFor(x => x.TableName)
                .MaximumLength(20)
                .MinimumLength(1)
                .WithMessage("Table Name must be 1-20 characters");


            RuleFor(x => x.Capacity)
                .NotEmpty()
                .WithMessage("Table Capacity is Required");
            RuleFor(x => x.Capacity)
                .GreaterThan(0)
                .WithMessage("Table Capacity must be greater than zero");

        }
    }

    public class SaveTableFloorMapValidator : AbstractValidator<SaveTableFloorMapRequestDto>
    {
        public SaveTableFloorMapValidator()
        {
            RuleFor(x => x.Shapes)
                .NotNull()
                .WithMessage("Shapes collection is required.");

            RuleForEach(x => x.Shapes)
                .SetValidator(new TableFloorMapShapeValidator());
        }
    }

    public class TableFloorMapShapeValidator : AbstractValidator<TableFloorMapShapeRequestDto>
    {
        public TableFloorMapShapeValidator()
        {
            RuleFor(x => x.Type)
                .Must(value => Enum.IsDefined(typeof(ShapeType), value))
                .WithMessage("Invalid shape type.");

            RuleFor(x => x.Width)
                .GreaterThan(0)
                .WithMessage("Width must be greater than zero.");

            RuleFor(x => x.Height)
                .GreaterThan(0)
                .WithMessage("Height must be greater than zero.");

            RuleFor(x => x.Fill)
                .NotEmpty()
                .WithMessage("Fill is required.");
        }
    }
}
