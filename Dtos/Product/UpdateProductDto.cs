using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace GearShop.Dtos.Product
{
    public class UpdateProductDto
    {
        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        [Required, MaxLength(100)]
        public string Category { get; set; } = string.Empty;

        public IFormFile? ImageFile { get; set; }
    }
}
