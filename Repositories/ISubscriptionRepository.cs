using GearShop.Models;
using GearShop.Models.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GearShop.Repositories
{
    public interface ISubscriptionRepository
    {
        Task<IEnumerable<Subscription>> GetAllAsync();
        Task<Subscription?> GetByIdAsync(int id);
        Task<IEnumerable<Subscription>> GetByUserIdAsync(int userId);
        Task<IEnumerable<Subscription>> GetActiveSubscriptionsAsync();
        Task<IEnumerable<Subscription>> GetDueSubscriptionsAsync();
        Task<Subscription> CreateAsync(Subscription subscription);
        Task<Subscription?> UpdateAsync(int id, Subscription subscription);
        Task<bool> DeleteAsync(int id);
        Task<bool> UserHasActiveSubscriptionAsync(int userId, int productId);
    }
}
