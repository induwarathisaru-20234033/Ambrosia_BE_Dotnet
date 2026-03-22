using AMB.Application.Interfaces.Repositories;
using AMB.Domain.Entities;
using AMB.Domain.Enums;
using AMB.Infra.DBContexts;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AMB.Infra.Repositories
{
    public class WastageRecordRepository : IWastageRecordRepository
    {
        private readonly AMBContext _context;

        public WastageRecordRepository(AMBContext context)
        {
            _context = context;
        }

        public async Task<WastageRecord> AddAsync(WastageRecord wastageRecord)
        {
            _context.WastageRecords.Add(wastageRecord);
            await _context.SaveChangesAsync();

            // For each wastage entry item, create a stock transaction and update inventory
            foreach (var entry in wastageRecord.WastageEntryItems)
            {
                var inventoryItem = await _context.InventoryItems.FirstOrDefaultAsync(i => i.Id == entry.InventoryItemId);
                if (inventoryItem == null)
                {
                    continue;
                }

                if (entry.Quantity > 0)
                {
                    inventoryItem.RemainingQuantity -= entry.Quantity;

                    _context.StockTransactions.Add(new StockTransaction
                    {
                        InventoryItemId = inventoryItem.Id,
                        Quantity = entry.Quantity,
                        Direction = (int)StockMovementDirection.Down,
                        TransactionType = (int)StockTransactionType.Wastage,
                        ReferenceId = wastageRecord.Id,
                        ReferenceLineId = entry.Id,
                        ReferenceNumber = wastageRecord.WastageEntryNumber,
                        Remarks = string.IsNullOrWhiteSpace(entry.Reason)
                            ? "Wastage entry"
                            : entry.Reason,
                        TransactionDate = wastageRecord.EntryDate,
                        Status = (int)EntityStatus.Active,
                    });
                }
            }

            await _context.SaveChangesAsync();
            return wastageRecord;
        }

        public async Task<WastageRecord?> GetByIdAsync(int id)
        {
            return await _context.WastageRecords
                .Include(w => w.WastageEntryItems)
                .FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<WastageRecord?> UpdateAsync(WastageRecord wastageRecord)
        {
            var existing = await _context.WastageRecords
                .Include(w => w.WastageEntryItems)
                .FirstOrDefaultAsync(w => w.Id == wastageRecord.Id);

            if (existing == null)
            {
                return null;
            }

            existing.EntryDate = wastageRecord.EntryDate;
            existing.RecordedBy = wastageRecord.RecordedBy;
            existing.GeneralNotes = wastageRecord.GeneralNotes;

            // Remove old entry items and stock transactions
            _context.Set<WastageEntryItem>().RemoveRange(existing.WastageEntryItems);
            existing.WastageEntryItems.Clear();

            var oldStockTransactions = _context.StockTransactions.Where(st => st.TransactionType == (int)AMB.Domain.Enums.StockTransactionType.Wastage && st.ReferenceId == existing.Id);
            _context.StockTransactions.RemoveRange(oldStockTransactions);

            await _context.SaveChangesAsync();

            // Add new entry items and update inventory/stock
            foreach (var item in wastageRecord.WastageEntryItems)
            {
                var newEntry = new WastageEntryItem
                {
                    ItemNo = item.ItemNo,
                    WastageType = item.WastageType,
                    Quantity = item.Quantity,
                    Reason = item.Reason,
                    InventoryItemId = item.InventoryItemId
                };
                existing.WastageEntryItems.Add(newEntry);
                var inventoryItem = await _context.InventoryItems.FirstOrDefaultAsync(i => i.Id == item.InventoryItemId);
                if (inventoryItem != null)
                {
                    inventoryItem.RemainingQuantity -= item.Quantity;
                }
                _context.StockTransactions.Add(new AMB.Domain.Entities.StockTransaction
                {
                    InventoryItemId = item.InventoryItemId,
                    Quantity = item.Quantity,
                    Direction = (int)AMB.Domain.Enums.StockMovementDirection.Down,
                    TransactionType = (int)AMB.Domain.Enums.StockTransactionType.Wastage,
                    ReferenceId = existing.Id,
                    ReferenceLineId = null, // Will be set after SaveChanges if needed
                    ReferenceNumber = existing.WastageEntryNumber,
                    Remarks = string.IsNullOrWhiteSpace(item.Reason) ? "Wastage entry" : item.Reason,
                    TransactionDate = existing.EntryDate,
                    Status = (int)AMB.Domain.Enums.EntityStatus.Active,
                });
            }

            await _context.SaveChangesAsync();
            return existing;
        }
        public async Task<IEnumerable<WastageRecord>> GetAllAsync()
        {
            return await _context.WastageRecords
                .Include(w => w.WastageEntryItems)
                    .ThenInclude(i => i.InventoryItem)
                .ToListAsync();
        }
    }
}
