using AMB.Application.Dtos;
using AMB.Application.Interfaces.Repositories;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AMB.Application.Validators
{
    public class CreateRoleRequestValidator : AbstractValidator<CreateRoleRequestDto>
    {
        private readonly IServiceProvider _serviceProvider;

        public CreateRoleRequestValidator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            RuleFor(x => x.RoleCode)
                .NotEmpty().WithMessage("Role Code is required")
                .Length(3, 50).WithMessage("Role Code must be 3-50 characters")
                .Matches(@"^[A-Z0-9_]+$").WithMessage("Only uppercase letters, numbers, and underscores allowed")
                .MustAsync(BeUniqueRoleCode).WithMessage("Role Code already exists. Please choose a different code.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Role Name is required")
                .Length(2, 100).WithMessage("Role Name must be 2-100 characters");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

            RuleFor(x => x.PermissionIds)
                .NotEmpty().WithMessage("Please select at least one permission for this role");
        }

        private async Task<bool> BeUniqueRoleCode(string roleCode, CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var roleRepository = scope.ServiceProvider.GetRequiredService<IRoleRepository>();
            return await roleRepository.IsRoleCodeUniqueAsync(roleCode.ToUpper());
        }
    }
}