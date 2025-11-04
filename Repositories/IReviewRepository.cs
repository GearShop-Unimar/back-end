using GearShop.Models;

namespace GearShop.Repositories
{
    public interface IReviewRepository
    {
        Task AddAsync(ProductReview review);
        Task<ProductReview?> GetByIdAsync(int id);
        Task<IEnumerable<ProductReview>> GetByProductIdAsync(int productId);
        // (Podes adicionar métodos de Update/Delete depois, se necessário)
    }
}
