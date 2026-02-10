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
        Task RemoveTableAsync(int id);
        Task<PaginatedResultDto<TableDto>> SearchAsync(SearchTableRequestDto request);
    }
}
