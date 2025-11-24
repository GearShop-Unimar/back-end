using GearShop.Models;
using GearShop.Repositories;
using System;
using System.Threading.Tasks;

namespace GearShop.Services.Premium
{
    public class PremiumAccountService : IPremiumAccountService
    {
        private readonly IPremiumAccountRepository _premiumAccountRepository;
        private readonly IUserRepository _userRepository;

        public PremiumAccountService(IPremiumAccountRepository premiumAccountRepository, IUserRepository userRepository)
        {
            _premiumAccountRepository = premiumAccountRepository;
            _userRepository = userRepository;
        }

        public async Task<PremiumAccount> ActivatePremiumAsync(int userId, int durationDays)
        {
            var existingAccount = await _premiumAccountRepository.GetByUserIdAsync(userId);
            var now = DateTime.Now;
            var endDate = now.AddDays(durationDays);
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                throw new ArgumentException("User not found.");
            }

            if (existingAccount == null)
            {
                var newAccount = new PremiumAccount
                {
                    UserId = userId,
                    StartDate = now,
                    EndDate = endDate,
                    Status = Models.Enums.SubscriptionStatus.Active,
                    CreatedAt = now
                };
                return await _premiumAccountRepository.CreateAsync(newAccount);
            }
            else
            {
                if (existingAccount.EndDate.HasValue && existingAccount.EndDate.Value > now)
                {
                    existingAccount.EndDate = existingAccount.EndDate.Value.AddDays(durationDays);
                }
                else
                {
                    existingAccount.EndDate = endDate;
                }
                existingAccount.Status = Models.Enums.SubscriptionStatus.Active;
                existingAccount.UpdatedAt = now;
                await _premiumAccountRepository.UpdateAsync(existingAccount);
                return existingAccount;
            }
        }

        public async Task<bool> IsUserPremiumAsync(int userId)
        {
            var account = await _premiumAccountRepository.GetByUserIdAsync(userId);

            if (account == null || account.Status != Models.Enums.SubscriptionStatus.Active)
            {
                return false;
            }

            if (account.EndDate.HasValue && account.EndDate.Value < DateTime.Now)
            {
                account.Status = Models.Enums.SubscriptionStatus.Expired;
                account.UpdatedAt = DateTime.Now;
                await _premiumAccountRepository.UpdateAsync(account);
                return false;
            }

            return true;
        }

        public async Task<PremiumAccount?> GetAccountDetailsAsync(int userId)
        {
            return await _premiumAccountRepository.GetByUserIdAsync(userId);
        }

        public async Task CancelPremiumAsync(int userId)
        {
            var account = await _premiumAccountRepository.GetByUserIdAsync(userId);
            if (account != null && account.Status == Models.Enums.SubscriptionStatus.Active)
            {
                account.Status = Models.Enums.SubscriptionStatus.Cancelled;
                account.UpdatedAt = DateTime.Now;
                await _premiumAccountRepository.UpdateAsync(account);
            }
        }
    }
}
