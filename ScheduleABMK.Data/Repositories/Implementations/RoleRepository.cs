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
    public class RoleRepository : IRoleRepository
    {
        private readonly ScheduleDataContext _db;

        public RoleRepository(ScheduleDataContext db)
        {
            _db = db;
        }

        public async Task<Role?> GetByName(string roleName)
        {
            return await _db.Roles.SingleOrDefaultAsync(x => x.Name == roleName);
        }
    }
}
