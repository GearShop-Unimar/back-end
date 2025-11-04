using GearShop.Dtos.Product;
using GearShop.Dtos.User;
using GearShop.Models;
using GearShop.Repositories;
using System.Security.Claims;
using System.Linq;
using Microsoft.EntityFrameworkCore; // Para o DbUpdateException

namespace GearShop.Services.Review
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IProductRepository _productRepository; // Para verificar se o produto existe
        // private readonly IOrderRepository _orderRepository; // *** (Ver nota abaixo)

        public ReviewService(
            IReviewRepository reviewRepository,
            IProductRepository productRepository
            // IOrderRepository orderRepository // Descomentar se fores implementar a regra de "só quem comprou"
            )
        {
            this._reviewRepository = reviewRepository;
            this._productRepository = productRepository;
            // this._orderRepository = orderRepository;
        }

        public async Task<ReviewDto> CreateReviewAsync(CreateReviewDto createDto, ClaimsPrincipal user)
        {
            // 1. Obter o ID do utilizador a partir do token JWT
            var authorIdStr = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(authorIdStr))
            {
                throw new UnauthorizedAccessException("Utilizador não autenticado.");
            }
            var authorId = int.Parse(authorIdStr);

            // 2. Verificar se o produto existe
            var product = await this._productRepository.GetByIdAsync(createDto.ProductId);
            if (product == null)
            {
                throw new KeyNotFoundException("Produto não encontrado.");
            }

            // 3. *** REGRA DE NEGÓCIO (Opcional) ***
            // Se quiseres que SÓ quem comprou o produto possa avaliar,
            // aqui é onde verificarias. Precisarias de injetar o IOrderRepository.
            // Ex: var hasPurchased = await this._orderRepository.UserHasPurchasedProductAsync(authorId, createDto.ProductId);
            // if (!hasPurchased)
            // {
            //    throw new InvalidOperationException("Só podes avaliar produtos que compraste.");
            // }

            // 4. Criar a nova entidade de Review
            var newReview = new ProductReview
            {
                Rating = createDto.Rating,
                Comment = createDto.Comment,
                ProductId = createDto.ProductId,
                AuthorId = authorId,
                CreatedAt = DateTime.UtcNow
            };

            // 5. Adicionar ao banco
            try
            {
                await this._reviewRepository.AddAsync(newReview);
            }
            catch (DbUpdateException) // Apanha a violação do [Index] (tentativa de avaliar duas vezes)
            {
                throw new InvalidOperationException("Já avaliaste este produto.");
            }

            // 6. Retornar o DTO
            var authorName = user.FindFirstValue(ClaimTypes.Name) ?? "Utilizador";

            return new ReviewDto
            {
                Id = newReview.Id,
                Rating = newReview.Rating,
                Comment = newReview.Comment,
                CreatedAt = newReview.CreatedAt,
                ProductId = newReview.ProductId,
                Author = new AuthorDto
                {
                    Id = authorId,
                    Name = authorName
                }
            };
        }

        public async Task<IEnumerable<ReviewDto>> GetReviewsForProductAsync(int productId)
        {
            var reviews = await this._reviewRepository.GetByProductIdAsync(productId);

            // Mapear a lista de Model (ProductReview) para DTO (ReviewDto)
            return reviews.Select(review => new ReviewDto
            {
                Id = review.Id,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt,
                ProductId = review.ProductId,
                Author = new AuthorDto
                {
                    Id = review.Author.Id,
                    Name = review.Author.Name
                }
            });
        }
    }
}
