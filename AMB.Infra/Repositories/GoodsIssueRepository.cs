using AMB.Application.Interfaces.Repositories;
using AMB.Domain.Entities;
using AMB.Domain.Enums;
using AMB.Infra.DBContexts;
using Microsoft.EntityFrameworkCore;

namespace AMB.Infra.Repositories
{
    public class GoodsIssueRepository : IGoodsIssueRepository
    {
        private readonly AMBContext _context;

        public GoodsIssueRepository(AMBContext context)
        {
            _context = context;
        }

        public async Task<GoodIssueNote> AddAsync(GoodIssueNote goodIssueNote)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _context.GoodIssueNotes.Add(goodIssueNote);
                await _context.SaveChangesAsync();

                var inventoryItemIds = goodIssueNote.GIItems
                    .Select(item => item.InventoryItemId)
                    .Distinct()
                    .ToList();

                var inventoryItems = await _context.InventoryItems
                    .Where(item => inventoryItemIds.Contains(item.Id))
                    .ToDictionaryAsync(item => item.Id);

                foreach (var item in goodIssueNote.GIItems)
                {
                    if (!inventoryItems.TryGetValue(item.InventoryItemId, out var inventoryItem))
                    {
                        throw new KeyNotFoundException($"Inventory item with ID {item.InventoryItemId} not found.");
                    }

                    inventoryItem.RemainingQuantity -= item.IssuedQuantity;

                    _context.StockTransactions.Add(new StockTransaction
                    {
                        InventoryItemId = inventoryItem.Id,
                        Quantity = item.IssuedQuantity,
                        Direction = (int)StockMovementDirection.Down,
                        TransactionType = (int)StockTransactionType.GoodIssuing,
                        ReferenceId = goodIssueNote.Id,
                        ReferenceLineId = item.Id,
                        ReferenceNumber = goodIssueNote.GINumber,
                        Remarks = string.IsNullOrWhiteSpace(item.Remarks)
                            ? "GI issued quantity"
                            : item.Remarks,
                        TransactionDate = DateTimeOffset.UtcNow,
                        Status = (int)EntityStatus.Active,
                    });
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return await _context.GoodIssueNotes
                    .Include(note => note.GIItems)
                    .ThenInclude(giItem => giItem.InventoryItem)
                    .AsNoTracking()
                    .FirstAsync(note => note.Id == goodIssueNote.Id);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<GoodIssueNote?> GetByIdAsync(int id)
        {
            return await _context.GoodIssueNotes
                .Include(note => note.GIItems)
                .ThenInclude(giItem => giItem.InventoryItem)
                .AsNoTracking()
                .FirstOrDefaultAsync(note => note.Id == id);
        }

        public async Task<GoodIssueNote?> UpdateAsync(GoodIssueNote goodIssueNote)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var existing = await _context.GoodIssueNotes
                    .Include(note => note.GIItems)
                    .FirstOrDefaultAsync(note => note.Id == goodIssueNote.Id);

                if (existing == null)
                {
                    return null;
                }

                var existingItemIds = existing.GIItems.Select(item => item.Id).ToList();
                var stockTransactions = await _context.StockTransactions
                    .Where(tx => tx.TransactionType == (int)StockTransactionType.GoodIssuing
                        && tx.ReferenceId == existing.Id
                        && tx.ReferenceLineId.HasValue
                        && existingItemIds.Contains(tx.ReferenceLineId.Value))
                    .ToListAsync();

                var rollbackInventoryIds = existing.GIItems
                    .Select(item => item.InventoryItemId)
                    .Distinct()
                    .ToList();

                var rollbackInventoryItems = await _context.InventoryItems
                    .Where(item => rollbackInventoryIds.Contains(item.Id))
                    .ToDictionaryAsync(item => item.Id);

                foreach (var item in existing.GIItems)
                {
                    if (rollbackInventoryItems.TryGetValue(item.InventoryItemId, out var inventoryItem))
                    {
                        inventoryItem.RemainingQuantity += item.IssuedQuantity;
                    }
                }

                if (stockTransactions.Count > 0)
                {
                    _context.StockTransactions.RemoveRange(stockTransactions);
                }

                existing.IssuedBy = goodIssueNote.IssuedBy;
                existing.IssuedDate = goodIssueNote.IssuedDate;
                existing.Status = goodIssueNote.Status;

                _context.Set<GIItem>().RemoveRange(existing.GIItems);
                existing.GIItems.Clear();

                foreach (var item in goodIssueNote.GIItems)
                {
                    existing.GIItems.Add(new GIItem
                    {
                        LineItemNo = item.LineItemNo,
                        InventoryItemId = item.InventoryItemId,
                        RequestedQuantity = item.RequestedQuantity,
                        IssuedQuantity = item.IssuedQuantity,
                        IssuedFrom = item.IssuedFrom,
                        IssuedTo = item.IssuedTo,
                        Remarks = item.Remarks,
                        Status = item.Status,
                    });
                }

                await _context.SaveChangesAsync();

                var applyInventoryIds = existing.GIItems
                    .Select(item => item.InventoryItemId)
                    .Distinct()
                    .ToList();

                var applyInventoryItems = await _context.InventoryItems
                    .Where(item => applyInventoryIds.Contains(item.Id))
                    .ToDictionaryAsync(item => item.Id);

                foreach (var item in existing.GIItems)
                {
                    if (!applyInventoryItems.TryGetValue(item.InventoryItemId, out var inventoryItem))
                    {
                        throw new KeyNotFoundException($"Inventory item with ID {item.InventoryItemId} not found.");
                    }

                    inventoryItem.RemainingQuantity -= item.IssuedQuantity;

                    _context.StockTransactions.Add(new StockTransaction
                    {
                        InventoryItemId = inventoryItem.Id,
                        Quantity = item.IssuedQuantity,
                        Direction = (int)StockMovementDirection.Down,
                        TransactionType = (int)StockTransactionType.GoodIssuing,
                        ReferenceId = existing.Id,
                        ReferenceLineId = item.Id,
                        ReferenceNumber = existing.GINumber,
                        Remarks = string.IsNullOrWhiteSpace(item.Remarks)
                            ? "GI issued quantity"
                            : item.Remarks,
                        TransactionDate = DateTimeOffset.UtcNow,
                        Status = (int)EntityStatus.Active,
                    });
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return await _context.GoodIssueNotes
                    .Include(note => note.GIItems)
                    .ThenInclude(giItem => giItem.InventoryItem)
                    .AsNoTracking()
                    .FirstAsync(note => note.Id == existing.Id);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<InventoryItem>> GetInventoryItemsByIdsAsync(List<int> inventoryItemIds)
        {
            return await _context.InventoryItems
                .Where(item => inventoryItemIds.Contains(item.Id))
                .ToListAsync();
        }

        public async Task<bool> ExistsByNumberAsync(string giNumber)
        {
            return await _context.GoodIssueNotes
                .AsNoTracking()
                .AnyAsync(note => note.GINumber == giNumber);
        }
    }
}