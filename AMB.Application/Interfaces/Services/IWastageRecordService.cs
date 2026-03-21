using AMB.Application.Dtos;
using System.Threading.Tasks;

namespace AMB.Application.Interfaces.Services
{
    public interface IWastageRecordService
    {
        Task<WastageRecordDto> CreateWastageRecordAsync(CreateWastageRecordDto request);
        // Additional methods (get, update, etc.) can be added here later
    }
}
