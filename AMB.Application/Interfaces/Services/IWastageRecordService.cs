using AMB.Application.Dtos;
using System.Threading.Tasks;

namespace AMB.Application.Interfaces.Services
{
    public interface IWastageRecordService
    {
        Task<WastageRecordDto> CreateWastageRecordAsync(CreateWastageRecordDto request);
        Task<WastageRecordDto> UpdateWastageRecordAsync(UpdateWastageRecordDto request);
        Task<IEnumerable<WastageRecordDto>> GetAllWastageRecordsAsync();
        Task<WastageRecordDto> GetWastageRecordByIdAsync(int id);
    }
}
