using System.ComponentModel.DataAnnotations;

namespace GearShop.Dtos.Product
{
    public class CreateProductDto
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

        [Url, MaxLength(500)]
        public string MainImageUrl { get; set; } = string.Empty;

        // Categoria do produto
        [Required, MaxLength(100)]
        public string Category { get; set; } = string.Empty;

        // Id do usuário vendedor
        [Required]
        public int SellerId { get; set; }
    }
}