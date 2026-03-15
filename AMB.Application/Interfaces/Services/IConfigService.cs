using AMB.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AMB.Application.Interfaces.Services
{
    public interface IConfigService
    {
        Task AddConfigurationsAsync(CreateServiceRulesRequestDto request);
        Task UpdateConfigurationsAsync(UpdateServiceRulesRequestDto request);
        Task<ServiceRulesResponseDto?> GetConfigurationsAsync();
        Task<List<BookingSlotDto>> GetBookingSlotsWithAllocationsAsync(DateTimeOffset? dateTime = null);
    }
}
