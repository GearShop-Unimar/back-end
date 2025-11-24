using GearShop.Models; // Adicionar/Garantir este using
using System.Threading.Tasks;

namespace GearShop.Services.Premium
{
    public interface IPremiumAccountService
    {
        // Certifique-se que o retorno é o tipo de classe correto, não o nome da interface
        Task<PremiumAccount> ActivatePremiumAsync(int userId, int durationDays);
        Task<bool> IsUserPremiumAsync(int userId);
        Task<PremiumAccount?> GetAccountDetailsAsync(int userId);
        Task CancelPremiumAsync(int userId);
    }
}
