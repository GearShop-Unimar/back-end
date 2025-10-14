using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GearShop.Models.Enums;

namespace GearShop.Models
{
    public class Order
    {
        public int Id { get; set; }
        
        [Required]
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }
        
        [Required]
        public OrderStatus Status { get; set; }
        
        [Required]
        public DateTime CreatedAt { get; set; }
        
        public DateTime? UpdatedAt { get; set; }
        
        [MaxLength(500)]
        public string? Notes { get; set; }
        
        // Navegação para itens do pedido
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        
        // Navegação para pagamentos
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
