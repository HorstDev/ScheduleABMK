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
    public class TeacherRepository : ITeacherRepository
    {
        private readonly ScheduleDataContext _context;

        public TeacherRepository(ScheduleDataContext context)
        {
            _context = context;
        }

        public async Task<IList<Teacher>> GetAllAsync()
        {
            return await _context.Teachers.ToListAsync();
        }

        public async Task AddAsync(Teacher teacher)
        {
            await _context.Teachers.AddAsync(teacher);
        }
    }
}
