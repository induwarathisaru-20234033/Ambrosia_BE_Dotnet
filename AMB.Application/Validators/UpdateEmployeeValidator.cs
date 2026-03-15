using AMB.Application.Dtos;
using FluentValidation;

namespace AMB.Application.Validators
{
    public class UpdateEmployeeValidator : AbstractValidator<UpdateEmployeeRequestDto>
    {
        public UpdateEmployeeValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Valid Employee Id is required");

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

            RuleFor(x => x.Password)
                .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Password));
        }
    }
}
