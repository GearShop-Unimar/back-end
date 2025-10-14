using GearShop.Data;
using GearShop.Models;
using GearShop.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace GearShop.Repositories
{
    public class EfPaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext _context;

        public EfPaymentRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<Payment>> GetAllAsync()
        {
            return await _context.Payments
                .Include(p => p.Order)
                    .ThenInclude(o => o.User)
                .Include(p => p.Subscription)
                    .ThenInclude(s => s.User)
                .ToListAsync();
        }

        public async Task<Payment?> GetByIdAsync(int id)
        {
            return await _context.Payments
                .Include(p => p.Order)
                    .ThenInclude(o => o.User)
                .Include(p => p.Subscription)
                    .ThenInclude(s => s.User)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Payment>> GetByOrderIdAsync(int orderId)
        {
            return await _context.Payments
                .Include(p => p.Order)
                    .ThenInclude(o => o.User)
                .Include(p => p.Subscription)
                    .ThenInclude(s => s.User)
                .Where(p => p.OrderId == orderId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetByUserIdAsync(int userId)
        {
            return await _context.Payments
                .Include(p => p.Order)
                    .ThenInclude(o => o.User)
                .Include(p => p.Subscription)
                    .ThenInclude(s => s.User)
                .Where(p => (p.Order != null && p.Order.UserId == userId) || 
                           (p.Subscription != null && p.Subscription.UserId == userId))
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<Payment> CreateAsync(Payment payment)
        {
            payment.CreatedAt = DateTime.UtcNow;
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<Payment?> UpdateAsync(int id, Payment payment)
        {
            var existingPayment = await _context.Payments.FindAsync(id);
            if (existingPayment is null) return null;

            existingPayment.Status = payment.Status;
            existingPayment.ExternalPaymentId = payment.ExternalPaymentId;
            existingPayment.PaymentUrl = payment.PaymentUrl;
            existingPayment.QrCode = payment.QrCode;
            existingPayment.Barcode = payment.Barcode;
            existingPayment.FailureReason = payment.FailureReason;
            existingPayment.LastFourDigits = payment.LastFourDigits;
            existingPayment.CardBrand = payment.CardBrand;
            
            if (payment.Status == PaymentStatus.Approved || payment.Status == PaymentStatus.Rejected)
            {
                existingPayment.ProcessedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return existingPayment;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment is null) return false;

            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Payment?> GetByExternalIdAsync(string externalPaymentId)
        {
            return await _context.Payments
                .Include(p => p.Order)
                .FirstOrDefaultAsync(p => p.ExternalPaymentId == externalPaymentId);
        }

        public async Task<IEnumerable<Payment>> GetPendingPaymentsAsync()
        {
            return await _context.Payments
                .Include(p => p.Order)
                .Include(p => p.Subscription)
                .Where(p => p.Status == PaymentStatus.Pending || p.Status == PaymentStatus.Processing)
                .ToListAsync();
        }
    }
}
