using AMB.Application.Interfaces.Repositories;
using AMB.Domain.Entities;
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
    }
}
