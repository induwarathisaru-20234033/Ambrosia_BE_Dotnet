using AMB.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMB.Application.Interfaces.Services
{
    public interface ITableService
    {
        Task<TableDto> CreateTableAsync(CreateTableRequestDto request);
        Task SaveTableFloorMapAsync(SaveTableFloorMapRequestDto request);
        Task<GetTableFloorMapResponseDto> GetTableFloorMapAsync();
        Task RemoveTableAsync(int id);
        Task<PaginatedResultDto<TableDto>> SearchAsync(SearchTableRequestDto request);
        Task<List<TableDto>> GetAllAsync();
        Task<List<TableDto>> GetTablesWithAllocationsAsync(DateOnly? date = null);
    }
}
