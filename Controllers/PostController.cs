using GearShop.Dtos.Post;
using GearShop.Services;
using Microsoft.AspNetCore.Authorization;
// REMOVED: using Microsoft.AspNetCore.Hosting; // No longer needed here
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO; // Keep for potential future use or remove if unused elsewhere
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
        // REMOVED: IWebHostEnvironment dependency

        // Constructor updated to remove IWebHostEnvironment
        public PostController(IPostService postService)
        {
            _postService = postService;
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

        // --- CreatePost Action Updated ---
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreatePost([FromForm] CreatePostDto dto)
        {
            var authorId = GetUserId(); // Renamed variable

            // REMOVED: All logic related to imageUrl, uniqueFileName, uploadsFolder, filePath, FileStream

            try
            {
                // Call the service method that now expects only DTO and authorId
                // The service (PostService) will handle reading the IFormFile from the DTO
                var newPost = await _postService.CreatePostAsync(dto, authorId);
                return CreatedAtAction(nameof(GetPostById), new { id = newPost.Id }, newPost);
            }
            catch (ArgumentException ex) // Catch specific errors like file size from the service
            {
                ModelState.AddModelError("ImageFile", ex.Message);
                return BadRequest(ModelState);
            }
            catch (Exception ex) // General error handling
            {
                // Consider logging the exception ex
                Console.WriteLine($"Error creating post: {ex.Message}"); // Simple console log
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um erro ao criar o post.");
            }
        }
        // --- End CreatePost Update ---


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