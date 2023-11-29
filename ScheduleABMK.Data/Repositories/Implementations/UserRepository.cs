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
    public class UserRepository : IUserRepository
    {
        private readonly ScheduleDataContext _db;

        public UserRepository(ScheduleDataContext db)
        {
            _db = db;
        }

        public async Task Create(User entity)
        {
            await _db.Users.AddAsync(entity);
            await _db.SaveChangesAsync();
        }

        public async Task<List<User>?> GetAll()
        {
            return await _db.Users.ToListAsync();
        }

        public async Task Delete(User entity)
        {
            _db.Users.Remove(entity);
            await _db.SaveChangesAsync();
        }

        public async Task<User> Update(User entity)
        {
            _db.Users.Update(entity);
            await _db.SaveChangesAsync();

            return entity;
        }

        public async Task<User?> GetById(Guid id)
        {
            return await _db.Users
                .Include(u => u.Role)
                .SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<User?> GetByEmail(string email)
        {
            return await _db.Users
                .Include(u => u.Role)
                .SingleOrDefaultAsync(x => x.Email == email);
        }

        public async Task<User?> GetByRefreshToken(string? refreshToken)
        {
            return await _db.Users
                .Include(u => u.Role)
                .SingleOrDefaultAsync(x => x.RefreshToken == refreshToken);
        }
    }
}
