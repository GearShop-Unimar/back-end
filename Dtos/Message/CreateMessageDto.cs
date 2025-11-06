using System.ComponentModel.DataAnnotations;

namespace GearShop.Dtos.Message
{
    public class CreateMessageDto
    {
        [Required]
        public required string Text { get; set; }
    }
}
