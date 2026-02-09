using AMB.Application.Dtos;
using System.Threading.Tasks;

namespace AMB.Application.Interfaces.Services
{
    public interface ICalendarExclusionService
    {
        Task<PaginatedResultDto<CalenderExclusionDto>> GetPaginatedExclusionsAsync(int pageNumber, int pageSize);
    }
}
