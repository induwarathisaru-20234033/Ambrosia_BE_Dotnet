using AMB.Application.Dtos;
using System.Threading.Tasks;

namespace AMB.Application.Interfaces.Services
{
    public interface ICalendarExclusionService
    {
        Task<CalenderExclusionDto> CreateCalenderExclusionAsync(CreateCalenderExclusionRequestDto request);
        Task<PaginatedResultDto<CalenderExclusionDto>> GetPaginatedExclusionsAsync(int pageNumber, int pageSize);
    }
}
