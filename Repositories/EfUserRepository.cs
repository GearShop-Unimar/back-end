using GearShop.Models;
using Microsoft.EntityFrameworkCore;
using GearShop.Data;

namespace GearShop.Repositories
{
    public class EfUserRepository : IUserRepository
    {
        private readonly AppDbContext _db;
        public EfUserRepository(AppDbContext db) => _db = db;

        public Task<List<User>> GetAllAsync() => _db.Users.AsNoTracking().ToListAsync();
        public Task<User?> GetByIdAsync(int id) => _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);

        public async Task<User> CreateAsync(User user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return user;
        }

        public async Task<User?> UpdateAsync(int id, User data)
        {
            var u = await _db.Users.FindAsync(id);
            if (u is null) return null;
            u.Name = data.Name; u.Email = data.Email; u.phoneNumber = data.phoneNumber; u.profilePicture = data.profilePicture;
            await _db.SaveChangesAsync();
            return u;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var u = await _db.Users.FindAsync(id);
            if (u is null) return false;
            _db.Users.Remove(u);
            await _db.SaveChangesAsync();
            return true;
        }

        public Task<bool> EmailExistsAsync(string email, int? exceptId = null) =>
            _db.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower() && (!exceptId.HasValue || u.Id != exceptId));
    }
}
