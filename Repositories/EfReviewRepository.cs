using GearShop.Data;
using GearShop.Models;
using Microsoft.EntityFrameworkCore;

namespace GearShop.Repositories
{
    public class EfReviewRepository : IReviewRepository
    {
        private readonly AppDbContext _context;

        public EfReviewRepository(AppDbContext context)
        {
            this._context = context;
        }

        public async Task AddAsync(ProductReview review)
        {
            await this._context.ProductReviews.AddAsync(review);
            await this._context.SaveChangesAsync();
        }

        public async Task<ProductReview?> GetByIdAsync(int id)
        {
            return await this._context.ProductReviews
                .Include(r => r.Author) // Inclui os dados do autor
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<ProductReview>> GetByProductIdAsync(int productId)
        {
            return await this._context.ProductReviews
                .Where(r => r.ProductId == productId)
                .Include(r => r.Author) // Inclui os dados do autor
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }
    }
}
