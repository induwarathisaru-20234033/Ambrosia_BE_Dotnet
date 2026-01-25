using AMB.Application.Dtos;
using FluentValidation;

namespace AMB.Application.Validators
{
    public class CreateEmployeeValidator: AbstractValidator<CreateEmployeeRequestDto>
    {
        public CreateEmployeeValidator()
        {
            RuleFor(x => x.EmployeeId)
                .NotEmpty().WithMessage("Employee Id is required");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First Name is required")
                .MaximumLength(50);

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last Name is required")
                .MaximumLength(50);

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("A valid email is required.")
                .When(x => !string.IsNullOrEmpty(x.Email));

            RuleFor(x => x.Username)
                .EmailAddress().WithMessage("A valid email is required.")
                .NotEmpty().WithMessage("Username is required");

            RuleFor(x => x.MobileNumber)
                .NotEmpty().WithMessage("Mobile Number is required")
                .Matches(@"^\d{10}$").WithMessage("Mobile number must be 10 digits.");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address is required")
                .MaximumLength(200);
        }
    }
}
