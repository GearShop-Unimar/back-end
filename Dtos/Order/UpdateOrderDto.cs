using GearShop.Models.Enums;

namespace GearShop.Dtos.Order
{
    public class UpdateOrderDto
    {
        public OrderStatus Status { get; set; }
        
        public string? Notes { get; set; }
    }
}
