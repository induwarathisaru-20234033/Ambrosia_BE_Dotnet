using AMB.Application.Interfaces.Repositories;
using AMB.Domain.Entities;
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
            return wastageRecord;
        }
    }
}
