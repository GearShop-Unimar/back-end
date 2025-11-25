using GearShop.Models;
using System.Threading.Tasks;

namespace GearShop.Repositories
{
    public interface ICartRepository
    {
        // Mudei de Task<Cart> para Task<Cart?>
        Task<Cart?> GetCartByUserIdAsync(int userId, bool includePremiumAccount = false);

        Task CreateCartAsync(Cart cart);
        Task AddItemAsync(CartItem item);
        Task RemoveItemAsync(int cartItemId);
        Task ClearCartAsync(int cartId);
    }
}
