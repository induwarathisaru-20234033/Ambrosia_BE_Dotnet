using AMB.Application.Dtos;
using AMB.Application.Interfaces.Repositories;
using AMB.Application.Interfaces.Services;
using AMB.Application.Mappers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AMB.Application.Services
{
    public class CalenderExclusionService: ICalendarExclusionService
    {
        private readonly ICalenderExclusionRepository _repository;

        public CalenderExclusionService(ICalenderExclusionRepository repository)
        {
            _repository = repository; 
        }

        public async Task<PaginatedResultDto<CalenderExclusionDto>> GetPaginatedExclusionsAsync(int pageNumber, int pageSize)
        {
            var allItems = await _repository.GetAllAsync();
            var totalItemCount = allItems.Count;
            var pageCount = totalItemCount == 0
                ? 0
                : (int)Math.Ceiling(totalItemCount / (double)pageSize);

            var items = allItems
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(item => item.ToCalenderExclusionDto())
                .ToList();

            return new PaginatedResultDto<CalenderExclusionDto>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                PageCount = pageCount,
                Items = items,
                TotalItemCount = totalItemCount
            };
        }


    }
}
