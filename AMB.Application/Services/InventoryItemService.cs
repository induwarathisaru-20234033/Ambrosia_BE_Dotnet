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
    public class InventoryItemService : IInventoryItemService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IInventoryItemRepository _inventoryItemRepository;

        public InventoryItemService(IServiceProvider serviceProvider, IInventoryItemRepository inventoryItemRepository)
        {
            _serviceProvider = serviceProvider;
            _inventoryItemRepository = inventoryItemRepository;
        }

        public async Task<InventoryItemDto> CreateInventoryItemAsync(CreateInventoryItemRequestDto request)
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<CreateInventoryItemRequestDto>>();
            await validator.ValidateAndThrowAsync(request);

            var inventoryItemModel = request.ToInventoryItemEntity();
            inventoryItemModel.Status = (int)EntityStatus.Active;
            inventoryItemModel.InventoryStatus = GetInventoryStatus(
                request.OpeningQuantity,
                request.MinimumStockLevel,
                request.MaximumStockLevel);
            inventoryItemModel.RemainingQuantity = request.OpeningQuantity;
            
            var inventoryItem = await _inventoryItemRepository.AddAsync(inventoryItemModel);

            return inventoryItem.ToInventoryItemDto();
        }

        public async Task<InventoryItemDto> UpdateInventoryItemAsync(UpdateInventoryItemRequestDto request)
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<UpdateInventoryItemRequestDto>>();
            await validator.ValidateAndThrowAsync(request);

            var inventoryItem = await _inventoryItemRepository.GetByIdAsync(request.Id);
            if (inventoryItem == null)
            {
                throw new KeyNotFoundException($"Inventory item with ID {request.Id} not found.");
            }

            inventoryItem.ApplyUpdates(request);
            inventoryItem.InventoryStatus = GetInventoryStatus(
                request.OpeningQuantity,
                request.MinimumStockLevel,
                request.MaximumStockLevel);
            

            var updatedInventoryItem = await _inventoryItemRepository.UpdateAsync(inventoryItem);
            if (updatedInventoryItem == null)
            {
                throw new KeyNotFoundException($"Inventory item with ID {request.Id} not found.");
            }

            return updatedInventoryItem.ToInventoryItemDto();
        }

        public async Task<InventoryItemDto> GetInventoryItemByIdAsync(int id)
        {
            var inventoryItem = await _inventoryItemRepository.GetByIdAsync(id);
            if (inventoryItem == null)
            {
                throw new KeyNotFoundException($"Inventory item with ID {id} not found.");
            }

            return inventoryItem.ToInventoryItemDto();
        }

        public async Task<PaginatedResultDto<InventoryItemDto>> SearchAsync(InventoryItemFilterRequestDto request)
        {
            var query = _inventoryItemRepository.GetSearchQuery();

            if (!string.IsNullOrWhiteSpace(request.ItemNumber))
            {
                query = query.Where(item => item.ItemNumber.Contains(request.ItemNumber));
            }

            if (!string.IsNullOrWhiteSpace(request.ItemName))
            {
                query = query.Where(item => item.ItemName.Contains(request.ItemName));
            }

            if (!string.IsNullOrWhiteSpace(request.ItemType))
            {
                query = query.Where(item => item.ItemType.Contains(request.ItemType));
            }

            if (!string.IsNullOrWhiteSpace(request.ItemCategory))
            {
                query = query.Where(item => item.ItemCategory.Contains(request.ItemCategory));
            }

            if (!string.IsNullOrWhiteSpace(request.UoM))
            {
                query = query.Where(item => item.UoM.Contains(request.UoM));
            }

            if (request.Status.HasValue)
            {
                query = query.Where(item => item.Status == (int)request.Status.Value);
            }

            if (request.InventoryStatus.HasValue)
            {
                query = query.Where(item => item.InventoryStatus == (int)request.InventoryStatus.Value);
            }

            var totalItemCount = await query.CountAsync();

            var pageCount = request.PageSize == 0
                ? 0
                : (int)Math.Ceiling(totalItemCount / (double)request.PageSize);

            if (request.PageSize > 0)
            {
                query = query
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize);
            }

            var items = await query.ToListAsync();

            return new PaginatedResultDto<InventoryItemDto>
            {
                PageCount = pageCount,
                PageSize = request.PageSize,
                PageNumber = request.PageNumber,
                TotalItemCount = totalItemCount,
                Items = items.Select(item => item.ToInventoryItemDto()).ToList(),
            };
        }

        public async Task<List<UnitOfMeasureDto>> GetAllUoMsAsync()
        {
            var uoms = await _inventoryItemRepository.GetAllUoMsAsync();

            return uoms
                .Select(uom => new UnitOfMeasureDto
                {
                    Id = uom.Id,
                    UoM = uom.UoM,
                    Description = uom.Description,
                })
                .ToList();
        }

        public async Task<List<CurrencyDto>> GetAllCurrenciesAsync()
        {
            var currencies = await _inventoryItemRepository.GetAllCurrenciesAsync();

            return currencies
                .Select(currency => new CurrencyDto
                {
                    Id = currency.Id,
                    CurrencyCode = currency.CurrencyCode,
                    Description = currency.Description,
                })
                .ToList();
        }

        private static int GetInventoryStatus(float quantity, float minimumStockLevel, float maximumStockLevel)
        {
            if (quantity < minimumStockLevel)
            {
                return (int)InventoryStatus.LowStock;
            }

            if (quantity > maximumStockLevel)
            {
                return (int)InventoryStatus.HighStock;
            }

            return (int)InventoryStatus.InStock;
        }
    }
}
