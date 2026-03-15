using AMB.Application.Dtos;
using AMB.Application.Interfaces.Repositories;
using AMB.Application.Interfaces.Services;
using AMB.Application.Mappers;
using AMB.Domain.Enums;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AMB.Application.Services
{
    public class TableService : ITableService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ITableRepository _tableRepository;

        public TableService(IServiceProvider serviceProvider, ITableRepository tableRepository)
        {
            _serviceProvider = serviceProvider;
            _tableRepository = tableRepository;
        }

        public async Task<TableDto> CreateTableAsync(CreateTableRequestDto request)
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<CreateTableRequestDto>>();
            await validator.ValidateAndThrowAsync(request);

            var tableModel = request.ToTableEntity();
            tableModel.Status = (int)EntityStatus.Active;
            var table = await _tableRepository.AddAsync(tableModel);

            return table.ToTableDto();
        }

        public async Task RemoveTableAsync(int id)
        {
            await _tableRepository.UpdateStatusAsync(id, EntityStatus.Inactive);
        }

        public async Task<PaginatedResultDto<TableDto>> SearchAsync(SearchTableRequestDto request)
        {
            var query = _tableRepository.GetSearchQuery();

            if (request.Status.HasValue && (request.Status.Value == EntityStatus.Active || request.Status.Value == EntityStatus.Inactive))
            {
                query = query.Where(table => table.Status == (int)request.Status.Value);
            }

            var totalItemCount = await query.CountAsync();

            var pageCount = request.PageSize == 0 ? 0 : (int)Math.Ceiling(totalItemCount / (double)request.PageSize);

            if (request.PageSize > 0)
            {
                query = query
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize);
            }

            var items = await query.ToListAsync();

            var mappedItems = items.Select(item => item.ToTableDto()).ToList();

            return new PaginatedResultDto<TableDto>
            {
                PageCount = pageCount,
                PageSize = request.PageSize,
                PageNumber = request.PageNumber,
                Items = mappedItems,
                TotalItemCount = totalItemCount
            };
        }

        public async Task<List<TableDto>> GetAllAsync()
        {
            var allTables = await _tableRepository.GetAllAsync();

            var dtoModels = new List<TableDto>();

            foreach (var item in allTables)
            {
                var dtoModel = item.ToTableDto();
                dtoModels.Add(dtoModel);
            }

            return dtoModels;
        }

        public async Task<List<TableDto>> GetTablesWithAllocationsAsync(DateOnly? date = null)
        {
            var allTables = await _tableRepository.GetAllAsync();
            var reservationRepository = _serviceProvider.GetRequiredService<IReservationRepository>();

            var tableDtos = new List<TableDto>();

            foreach (var table in allTables)
            {
                var allocationCount = 0;
                if (date.HasValue)
                {
                    // Count allocations for specific date
                    allocationCount = (await reservationRepository.GetTableReservationsByDateAsync(table.Id, date.Value)).Count;
                }

                tableDtos.Add(new TableDto
                {
                    Id = table.Id,
                    TableName = table.TableName,
                    Capacity = table.Capacity,
                    IsOnlineBookingEnabled = table.IsOnlineBookingEnabled,
                    ExistingAllocations = allocationCount
                });
            }

            return tableDtos;
        }
    }
}
