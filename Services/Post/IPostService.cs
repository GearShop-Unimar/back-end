using GearShop.Dtos.Post;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http; // Maybe needed if passing IFormFile

namespace GearShop.Services
{
    public interface IPostService
    {
        Task<IEnumerable<PostDto>> GetFeedAsync(int currentUserId);
        Task<PostDto?> GetPostByIdAsync(int postId, int currentUserId);
        Task<(byte[]? data, string? mimeType)> GetImageAsync(int postId);
        Task<PostDto> CreatePostAsync(CreatePostDto dto, int authorId);
        Task<ToggleLikeResultDto> ToggleLikeAsync(int postId, int userId);
        Task<CommentDto> CreateCommentAsync(int postId, int userId, CreateCommentDto dto);
        Task<IEnumerable<CommentDto>> GetCommentsAsync(int postId);
        Task<bool> DeletePostAsync(int postId, int currentUserId);
    }
}
