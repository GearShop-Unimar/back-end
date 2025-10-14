using GearShop.Models.Enums;

namespace GearShop.Dtos.Payment
{
    public class UpdatePaymentDto
    {
        public PaymentStatus Status { get; set; }
        
        public string? ExternalPaymentId { get; set; }
        
        public string? PaymentUrl { get; set; }
        
        public string? QrCode { get; set; }
        
        public string? Barcode { get; set; }
        
        public string? FailureReason { get; set; }
        
        public string? LastFourDigits { get; set; }
        
        public string? CardBrand { get; set; }
    }
}
