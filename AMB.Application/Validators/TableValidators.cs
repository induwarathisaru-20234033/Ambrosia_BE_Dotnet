using AMB.Application.Dtos;
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
}
