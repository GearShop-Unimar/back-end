using GearShop.Dtos.Product;
using GearShop.Models;
using GearShop.Repositories;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System;

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

        public async Task<ProductDto> CreateAsync(CreateProductDto dto, byte[]? imageData, string? imageMimeType, int sellerId)
        {
            var product = new Models.Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                StockQuantity = dto.StockQuantity,
                ImageData = imageData,
                ImageMimeType = imageMimeType,
                Category = dto.Category,
                SellerId = sellerId
            };

            var createdProduct = await _productRepository.CreateAsync(product);
            return ToProductDto(createdProduct);
        }

        public async Task<ProductDto?> UpdateAsync(int id, UpdateProductDto dto, byte[]? imageData, string? imageMimeType)
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
            productToUpdate.Category = dto.Category;

            if (imageData != null)
            {
                productToUpdate.ImageData = imageData;
                productToUpdate.ImageMimeType = imageMimeType;
            }

            var updatedProduct = await _productRepository.UpdateAsync(id, productToUpdate);

            return updatedProduct == null ? null : ToProductDto(updatedProduct);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _productRepository.DeleteAsync(id);
        }

        private static ProductDto ToProductDto(Models.Product product)
        {
            string imageUrl = string.Empty;
            if (product.ImageData != null && !string.IsNullOrEmpty(product.ImageMimeType))
            {
                imageUrl = $"data:{product.ImageMimeType};base64,{Convert.ToBase64String(product.ImageData)}";
            }

            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                MainImageUrl = imageUrl,
                Category = product.Category,
                SellerId = product.SellerId,
                AverageRating = product.Reviews != null && product.Reviews.Any() ? (float)product.Reviews.Average(r => r.Rating) : 0,
                ReviewCount = product.Reviews != null ? product.Reviews.Count : 0
            };
        }
    }
}
