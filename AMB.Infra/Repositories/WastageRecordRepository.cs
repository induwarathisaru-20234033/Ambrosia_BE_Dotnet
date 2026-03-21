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
    }
}
