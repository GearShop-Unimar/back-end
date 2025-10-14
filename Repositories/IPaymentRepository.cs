using GearShop.Models;
using GearShop.Models.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GearShop.Repositories
{
    public interface IPaymentRepository
    {
        Task<IEnumerable<Payment>> GetAllAsync();
        Task<Payment?> GetByIdAsync(int id);
        Task<IEnumerable<Payment>> GetByOrderIdAsync(int orderId);
        Task<IEnumerable<Payment>> GetByUserIdAsync(int userId);
        Task<Payment> CreateAsync(Payment payment);
        Task<Payment?> UpdateAsync(int id, Payment payment);
        Task<bool> DeleteAsync(int id);
        Task<Payment?> GetByExternalIdAsync(string externalPaymentId);
        Task<IEnumerable<Payment>> GetPendingPaymentsAsync();
    }
}
