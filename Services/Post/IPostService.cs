
using GearShop.Dtos.Post;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GearShop.Services
{
    public interface IPostService
    {
        // Obtém o feed principal
        Task<IEnumerable<PostDto>> GetFeedAsync(int currentUserId);

        // Obtém um post específico
        Task<PostDto?> GetPostByIdAsync(int postId, int currentUserId);

        // Cria um novo post
        Task<PostDto> CreatePostAsync(CreatePostDto dto, string? imageUrl, int authorId);

        // Alterna (like/unlike) um post
        Task<ToggleLikeResultDto> ToggleLikeAsync(int postId, int userId);

        // Cria um comentário
        Task<CommentDto> CreateCommentAsync(int postId, int userId, CreateCommentDto dto);

        // Obtém todos os comentários de um post
        Task<IEnumerable<CommentDto>> GetCommentsAsync(int postId);

        // Apaga um post (se for o dono)
        Task<bool> DeletePostAsync(int postId, int currentUserId);
    }
}