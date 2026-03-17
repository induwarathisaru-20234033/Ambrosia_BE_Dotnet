using AMB.Application.Interfaces.Repositories;
using AMB.Domain.Entities;
using AMB.Domain.Enums;
using AMB.Infra.DBContexts;
using Microsoft.EntityFrameworkCore;

namespace AMB.Infra.Repositories
{
    public class GoodReceiptNoteRepository : IGoodReceiptNoteRepository
    {
        private readonly AMBContext _context;

        public GoodReceiptNoteRepository(AMBContext context)
        {
            _context = context;
        }

        public async Task<GoodReceiptNote> AddAsync(GoodReceiptNote goodReceiptNote)
        {
            _context.GoodReceiptNotes.Add(goodReceiptNote);
            await _context.SaveChangesAsync();
            return goodReceiptNote;
        }

        public async Task<GoodReceiptNote?> GetByIdAsync(int id)
        {
            return await _context.GoodReceiptNotes
                .Include(grn => grn.GRNItems)
                .ThenInclude(item => item.PRItem!)
                .ThenInclude(prItem => prItem.InventoryItem)
                .AsNoTracking()
                .FirstOrDefaultAsync(grn => grn.Id == id);
        }

        public async Task<GoodReceiptNote?> UpdateAsync(GoodReceiptNote goodReceiptNote)
        {
            var existing = await _context.GoodReceiptNotes
                .Include(grn => grn.GRNItems)
                .FirstOrDefaultAsync(grn => grn.Id == goodReceiptNote.Id);

            if (existing == null)
            {
                return null;
            }

            existing.Supplier = goodReceiptNote.Supplier;
            existing.ReceivedDate = goodReceiptNote.ReceivedDate;
            existing.ReceivedBy = goodReceiptNote.ReceivedBy;
            existing.ReceivedFacility = goodReceiptNote.ReceivedFacility;
            existing.PurchaseRequestId = goodReceiptNote.PurchaseRequestId;
            existing.GRNStatus = goodReceiptNote.GRNStatus;
            existing.Status = goodReceiptNote.Status;

            _context.Set<GRNItem>().RemoveRange(existing.GRNItems);
            existing.GRNItems.Clear();

            foreach (var item in goodReceiptNote.GRNItems)
            {
                existing.GRNItems.Add(new GRNItem
                {
                    LineItemNo = item.LineItemNo,
                    PRItemId = item.PRItemId,
                    Remarks = item.Remarks,
                    ReceivedQuantity = item.ReceivedQuantity,
                    AcceptedQuantity = item.AcceptedQuantity,
                    RejectedQuantity = item.RejectedQuantity,
                    TotalPrice = item.TotalPrice,
                    Status = item.Status,
                });
            }

            await _context.SaveChangesAsync();

            return existing;
        }

        public async Task<List<PurchaseRequestItem>> GetPurchaseRequestItemsByIdsAsync(List<int> prItemIds)
        {
            return await _context.PurchaseRequestItems
                .Include(item => item.PurchaseRequest)
                .Where(item => prItemIds.Contains(item.Id))
                .ToListAsync();
        }

        public async Task<bool> ExistsByNumberAsync(string grnNumber)
        {
            return await _context.GoodReceiptNotes
                .AsNoTracking()
                .AnyAsync(grn => grn.GRNNumber == grnNumber);
        }

        public IQueryable<GoodReceiptNote> GetSearchQuery()
        {
            return _context.GoodReceiptNotes
                .Include(grn => grn.GRNItems)
                .AsNoTracking();
        }

        public async Task ProcessPostedGrnAsync(int grnId)
        {
            var grn = await _context.GoodReceiptNotes
                .Include(note => note.GRNItems)
                .ThenInclude(item => item.PRItem!)
                .ThenInclude(prItem => prItem.InventoryItem)
                .FirstOrDefaultAsync(note => note.Id == grnId);

            if (grn == null)
            {
                throw new KeyNotFoundException($"GRN with ID {grnId} not found.");
            }

            foreach (var grnItem in grn.GRNItems)
            {
                var inventoryItem = grnItem.PRItem?.InventoryItem;
                if (inventoryItem == null)
                {
                    continue;
                }

                if (grnItem.AcceptedQuantity > 0)
                {
                    inventoryItem.RemainingQuantity += grnItem.AcceptedQuantity;

                    _context.StockTransactions.Add(new StockTransaction
                    {
                        InventoryItemId = inventoryItem.Id,
                        Quantity = grnItem.AcceptedQuantity,
                        Direction = (int)StockMovementDirection.Up,
                        TransactionType = (int)StockTransactionType.GRN,
                        ReferenceId = grn.Id,
                        ReferenceLineId = grnItem.Id,
                        ReferenceNumber = grn.GRNNumber,
                        Remarks = string.IsNullOrWhiteSpace(grnItem.Remarks)
                            ? "GRN accepted quantity"
                            : grnItem.Remarks,
                        TransactionDate = DateTimeOffset.UtcNow,
                        Status = (int)EntityStatus.Active,
                    });
                }

                if (grnItem.RejectedQuantity > 0)
                {
                    _context.StockTransactions.Add(new StockTransaction
                    {
                        InventoryItemId = inventoryItem.Id,
                        Quantity = grnItem.RejectedQuantity,
                        Direction = (int)StockMovementDirection.Down,
                        TransactionType = (int)StockTransactionType.GRN,
                        ReferenceId = grn.Id,
                        ReferenceLineId = grnItem.Id,
                        ReferenceNumber = grn.GRNNumber,
                        Remarks = string.IsNullOrWhiteSpace(grnItem.Remarks)
                            ? "GRN rejected quantity"
                            : grnItem.Remarks,
                        TransactionDate = DateTimeOffset.UtcNow,
                        Status = (int)EntityStatus.Active,
                    });
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
