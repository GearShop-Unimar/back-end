using GearShop.Dtos.Product;
using System.Collections.Generic; // Necessário para IEnumerable
using System.Threading.Tasks;    // MUITO IMPORTANTE: Necessário para usar Task

namespace GearShop.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllAsync();

        Task<ProductDto?> GetByIdAsync(int id);

        Task<ProductDto> CreateAsync(CreateProductDto dto);

        Task<ProductDto?> UpdateAsync(int id, UpdateProductDto dto);

        Task<bool> DeleteAsync(int id);
    }
}