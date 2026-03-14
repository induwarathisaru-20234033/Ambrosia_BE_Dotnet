using AMB.Application.Interfaces.Repositories;
using AMB.Domain.Entities;
using AMB.Domain.Enums;
using AMB.Infra.DBContexts;
using Microsoft.EntityFrameworkCore;

namespace AMB.Infra.Repositories
{
    public class InventoryItemRepository : IInventoryItemRepository
    {
        private readonly AMBContext _context;

        public InventoryItemRepository(AMBContext context)
        {
            _context = context;
        }

        public async Task<InventoryItem> AddAsync(InventoryItem inventoryItem)
        {
            _context.InventoryItems.Add(inventoryItem);
            await _context.SaveChangesAsync();
            return inventoryItem;
        }

        public async Task<InventoryItem?> GetByIdAsync(int id)
        {
            return await _context.InventoryItems
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.Id == id);
        }

        public async Task<InventoryItem?> UpdateAsync(InventoryItem inventoryItem)
        {
            var existingItem = await _context.InventoryItems.FirstOrDefaultAsync(item => item.Id == inventoryItem.Id);
            if (existingItem == null)
            {
                return null;
            }

            existingItem.ItemNumber = inventoryItem.ItemNumber;
            existingItem.ItemName = inventoryItem.ItemName;
            existingItem.OpeningQuantity = inventoryItem.OpeningQuantity;
            existingItem.RemainingQuantity = inventoryItem.RemainingQuantity;
            existingItem.ItemType = inventoryItem.ItemType;
            existingItem.ItemCategory = inventoryItem.ItemCategory;
            existingItem.UoM = inventoryItem.UoM;
            existingItem.UnitPrice = inventoryItem.UnitPrice;
            existingItem.Currency = inventoryItem.Currency;
            existingItem.Remarks = inventoryItem.Remarks;
            existingItem.MinimumStockLevel = inventoryItem.MinimumStockLevel;
            existingItem.MaximumStockLevel = inventoryItem.MaximumStockLevel;
            existingItem.ReOrderLevel = inventoryItem.ReOrderLevel;
            existingItem.StorageLocation = inventoryItem.StorageLocation;
            existingItem.ShelveLife = inventoryItem.ShelveLife;
            existingItem.StorageConditions = inventoryItem.StorageConditions;
            existingItem.Sku = inventoryItem.Sku;
            existingItem.ExpiryDate = inventoryItem.ExpiryDate;
            existingItem.InventoryStatus = inventoryItem.InventoryStatus;

            _context.InventoryItems.Update(existingItem);
            await _context.SaveChangesAsync();

            return existingItem;
        }

        public IQueryable<InventoryItem> GetSearchQuery()
        {
            return _context.InventoryItems.AsNoTracking();
        }

        public async Task<List<UnitOfMeasure>> GetAllUoMsAsync()
        {
            return await _context.UnitsOfMeasure
                .Where(uom => uom.Status == (int)EntityStatus.Active)
                .AsNoTracking()
                .OrderBy(uom => uom.UoM)
                .ToListAsync();
        }

        public async Task<List<Currency>> GetAllCurrenciesAsync()
        {
            return await _context.Currencies
                .Where(currency => currency.Status == (int)EntityStatus.Active)
                .AsNoTracking()
                .OrderBy(currency => currency.CurrencyCode)
                .ToListAsync();
        }
    }
}
