using GearShop.Models;
using GearShop.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GearShop.Dtos.Payment;

namespace GearShop.Services.Premium
{
    public class PremiumAccountService : IPremiumAccountService
    {
        private readonly IPremiumAccountRepository _premiumAccountRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPaymentRepository _paymentRepository;

        public PremiumAccountService(IPremiumAccountRepository premiumAccountRepository, IUserRepository userRepository, IPaymentRepository paymentRepository)
        {
            _premiumAccountRepository = premiumAccountRepository;
            _userRepository = userRepository;
            _paymentRepository = paymentRepository;
        }

        public async Task<PremiumAccount> ActivatePremiumAsync(int userId, int durationDays, decimal price)
        {
            var existingAccount = await _premiumAccountRepository.GetByUserIdAsync(userId);
            var now = DateTime.Now;
            var endDate = now.AddDays(durationDays);
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                throw new ArgumentException("User not found.");
            }

            PremiumAccount premiumAccount;

            if (existingAccount == null)
            {
                premiumAccount = new PremiumAccount
                {
                    UserId = userId,
                    StartDate = now,
                    EndDate = endDate,
                    Status = Models.Enums.SubscriptionStatus.Active,
                    Price = price,
                    CreatedAt = now
                };
                await _premiumAccountRepository.CreateAsync(premiumAccount);
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
                existingAccount.Price = price;
                existingAccount.UpdatedAt = now;
                await _premiumAccountRepository.UpdateAsync(existingAccount);
                premiumAccount = existingAccount;
            }

            // Simulação de interação com gateway de pagamento
            string transactionId = Guid.NewGuid().ToString(); // Gerar um ID de transação fictício
            string subscriptionId = Guid.NewGuid().ToString(); // Gerar um ID de assinatura fictício para pagamentos recorrentes

            var payment = new Payment
            {
                PremiumAccountId = premiumAccount.Id,
                PaymentType = Models.Enums.PaymentType.CreditCard, // Assumindo cartão de crédito para recorrência, pode ser ajustado
                Status = Models.Enums.PaymentStatus.Approved,
                Amount = price,
                CreatedAt = now,
                ProcessedAt = now,
                IsRecurring = true,
                TransactionId = transactionId,
                SubscriptionId = subscriptionId
            };
            await _paymentRepository.CreateAsync(payment);

            return premiumAccount;
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

        public async Task<IEnumerable<PaymentDto>> GetPaymentHistoryAsync(int userId)
        {
            var premiumAccount = await _premiumAccountRepository.GetByUserIdAsync(userId);

            if (premiumAccount == null)
            {
                return new List<PaymentDto>();
            }

            var payments = await _paymentRepository.GetByPremiumAccountIdAsync(premiumAccount.Id);

            return payments.Select(p => new PaymentDto
            {
                Id = p.Id,
                Amount = p.Amount,
                PaymentType = p.PaymentType,
                Status = p.Status,
                CreatedAt = p.CreatedAt,
                ProcessedAt = p.ProcessedAt,
                IsRecurring = p.IsRecurring
            }).ToList();
        }

        public async Task ProcessRecurringPaymentsAsync()
        {
            // Implementar lógica para processar pagamentos recorrentes
            // Por exemplo, buscar todas as contas premium ativas e
            // verificar se um novo pagamento recorrente é devido.
            await Task.CompletedTask; // Placeholder
        }
    }
}
