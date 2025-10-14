using System.ComponentModel.DataAnnotations;
using GearShop.Models.Enums;

namespace GearShop.Dtos.Subscription
{
    public class CreateSubscriptionDto
    {
        [Required]
        public int UserId { get; set; }
        
        [Required]
        public int ProductId { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal MonthlyAmount { get; set; }
        
        [Required]
        public PaymentType PaymentType { get; set; }
        
        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}
