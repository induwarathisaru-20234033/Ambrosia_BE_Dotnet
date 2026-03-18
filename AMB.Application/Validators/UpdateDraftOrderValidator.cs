using AMB.Application.Dtos;
using AMB.Application.Interfaces.Repositories;
using AMB.Domain.Enums;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace AMB.Application.Validators
{
    public class UpdateDraftOrderValidator : AbstractValidator<UpdateDraftOrderDto>
    {
        private readonly IServiceProvider _serviceProvider;

        
    }
}