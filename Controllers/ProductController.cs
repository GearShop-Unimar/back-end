using Microsoft.AspNetCore.Mvc;
using GearShop.Dtos.Product;
using GearShop.Models;
using GearShop.Services; // Assumindo que IProductService está aqui

namespace GearShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // --- MÉTODOS ASSÍNCRONOS ---

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound();
            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateProductDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var product = await _productService.CreateAsync(dto);
            // Usamos o nome do método GetByIdAsync para gerar a URL de resposta
            return CreatedAtAction(nameof(GetByIdAsync), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] UpdateProductDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedProduct = await _productService.UpdateAsync(id, dto);
            if (updatedProduct == null)
                return NotFound();

            return Ok(updatedProduct);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var success = await _productService.DeleteAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}