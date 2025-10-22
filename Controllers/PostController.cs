using GearShop.Dtos.Post;
using GearShop.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GearShop.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/posts")]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public PostController(IPostService postService, IWebHostEnvironment hostingEnvironment)
        {
            _postService = postService;
            _hostingEnvironment = hostingEnvironment;
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

        [HttpGet]
        public async Task<IActionResult> GetFeed()
        {
            var userId = GetUserId();
            var posts = await _postService.GetFeedAsync(userId);
            return Ok(posts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPostById(int id)
        {
            var userId = GetUserId();
            var post = await _postService.GetPostByIdAsync(id, userId);
            if (post == null) return NotFound();
            return Ok(post);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreatePost([FromForm] CreatePostDto dto)
        {
            var sellerId = GetUserId();
            string? imageUrl = null;

            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                if (dto.ImageFile.Length > 5 * 1024 * 1024) // 5MB
                {
                    ModelState.AddModelError("ImageFile", "O arquivo de imagem deve ter no máximo 5MB.");
                    return BadRequest(ModelState);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.ImageFile.FileName);
                string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images", "posts");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.ImageFile.CopyToAsync(fileStream);
                }
                imageUrl = $"/images/posts/{uniqueFileName}";
            }

            var newPost = await _postService.CreatePostAsync(dto, imageUrl, sellerId);

            return CreatedAtAction(nameof(GetPostById), new { id = newPost.Id }, newPost);
        }

        [HttpPost("{postId}/like")]
        public async Task<IActionResult> ToggleLike(int postId)
        {
            var userId = GetUserId();
            var result = await _postService.ToggleLikeAsync(postId, userId);
            return Ok(result);
        }

        [HttpPost("{postId}/comments")]
        public async Task<IActionResult> CreateComment(int postId, [FromBody] CreateCommentDto dto)
        {
            var userId = GetUserId();
            var newComment = await _postService.CreateCommentAsync(postId, userId, dto);
            return Ok(newComment);
        }

        [HttpGet("{postId}/comments")]
        public async Task<IActionResult> GetComments(int postId)
        {
            var comments = await _postService.GetCommentsAsync(postId);
            return Ok(comments);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var userId = GetUserId();
            var success = await _postService.DeletePostAsync(id, userId);

            if (!success)
            {
                return Forbid();
            }

            return NoContent();
        }
    }
}