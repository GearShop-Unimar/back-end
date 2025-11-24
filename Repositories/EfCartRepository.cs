using GearShop.Models;
using GearShop.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace GearShop.Repositories
{
    public class EfCartRepository : ICartRepository
    {
        private readonly AppDbContext _context;

        public EfCartRepository(AppDbContext context)
        {
            _context = context;
        }

        // Mudei de Task<Cart> para Task<Cart?>
        public async Task<Cart?> GetCartByUserIdAsync(int userId)
        {
            return await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task CreateCartAsync(Cart cart)
        {
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
        }

        public async Task AddItemAsync(CartItem item)
        {
            if (item.Id > 0)
            {
                _context.CartItems.Update(item);
            }
            else
            {
                _context.CartItems.Add(item);
            }

            await _context.SaveChangesAsync();
        }

        public async Task RemoveItemAsync(int cartItemId)
        {
            var item = await _context.CartItems.FindAsync(cartItemId);
            if (item != null)
            {
                _context.CartItems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ClearCartAsync(int cartId)
        {
            var items = _context.CartItems.Where(i => i.CartId == cartId);
            _context.CartItems.RemoveRange(items);
            await _context.SaveChangesAsync();
        }
    }
}
