using AMB.Application.Interfaces.Repositories;
using AMB.Domain.Entities;
using AMB.Domain.Enums;
using AMB.Infra.DBContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMB.Infra.Repositories
{
    public class CalenderExclusionRepository : ICalenderExclusionRepository
    {
        private readonly AMBContext _context;

        public CalenderExclusionRepository(AMBContext context)
        {
            _context = context;
        }

        public async Task<CalenderExclusion> AddAsync(CalenderExclusion exclusion)
        {
            await _context.CalenderExclusions.AddAsync(exclusion);
            await _context.SaveChangesAsync();
            return exclusion;
        }

        public async Task<List<CalenderExclusion>> GetAllAsync()
        {
            return await _context.CalenderExclusions
                .Where(ce => ce.Status == (int)EntityStatus.Active)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task UpdateStatusAsync(int id, EntityStatus entityStatus)
        {
            var calenderExclusion = await _context.CalenderExclusions.SingleOrDefaultAsync(t => t.Id == id);
            if (calenderExclusion == null)
            {
                throw new InvalidOperationException($"Exclusion with id {id} not found.");
            }

            calenderExclusion.Status = (int)entityStatus;
            await _context.SaveChangesAsync();
        }
    }
}
