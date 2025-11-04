using GearShop.Dtos.Product;
using GearShop.Services.Review;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GearShop.Controllers
{
    [ApiController]
    [Route("api/review")] // Alterei a rota para /api/review
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            this._reviewService = reviewService;
        }

        // POST /api/review
        [HttpPost]
        [Authorize] // Só utilizadores autenticados podem criar
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewDto createDto)
        {
            try
            {
                // Passa o 'User' (ClaimsPrincipal) do HttpContext para o serviço
                var reviewDto = await this._reviewService.CreateReviewAsync(createDto, this.User);

                // Retorna 201 Created com a localização do novo recurso
                return CreatedAtAction(nameof(GetReviewById), new { id = reviewDto.Id }, reviewDto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message }); // Conflito (ex: já avaliou)
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        // GET /api/review/product/5
        [HttpGet("product/{productId}")]
        [AllowAnonymous] // Todos podem ver as reviews
        public async Task<IActionResult> GetReviewsForProduct(int productId)
        {
            var reviews = await this._reviewService.GetReviewsForProductAsync(productId);
            return Ok(reviews);
        }

        // Este método é só para o CreatedAtAction funcionar.
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetReviewById(int id)
        {
            return Ok(new { message = $"Endpoint para GetReviewById({id}) chamado." });
        }
    }
}
