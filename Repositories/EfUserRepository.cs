using GearShop.Models;
using Microsoft.EntityFrameworkCore;
using GearShop.Data;

namespace GearShop.Repositories
{
    public class EfUserRepository : IUserRepository
    {
        private readonly AppDbContext db;
        public EfUserRepository(AppDbContext db) => this.db = db;

        public Task<List<User>> GetAllAsync() => db.Users.AsNoTracking().ToListAsync();

        public Task<User?> GetByIdAsync(int id) => db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);

        public Task<User?> GetByEmailAsync(string email) =>
            db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

        public async Task<User> CreateAsync(User user)
        {
            db.Users.Add(user);
            await db.SaveChangesAsync();
            return user;
        }

        public async Task<User?> UpdateAsync(int id, User data)
        {
            var u = await db.Users.FindAsync(id);
            if (u is null) return null;

            u.Name = data.Name;
            u.Email = data.Email;
            u.PhoneNumber = data.PhoneNumber;
            u.Cpf = data.Cpf;
            u.Estado = data.Estado;
            u.Cidade = data.Cidade;
            u.Cep = data.Cep;
            u.Rua = data.Rua;
            u.NumeroCasa = data.NumeroCasa;

            await db.SaveChangesAsync();
            return u;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var u = await db.Users.FindAsync(id);
            if (u is null) return false;
            db.Users.Remove(u);
            await db.SaveChangesAsync();
            return true;
        }

        public Task<bool> EmailExistsAsync(string email, int? exceptId = null) =>
            db.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower() && (!exceptId.HasValue || u.Id != exceptId));
    }
}
