using System.ComponentModel.DataAnnotations;

namespace GearShop.Dtos.Product
{
    public class CreateReviewDto
    {
        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string? Comment { get; set; }

        [Required]
        public int ProductId { get; set; }
    }
}
