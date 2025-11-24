using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GearShop.Models.Enums;

namespace GearShop.Models
{
    public class Payment
    {
        public int Id { get; set; }
        
        public int? OrderId { get; set; }
        public Order? Order { get; set; }

        public int? PremiumAccountId { get; set; }
        public PremiumAccount? PremiumAccount { get; set; }
        
        [Required]
        public PaymentType PaymentType { get; set; }
        
        [Required]
        public PaymentStatus Status { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }
        
        [Required]
        public DateTime CreatedAt { get; set; }
        
        public DateTime? ProcessedAt { get; set; }
        
        [MaxLength(500)]
        public string? ExternalPaymentId { get; set; }
        
        [MaxLength(1000)]
        public string? PaymentUrl { get; set; } // Para PIX e Boleto
        
        [MaxLength(500)]
        public string? QrCode { get; set; } // Para PIX
        
        [MaxLength(500)]
        public string? Barcode { get; set; } // Para Boleto
        
        [MaxLength(1000)]
        public string? FailureReason { get; set; }
        
        // Campos específicos para cartão (simulados)
        [MaxLength(4)]
        public string? LastFourDigits { get; set; }
        
        [MaxLength(100)]
        public string? CardBrand { get; set; }
    }
}
