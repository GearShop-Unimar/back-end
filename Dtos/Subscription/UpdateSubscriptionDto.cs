using GearShop.Models.Enums;

namespace GearShop.Dtos.Subscription
{
    public class UpdateSubscriptionDto
    {
        public SubscriptionStatus Status { get; set; }
        
        public decimal? MonthlyAmount { get; set; }
        
        public DateTime? EndDate { get; set; }
        
        public string? Notes { get; set; }
    }
}
