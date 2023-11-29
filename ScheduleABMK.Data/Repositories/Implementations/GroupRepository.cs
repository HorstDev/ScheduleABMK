using Microsoft.EntityFrameworkCore;
using ScheduleABMK.Data.Repositories.Interfaces;
using ScheduleABMK.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleABMK.Data.Repositories.Implementations
{
    public class GroupRepository : IGroupRepository
    {
        private readonly ScheduleDataContext _context;

        public GroupRepository(ScheduleDataContext context)
        {
            _context = context;
        }

        public async Task<IList<Group>> GetAllAsync()
        {
            return await _context.Groups.ToListAsync();
        }

        public async Task AddAsync(Group group)
        {
            await _context.Groups.AddAsync(group);
        }
    }
}
