using AMB.Application.Interfaces.Repositories;
using AMB.Domain.Entities;
using AMB.Domain.Enums;
using AMB.Infra.DBContexts;
using Microsoft.EntityFrameworkCore;

namespace AMB.Infra.Repositories
{
    public class TableRespository: ITableRepository
    {
        private readonly AMBContext _context;

        public TableRespository(AMBContext context)
        {
            _context = context;
        }

        public async Task<Table> AddAsync(Table table)
        {
            _context.Tables.Add(table);
            await _context.SaveChangesAsync();
            return table;
        }

        public async Task UpdateStatusAsync(int id, EntityStatus entityStatus)
        {
            var table = await _context.Tables.SingleOrDefaultAsync(t => t.Id == id);
            if (table == null)
            {
                throw new InvalidOperationException($"Table with id {id} not found.");
            }

            table.Status = (int)entityStatus;
            await _context.SaveChangesAsync();
        }

        public async Task<List<Table>> GetAllAsync()
        {
            return await _context.Tables
                .AsNoTracking()
                .ToListAsync();
        }

        public IQueryable<Table> GetSearchQuery()
        {
            return _context.Tables.AsNoTracking();
        }
    }
}
