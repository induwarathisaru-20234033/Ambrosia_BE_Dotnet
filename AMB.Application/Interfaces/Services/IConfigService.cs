using AMB.Application.Dtos;
using System.Threading.Tasks;

namespace AMB.Application.Interfaces.Services
{
    public interface IConfigService
    {
        Task AddConfigurationsAsync(CreateServiceRulesRequestDto request);
        Task UpdateConfigurationsAsync(UpdateServiceRulesRequestDto request);
        Task<ServiceRulesResponseDto?> GetConfigurationsAsync();
    }
}
