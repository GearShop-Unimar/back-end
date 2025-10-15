using System.ComponentModel.DataAnnotations;

// Mantenha o namespace consistente com a pasta Dtos/Product
namespace GearShop.Dtos.Product
{
    // Ele herda todos os campos, como Name, Price, StockQuantity, etc., 
    // do CreateProductDto.
    public class UpdateProductDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string MainImageUrl { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;

        // Adicione SellerId
        public int SellerId { get; set; }
    }
}