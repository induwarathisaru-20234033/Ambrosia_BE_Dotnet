using AMB.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMB.Application.Interfaces.Repositories
{
    public interface ICalenderExclusionRepository
    {
        Task<CalenderExclusion> AddAsync(CalenderExclusion exclusion);
        Task<List<CalenderExclusion>> GetAllAsync();
    }
}
