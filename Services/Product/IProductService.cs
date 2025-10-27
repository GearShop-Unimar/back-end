using GearShop.Dtos.Product;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GearShop.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllAsync();

        Task<ProductDto?> GetByIdAsync(int id);

        Task<ProductDto> CreateAsync(CreateProductDto dto, byte[]? imageData, string? imageMimeType, int sellerId);

        Task<ProductDto?> UpdateAsync(int id, UpdateProductDto dto, byte[]? imageData, string? imageMimeType);

        Task<bool> DeleteAsync(int id);
    }
}