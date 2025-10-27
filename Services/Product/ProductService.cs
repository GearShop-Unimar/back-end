using GearShop.Data; // Added for AppDbContext (if needed, otherwise repository is enough)
using GearShop.Dtos.Product;
using GearShop.Models;
using GearShop.Repositories;
using System.Collections.Generic;
using System.IO; // Added for MemoryStream
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http; // Added for IFormFile (if needed directly)

namespace GearShop.Services.Product
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        // Optional: Inject DbContext if repository doesn't handle byte[] saving well
        // private readonly AppDbContext _context;

        public ProductService(IProductRepository productRepository /*, AppDbContext context */)
        {
            _productRepository = productRepository;
            // _context = context;
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

        // --- CreateAsync MODIFIED ---
        // Option 1: Keep signature, but get bytes inside (less clean)
        // public async Task<ProductDto> CreateAsync(CreateProductDto dto, string imageUrl, int sellerId) { ... }

        // Option 2 (Better): Change signature to take IFormFile directly or handle bytes in Controller
        // Let's modify the Controller instead and keep the service clean for now.
        // We will assume the controller reads the bytes and passes them.
        public async Task<ProductDto> CreateAsync(CreateProductDto dto, byte[]? imageData, string? imageMimeType, int sellerId)
        {
            var product = new Models.Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                StockQuantity = dto.StockQuantity,
                ImageData = imageData, // Use passed bytes
                ImageMimeType = imageMimeType, // Use passed MIME type
                Category = dto.Category,
                SellerId = sellerId
            };

            var createdProduct = await _productRepository.CreateAsync(product);
            return ToProductDto(createdProduct);
        }

        // --- UpdateAsync MODIFIED (Needs similar byte handling) ---
        // For simplicity, assuming UpdateProductDto might also get an IFormFile or bytes
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
            productToUpdate.Category = dto.Category; // Added Category update

            // Update image data only if new data is provided
            if (imageData != null)
            {
                productToUpdate.ImageData = imageData;
                productToUpdate.ImageMimeType = imageMimeType;
            }
            // If you want to allow REMOVING the image, you'd need another flag/logic

            var updatedProduct = await _productRepository.UpdateAsync(id, productToUpdate);

            return updatedProduct == null ? null : ToProductDto(updatedProduct);
        }


        public async Task<bool> DeleteAsync(int id)
        {
            // Consider deleting related image data if stored externally
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
                StockQuantity = product.StockQuantity,
                // REMOVED: MainImageUrl = product.MainImageUrl,
                Category = product.Category,
                SellerId = product.SellerId
                // Frontend will get image via /api/images/product/{product.Id}
            };
        }

        // --- INTERFACE NEEDS UPDATE ---
        // Update IProductService to match the new signatures
        // Add methods like:
        // Task<ProductDto> CreateAsync(CreateProductDto dto, byte[]? imageData, string? imageMimeType, int sellerId);
        // Task<ProductDto?> UpdateAsync(int id, UpdateProductDto dto, byte[]? imageData, string? imageMimeType);
    }
}