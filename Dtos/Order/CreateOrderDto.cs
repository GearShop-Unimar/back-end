using System.ComponentModel.DataAnnotations;
using GearShop.Models.Enums;

namespace GearShop.Dtos.Order
{
    public class CreateOrderDto
    {
        [Required]
        public int UserId { get; set; }
        
        [Required]
        public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
        
        [MaxLength(500)]
        public string? Notes { get; set; }
        
        [Required]
        public PaymentType PaymentType { get; set; }
    }
    
    public class OrderItemDto
    {
        [Required]
        public int ProductId { get; set; }
        
        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
