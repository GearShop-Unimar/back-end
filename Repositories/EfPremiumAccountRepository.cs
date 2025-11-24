using GearShop.Data;
using GearShop.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace GearShop.Repositories
{
    public class EfPremiumAccountRepository : IPremiumAccountRepository
    {
        private readonly AppDbContext _context;

        public EfPremiumAccountRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PremiumAccount?> GetByUserIdAsync(int userId)
        {
            return await _context.PremiumAccounts
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.UserId == userId);
        }

        public async Task<PremiumAccount> CreateAsync(PremiumAccount account)
        {
            // Ensure we don't try to insert an existing User navigation entity.
            if (account.User != null)
            {
                _context.Entry(account.User).State = EntityState.Unchanged;
            }
            _context.PremiumAccounts.Add(account);
            await _context.SaveChangesAsync();
            return account;
        }

        public async Task UpdateAsync(PremiumAccount account)
        {
            _context.PremiumAccounts.Update(account);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int accountId)
        {
            var account = await _context.PremiumAccounts.FindAsync(accountId);
            if (account != null)
            {
                _context.PremiumAccounts.Remove(account);
                await _context.SaveChangesAsync();
            }
        }
    }
}
