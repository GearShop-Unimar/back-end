using System.ComponentModel.DataAnnotations;
using GearShop.Models.Enums;

namespace GearShop.Dtos.Payment
{
    public class CreatePaymentDto
    {
        [Required]
        public int OrderId { get; set; }
        
        [Required]
        public PaymentType PaymentType { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }
    }
}
