using Microsoft.AspNetCore.Mvc;
using GearShop.Dtos.Product;
using GearShop.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using System.Security.Claims;

namespace GearShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public ProductController(IProductService productService, IWebHostEnvironment hostingEnvironment)
        {
            _productService = productService;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var product = await _productService.GetAllAsync();
            return Ok(product);
        }

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

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int sellerId))
            {
                return Unauthorized(new { Message = "ID do vendedor não encontrado no token." });
            }

            if (dto.ImageFile.Length > 5 * 1024 * 1024)
            {
                ModelState.AddModelError("ImageFile", "O arquivo de imagem deve ter no máximo 5MB.");
                return BadRequest(ModelState);
            }

            string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.ImageFile.FileName);
            string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await dto.ImageFile.CopyToAsync(fileStream);
            }

            string imageUrl = $"/images/{uniqueFileName}";

            var product = await _productService.CreateAsync(dto, imageUrl, sellerId);

            return Created($"/api/Product/{product.Id}", product);
        }
    }
}