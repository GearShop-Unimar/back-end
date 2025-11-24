using GearShop.Models;
using System.Threading.Tasks;

namespace GearShop.Services.CartService
{
    public interface ICartService
    {
        // Retorna o carrinho do utilizador
        Task<Cart> GetCartAsync(int userId);

        // Adiciona um produto ao carrinho
        Task AddItemToCartAsync(int userId, int productId, int quantity);

        // Remove um item
        Task RemoveItemFromCartAsync(int cartItemId);
    }
}
