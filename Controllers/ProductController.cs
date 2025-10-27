using GearShop.Dtos.Product;
using GearShop.Services.Product; // Correct using directive
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using GearShop.Services; // Added for Authorize attribute


namespace GearShop.Controllers
{
    [Authorize] // Added Authorize attribute, common for product creation
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService) // Updated constructor
        {
            _productService = productService;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                throw new ApplicationException("ID do utilizador não encontrado no token.");
            }
            return userId;
        }

        [AllowAnonymous] // Assuming fetching products is public
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }

        [AllowAnonymous] // Assuming fetching product by ID is public
        [HttpGet("{id}", Name = "GetProductById")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound();
            return Ok(product);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateAsync([FromForm] CreateProductDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var sellerId = GetUserId();

            byte[]? imageData = null;
            string? imageMimeType = null;
            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                if (dto.ImageFile.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("ImageFile", "O arquivo de imagem deve ter no máximo 5MB.");
                    return BadRequest(ModelState);
                }
                using (var memoryStream = new MemoryStream())
                {
                    await dto.ImageFile.CopyToAsync(memoryStream);
                    imageData = memoryStream.ToArray();
                    imageMimeType = dto.ImageFile.ContentType;
                }
            }
            else
            {
                ModelState.AddModelError("ImageFile", "O arquivo de imagem é obrigatório.");
                return BadRequest(ModelState);
            }


            try
            {
                var product = await _productService.CreateAsync(dto, imageData, imageMimeType, sellerId);
                return CreatedAtAction(nameof(GetByIdAsync), new { id = product.Id }, product);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating product: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um erro ao criar o produto.");
            }
        }

        // Add UpdateAsync and DeleteAsync methods here if needed,
        // applying similar logic for image handling in UpdateAsync
    }
}