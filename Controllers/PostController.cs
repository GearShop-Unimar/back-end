using System.Security.Claims;
using GearShop.Dtos.Post;
using GearShop.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GearShop.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/posts")]
    public class PostController : ControllerBase
    {
        private readonly IPostService postService;

        public PostController(IPostService postService)
        {
            this.postService = postService;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                throw new ApplicationException("ID do utilizador não encontrado no token.");
            return userId;
        }

        [HttpGet]
        public async Task<IActionResult> GetFeed()
        {
            var userId = GetUserId();
            var posts = await postService.GetFeedAsync(userId);
            return Ok(posts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPostById(int id)
        {
            var userId = GetUserId();
            var post = await postService.GetPostByIdAsync(id, userId);
            if (post == null) return NotFound();
            return Ok(post);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreatePost([FromForm] CreatePostDto dto)
        {
            var authorId = GetUserId();

            try
            {
                var newPost = await postService.CreatePostAsync(dto, authorId);
                return CreatedAtAction(nameof(GetPostById), new { id = newPost.Id }, newPost);
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("ImageFile", ex.Message);
                return BadRequest(ModelState);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um erro ao criar o post.");
            }
        }

        [HttpPost("{postId}/like")]
        public async Task<IActionResult> ToggleLike(int postId)
        {
            var userId = GetUserId();
            var result = await postService.ToggleLikeAsync(postId, userId);
            return Ok(result);
        }

        [HttpPost("{postId}/comments")]
        public async Task<IActionResult> CreateComment(int postId, [FromBody] CreateCommentDto dto)
        {
            var userId = GetUserId();
            var newComment = await postService.CreateCommentAsync(postId, userId, dto);
            return Ok(newComment);
        }

        [HttpGet("{postId}/comments")]
        public async Task<IActionResult> GetComments(int postId)
        {
            var comments = await postService.GetCommentsAsync(postId);
            return Ok(comments);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var userId = GetUserId();
            var success = await postService.DeletePostAsync(id, userId);
            if (!success) return Forbid();
            return NoContent();
        }
    }
}
