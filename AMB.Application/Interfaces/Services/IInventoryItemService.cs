using AMB.Application.Dtos;

namespace AMB.Application.Interfaces.Services
{
    public interface IInventoryItemService
    {
        Task<InventoryItemDto> CreateInventoryItemAsync(CreateInventoryItemRequestDto request);
        Task<InventoryItemDto> UpdateInventoryItemAsync(UpdateInventoryItemRequestDto request);
        Task<InventoryItemDto> GetInventoryItemByIdAsync(int id);
        Task<PaginatedResultDto<InventoryItemDto>> SearchAsync(InventoryItemFilterRequestDto request);
        Task<List<UnitOfMeasureDto>> GetAllUoMsAsync();
        Task<List<CurrencyDto>> GetAllCurrenciesAsync();
    }
}
