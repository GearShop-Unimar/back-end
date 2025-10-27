using GearShop.Data;
using GearShop.Dtos.Post;
using GearShop.Dtos.User;
using GearShop.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO; // Needed for MemoryStream
using System.Linq;
using System.Threading.Tasks;

namespace GearShop.Services
{
    public class PostService : IPostService
    {
        private readonly AppDbContext _context;

        public PostService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PostDto>> GetFeedAsync(int currentUserId)
        {
            var posts = await _context.Posts
                .Include(p => p.Author) // Include Author for mapping
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                    .ThenInclude(c => c.Author)
                .OrderByDescending(p => p.CreatedAt)
                .Take(20)
                .ToListAsync();

            return posts.Select(post => MapPostToDto(post, currentUserId)).ToList();
        }

        public async Task<PostDto?> GetPostByIdAsync(int postId, int currentUserId)
        {
            var post = await _context.Posts
                .Include(p => p.Author)
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                    .ThenInclude(c => c.Author)
                .FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null) return null;

            return MapPostToDto(post, currentUserId);
        }

        // --- CreatePostAsync MODIFIED ---
        public async Task<PostDto> CreatePostAsync(CreatePostDto dto, int authorId)
        {
            byte[]? imageData = null;
            string? imageMimeType = null;

            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                // Read image into byte array
                using (var memoryStream = new MemoryStream())
                {
                    await dto.ImageFile.CopyToAsync(memoryStream);
                    if (memoryStream.Length < 5 * 1024 * 1024) // Check size (e.g., 5MB)
                    {
                        imageData = memoryStream.ToArray();
                        imageMimeType = dto.ImageFile.ContentType; // Get MIME type
                    }
                    else
                    {
                        // Handle error: file too large
                        throw new ArgumentException("Image file size exceeds the limit.");
                    }
                }
            }

            var post = new Post
            {
                Content = dto.Content,
                ImageData = imageData, // Save bytes
                ImageMimeType = imageMimeType, // Save MIME type
                UserId = authorId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            await _context.Entry(post).Reference(p => p.Author).LoadAsync();

            return MapPostToDto(post, authorId);
        }
        // --- End CreatePostAsync ---


        public async Task<ToggleLikeResultDto> ToggleLikeAsync(int postId, int userId)
        {
            var existingLike = await _context.PostLikes
                .FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId);

            if (existingLike != null)
            {
                _context.PostLikes.Remove(existingLike);
            }
            else
            {
                var newLike = new PostLike
                {
                    PostId = postId,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };
                _context.PostLikes.Add(newLike);
            }

            await _context.SaveChangesAsync();

            var newLikeCount = await _context.PostLikes.CountAsync(l => l.PostId == postId);
            return new ToggleLikeResultDto
            {
                NewLikeCount = newLikeCount,
                IsLikedByCurrentUser = (existingLike == null)
            };
        }

        public async Task<CommentDto> CreateCommentAsync(int postId, int userId, CreateCommentDto dto)
        {
            var newComment = new Comment
            {
                Content = dto.Content,
                PostId = postId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Comments.Add(newComment);
            await _context.SaveChangesAsync();

            await _context.Entry(newComment).Reference(c => c.Author).LoadAsync();

            return MapCommentToDto(newComment);
        }

        public async Task<IEnumerable<CommentDto>> GetCommentsAsync(int postId)
        {
            var comments = await _context.Comments
                .Include(c => c.Author)
                .Where(c => c.PostId == postId)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();

            return comments.Select(MapCommentToDto).ToList();
        }

        public async Task<bool> DeletePostAsync(int postId, int currentUserId)
        {
            var post = await _context.Posts.FindAsync(postId);
            if (post == null) return false;

            if (post.UserId != currentUserId)
            {
                return false;
            }

            // Note: If using external blob storage, you'd delete the blob here.
            // For DB blobs, Cascade Delete might handle this, or you might need manual cleanup if required.

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return true;
        }

        private PostDto MapPostToDto(Post post, int currentUserId)
        {
            return new PostDto
            {
                Id = post.Id,
                Content = post.Content,
                // REMOVED: ImageUrl = post.ImageUrl,
                CreatedAt = post.CreatedAt,
                Author = MapAuthorToDto(post.Author),
                LikeCount = post.Likes.Count,
                CommentCount = post.Comments.Count,
                IsLikedByCurrentUser = post.Likes.Any(l => l.UserId == currentUserId),
                // Note: RecentComments won't have images unless CommentDto is also changed
                RecentComments = post.Comments
                    .OrderByDescending(c => c.CreatedAt)
                    .Take(2)
                    .Select(MapCommentToDto)
                    .ToList()
            };
        }

        private CommentDto MapCommentToDto(Comment comment)
        {
            return new CommentDto
            {
                Id = comment.Id,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                Author = MapAuthorToDto(comment.Author)
            };
        }

        private AuthorDto MapAuthorToDto(GearShop.Models.User? author)
        {
            if (author == null)
            {
                return new AuthorDto { Id = 0, Name = "Utilizador Desconhecido" };
            }

            return new AuthorDto
            {
                Id = author.Id,
                Name = author.Name,
                // REMOVED: AvatarUrl = author.ProfilePicture
                // The frontend will get the avatar via /api/images/user/{author.Id}
            };
        }

        // --- INTERFACE NEEDS UPDATE ---
        // Update IPostService to match the new CreatePostAsync signature
        Task<PostDto> IPostService.CreatePostAsync(CreatePostDto dto, string? imageUrl, int authorId)
        {
            // This implementation matches the OLD signature, needs to call the NEW one
            if (imageUrl != null)
            {
            }
            return CreatePostAsync(dto, authorId); // Call the correct overload
        }
    }
}