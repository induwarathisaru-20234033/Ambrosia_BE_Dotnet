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

        public async Task<CalenderExclusionDto> CreateCalenderExclusionAsync(CreateCalenderExclusionRequestDto request)
        {
            var exclusion = request.ToCalenderExclusionEntity();
            var created = await _repository.AddAsync(exclusion);

            return created.ToCalenderExclusionDto();
        }

        public async Task<PaginatedResultDto<CalenderExclusionDto>> GetPaginatedExclusionsAsync(int pageNumber, int pageSize)
        {
            var allItems = await _repository.GetAllAsync();
            var totalItemCount = allItems.Count;
            int pageCount;
            var items = allItems;

            if (pageSize == 0)
            {
                pageCount = totalItemCount == 0 ? 0 : 1;
            }
            else
            {
                pageCount = totalItemCount == 0
                    ? 0
                    : (int)Math.Ceiling(totalItemCount / (double)pageSize);

                items = allItems
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
            }

            var mappedItems = items
                .Select(item => item.ToCalenderExclusionDto())
                .ToList();

            return new PaginatedResultDto<CalenderExclusionDto>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                PageCount = pageCount,
                Items = mappedItems,
                TotalItemCount = totalItemCount
            };
        }


    }
}
