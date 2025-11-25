using GearShop.Models; // Adicionar/Garantir este using
using System.Threading.Tasks;
using GearShop.Dtos.Payment;
using System.Collections.Generic;

namespace GearShop.Services.Premium
{
    public interface IPremiumAccountService
    {
        // Certifique-se que o retorno é o tipo de classe correto, não o nome da interface
        Task<PremiumAccount> ActivatePremiumAsync(int userId, int durationDays, decimal price);
        Task<bool> IsUserPremiumAsync(int userId);
        Task<PremiumAccount?> GetAccountDetailsAsync(int userId);
        Task CancelPremiumAsync(int userId);
        Task<IEnumerable<PaymentDto>> GetPaymentHistoryAsync(int userId);
        Task ProcessRecurringPaymentsAsync();
    }
}
