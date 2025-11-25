using GearShop.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Security.Claims;

namespace GearShop.Controllers
{
    [ApiController]
    [Route("api/images")]
    public class ImageController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ImageController(AppDbContext context)
        {
            _context = context;
        }




        [HttpGet("user/{userId}")]
        [ResponseCache(Duration = 3600)]
        public async Task<IActionResult> GetUserProfilePicture(int userId)
        {
            var userImage = await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => new { u.ProfilePictureData, u.ProfilePictureMimeType })
                .FirstOrDefaultAsync();

            if (userImage?.ProfilePictureData == null || string.IsNullOrEmpty(userImage.ProfilePictureMimeType))
            {
                return NotFound();
            }
            return File(userImage.ProfilePictureData, userImage.ProfilePictureMimeType);
        }

        [HttpGet("product/{productId}")]
        [ResponseCache(Duration = 3600)]
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

        [Authorize]
        [HttpPost("user/{userId}")]
        public async Task<IActionResult> UploadUserProfilePicture(int userId, IFormFile file)
        {
            var authenticatedUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(authenticatedUserIdStr, out var authenticatedUserId))
            {
                return Unauthorized("Token de usuário inválido.");
            }

            if (authenticatedUserId != userId)
            {
                return Forbid();
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            if (file == null || file.Length == 0)
            {
                return BadRequest("Nenhum arquivo foi enviado.");
            }

            if (file.Length > 5 * 1024 * 1024)
            {
                return BadRequest("O arquivo é muito grande (máximo 5MB).");
            }

            if (file.ContentType != "image/jpeg" && file.ContentType != "image/png")
            {
                return BadRequest("Formato de arquivo inválido. Apenas JPG ou PNG são permitidos.");
            }

            await using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                user.ProfilePictureData = memoryStream.ToArray();
            }

            user.ProfilePictureMimeType = file.ContentType;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "Erro ao salvar a imagem no banco de dados.");
            }

            var newImagePath = $"/api/images/user/{userId}?t={System.DateTime.UtcNow.Ticks}";
            return Ok(new { message = "Foto de perfil atualizada com sucesso.", newImagePath });
        }
    }
}
