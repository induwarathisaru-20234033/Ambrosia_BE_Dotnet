using AMB.Application.Dtos;
using AMB.Application.Interfaces.Repositories;
using AMB.Application.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;


namespace AMB.Application.Validators
{
    public class EditRoleRequestValidator : AbstractValidator<EditRoleRequestDto>
    {
        private readonly IServiceProvider _serviceProvider;

        public EditRoleRequestValidator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Invalid role ID");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Role Name is required")
                .Length(2, 100).WithMessage("Role Name must be 2-100 characters");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

            RuleFor(x => x.Status)
                .Must(x => x == "ENABLED" || x == "DISABLED")
                .WithMessage("Status must be either ENABLED or DISABLED");

            RuleFor(x => x.PermissionIds)
                .NotEmpty().WithMessage("Please select at least one permission for this role");

            RuleFor(x => x.RoleCode)
                .MustAsync(BeUniqueRoleCodeForUpdate)
                .When(x => !string.IsNullOrEmpty(x.RoleCode))
                .WithMessage("Role Code already exists. Please choose a different code.");
        }

        private async Task<bool> BeUniqueRoleCodeForUpdate(EditRoleRequestDto request, string roleCode, CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var roleRepository = scope.ServiceProvider.GetRequiredService<IRoleRepository>();
            return await roleRepository.IsRoleCodeUniqueForUpdateAsync(roleCode.ToUpper(), request.Id);
        }
    }
}
