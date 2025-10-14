using GearShop.Data;
using GearShop.Models;
using GearShop.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace GearShop.Repositories
{
    public class EfSubscriptionRepository : ISubscriptionRepository
    {
        private readonly AppDbContext _context;

        public EfSubscriptionRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<Subscription>> GetAllAsync()
        {
            return await _context.Subscriptions
                .Include(s => s.User)
                .Include(s => s.Product)
                .ToListAsync();
        }

        public async Task<Subscription?> GetByIdAsync(int id)
        {
            return await _context.Subscriptions
                .Include(s => s.User)
                .Include(s => s.Product)
                .Include(s => s.Payments)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Subscription>> GetByUserIdAsync(int userId)
        {
            return await _context.Subscriptions
                .Include(s => s.User)
                .Include(s => s.Product)
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Subscription>> GetActiveSubscriptionsAsync()
        {
            return await _context.Subscriptions
                .Include(s => s.User)
                .Include(s => s.Product)
                .Where(s => s.Status == SubscriptionStatus.Active)
                .ToListAsync();
        }

        public async Task<IEnumerable<Subscription>> GetDueSubscriptionsAsync()
        {
            var today = DateTime.UtcNow.Date;
            return await _context.Subscriptions
                .Include(s => s.User)
                .Include(s => s.Product)
                .Where(s => s.Status == SubscriptionStatus.Active && 
                           s.NextPaymentDate.HasValue && 
                           s.NextPaymentDate.Value.Date <= today)
                .ToListAsync();
        }

        public async Task<Subscription> CreateAsync(Subscription subscription)
        {
            subscription.CreatedAt = DateTime.UtcNow;
            subscription.NextPaymentDate = subscription.StartDate.AddMonths(1);
            _context.Subscriptions.Add(subscription);
            await _context.SaveChangesAsync();
            return subscription;
        }

        public async Task<Subscription?> UpdateAsync(int id, Subscription subscription)
        {
            var existingSubscription = await _context.Subscriptions.FindAsync(id);
            if (existingSubscription is null) return null;

            existingSubscription.Status = subscription.Status;
            existingSubscription.MonthlyAmount = subscription.MonthlyAmount;
            existingSubscription.EndDate = subscription.EndDate;
            existingSubscription.NextPaymentDate = subscription.NextPaymentDate;
            existingSubscription.Notes = subscription.Notes;
            existingSubscription.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingSubscription;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var subscription = await _context.Subscriptions.FindAsync(id);
            if (subscription is null) return false;

            _context.Subscriptions.Remove(subscription);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UserHasActiveSubscriptionAsync(int userId, int productId)
        {
            return await _context.Subscriptions
                .AnyAsync(s => s.UserId == userId && 
                              s.ProductId == productId && 
                              s.Status == SubscriptionStatus.Active);
        }
    }
}
