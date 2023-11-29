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
    public class ClassroomRepository : IClassroomRepository
    {
        private readonly ScheduleDataContext _context;

        public ClassroomRepository(ScheduleDataContext context)
        {
            _context = context;
        }

        public async Task<IList<Classroom>> GetAllAsync()
        {
            return await _context.Classrooms.ToListAsync();
        }

        public async Task AddAsync(Classroom classroom)
        {
            await _context.Classrooms.AddAsync(classroom);
        }
    }
}
