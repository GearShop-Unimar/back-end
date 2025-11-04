using GearShop.Dtos.Product;
using System.Security.Claims; // Para o ClaimsPrincipal

namespace GearShop.Services.Review
{
    public interface IReviewService
    {
        // Passamos o ClaimsPrincipal para saber QUEM (utilizador autenticado) está a criar
        Task<ReviewDto> CreateReviewAsync(CreateReviewDto createDto, ClaimsPrincipal user);

        Task<IEnumerable<ReviewDto>> GetReviewsForProductAsync(int productId);
    }
}
