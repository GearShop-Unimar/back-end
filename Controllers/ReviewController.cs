using GearShop.Dtos.Product;
using GearShop.Services.Review;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GearShop.Controllers
{
    [ApiController]
    [Route("api/review")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            this._reviewService = reviewService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewDto createDto)
        {
            try
            {
                var reviewDto = await this._reviewService.CreateReviewAsync(createDto, this.User);

                return CreatedAtAction(nameof(GetReviewById), new { id = reviewDto.Id }, reviewDto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }


        [HttpGet("product/{productId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetReviewsForProduct(int productId)
        {
            var reviews = await this._reviewService.GetReviewsForProductAsync(productId);
            return Ok(reviews);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public IActionResult GetReviewById(int id)
        {
            return Ok(new { message = $"Endpoint para GetReviewById({id}) chamado." });
        }
    }
}
