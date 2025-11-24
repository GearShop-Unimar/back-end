using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GearShop.Models.Enums; // Adicionado para SubscriptionStatus

namespace GearShop.Models
{
    public class PremiumAccount
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        [Required]
        public SubscriptionStatus Status { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal MonthlyAmount { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? NextPaymentDate { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        // Navegação para pagamentos da assinatura
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
