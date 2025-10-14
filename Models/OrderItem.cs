using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GearShop.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        
        [Required]
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;
        
        [Required]
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        
        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal UnitPrice { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalPrice { get; set; }
    }
}
