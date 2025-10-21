using GearShop.Dtos.Product;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GearShop.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllAsync();

        Task<ProductDto?> GetByIdAsync(int id);

        // CORRIGIDO: Agora aceita 3 argumentos
        Task<ProductDto> CreateAsync(CreateProductDto dto, string imageUrl, int sellerId);

        Task<ProductDto?> UpdateAsync(int id, UpdateProductDto dto);

        Task<bool> DeleteAsync(int id);
    }
}