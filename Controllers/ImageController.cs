using GearShop.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace GearShop.Controllers
{
    [ApiController]
    [Route("api/images")] // Base route for images
    public class ImageController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ImageController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("post/{postId}")]
        [ResponseCache(Duration = 3600)] // Cache image for 1 hour
        public async Task<IActionResult> GetPostImage(int postId)
        {
            var postImage = await _context.Posts
                .Where(p => p.Id == postId)
                .Select(p => new { p.ImageData, p.ImageMimeType })
                .FirstOrDefaultAsync();

            if (postImage?.ImageData == null || string.IsNullOrEmpty(postImage.ImageMimeType))
            {
                return NotFound();
            }
            return File(postImage.ImageData, postImage.ImageMimeType);
        }

        [HttpGet("user/{userId}")]
        [ResponseCache(Duration = 3600)] // Cache image for 1 hour
        public async Task<IActionResult> GetUserProfilePicture(int userId)
        {
            var userImage = await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => new { u.ProfilePictureData, u.ProfilePictureMimeType })
                .FirstOrDefaultAsync();

            if (userImage?.ProfilePictureData == null || string.IsNullOrEmpty(userImage.ProfilePictureMimeType))
            {
                return NotFound(); // Or return a default avatar image
            }
            return File(userImage.ProfilePictureData, userImage.ProfilePictureMimeType);
        }

        [HttpGet("product/{productId}")]
        [ResponseCache(Duration = 3600)] // Cache image for 1 hour
        public async Task<IActionResult> GetProductImage(int productId)
        {
            var productImage = await _context.Products
                .Where(p => p.Id == productId)
                .Select(p => new { p.ImageData, p.ImageMimeType })
                .FirstOrDefaultAsync();

            if (productImage?.ImageData == null || string.IsNullOrEmpty(productImage.ImageMimeType))
            {
                return NotFound();
            }
            return File(productImage.ImageData, productImage.ImageMimeType);
        }
    }
}