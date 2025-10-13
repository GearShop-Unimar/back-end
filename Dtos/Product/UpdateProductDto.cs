using System.ComponentModel.DataAnnotations;

// Mantenha o namespace consistente com a pasta Dtos/Product
namespace GearShop.Dtos.Product
{
    // Ele herda todos os campos, como Name, Price, StockQuantity, etc., 
    // do CreateProductDto.
    public class UpdateProductDto : CreateProductDto
    {
        // Esta classe fica vazia, pois herda tudo o que precisa.
    }
}