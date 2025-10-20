// Local: Services/Product/ProductService.cs

using GearShop.Dtos.Product;
using GearShop.Models;
using GearShop.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GearShop.Services.Product
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var products = await _productRepository.GetAllAsync();
            return products.Select(p => ToProductDto(p));
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            return product == null ? null : ToProductDto(product);
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto dto)
        {
            var product = new Models.Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                StockQuantity = dto.StockQuantity, // CORRIGIDO
                MainImageUrl = dto.MainImageUrl,
                Category = dto.Category,       // CORRIGIDO
                SellerId = dto.SellerId
            };

            var createdProduct = await _productRepository.CreateAsync(product);
            return ToProductDto(createdProduct);
        }

        public async Task<ProductDto?> UpdateAsync(int id, UpdateProductDto dto)
        {
            var productToUpdate = await _productRepository.GetByIdAsync(id);
            if (productToUpdate == null)
            {
                return null;
            }

            productToUpdate.Name = dto.Name;
            productToUpdate.Description = dto.Description;
            productToUpdate.Price = dto.Price;
            productToUpdate.StockQuantity = dto.StockQuantity;
            productToUpdate.MainImageUrl = dto.MainImageUrl;

            var updatedProduct = await _productRepository.UpdateAsync(id, productToUpdate);

            return updatedProduct == null ? null : ToProductDto(updatedProduct);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _productRepository.DeleteAsync(id);
        }

        private static ProductDto ToProductDto(Models.Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity, // CORRIGIDO
                MainImageUrl = product.MainImageUrl,
                Category = product.Category,       // CORRIGIDO
                SellerId = product.SellerId
            };
        }
    }
}