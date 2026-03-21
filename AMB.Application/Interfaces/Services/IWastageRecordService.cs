using AMB.Application.Dtos;
using System.Threading.Tasks;

namespace AMB.Application.Interfaces.Services
{
    public interface IWastageRecordService
    {
        Task<WastageRecordDto> CreateWastageRecordAsync(CreateWastageRecordDto request);
        Task<WastageRecordDto> UpdateWastageRecordAsync(UpdateWastageRecordDto request);
        // Additional methods (get, update, etc.) can be added here later
    }
}
