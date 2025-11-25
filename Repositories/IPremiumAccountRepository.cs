using GearShop.Models;
using System.Threading.Tasks;

namespace GearShop.Repositories
{
    public interface IPremiumAccountRepository
    {
        Task<PremiumAccount?> GetByUserIdAsync(int userId);
        Task<PremiumAccount> CreateAsync(PremiumAccount account);
        Task UpdateAsync(PremiumAccount account);
        Task DeleteAsync(int accountId);
    }
}
