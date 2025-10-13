using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using GearShop.Models;
using GearShop.Dtos;
using GearShop.Repositories;
using GearShop.Dtos.Product;

namespace GearShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _repository;

        public ProductsController(IProductRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAll()
        {
            var products = await _repository.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Product>> GetById(int id)
        {
            var product = await _repository.GetByIdAsync(id);

            if (product is null)
            {
                return NotFound(new { message = $"Product with ID {id} not found" });
            }

            return Ok(product);
        }

        [HttpPost]
        [Consumes("application/json")]
        public async Task<ActionResult<Product>> Create([FromBody] CreateProductDto dto)
        {
            var product = new Product
            {
                Name = dto.Name.Trim(),
                Description = dto.Description.Trim(),
                Price = dto.Price,
                StockQuantity = dto.StockQuantity
            };

            var createdProduct = await _repository.CreateAsync(product);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdProduct.Id },
                createdProduct
            );
        }

        [HttpPut("{id:int}")]
        [Consumes("application/json")]
        public async Task<ActionResult<Product>> Update(int id, [FromBody] UpdateProductDto dto)
        {
            var productToUpdate = new Product
            {
                Id = id,
                Name = dto.Name.Trim(),
                Description = dto.Description.Trim(),
                Price = dto.Price,
                StockQuantity = dto.StockQuantity
            };

            var updatedProduct = await _repository.UpdateAsync(id, productToUpdate);

            if (updatedProduct is null)
            {
                return NotFound(new { message = $"Product with ID {id} not found" });
            }

            return Ok(updatedProduct);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _repository.DeleteAsync(id);

            return ok ? NoContent() : NotFound(new { message = $"Product with ID {id} not found" });
        }
    }
}