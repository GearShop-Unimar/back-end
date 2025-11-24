using GearShop.Models;
using GearShop.Repositories;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace GearShop.Services.CartService
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;

        public CartService(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task<Cart> GetCartAsync(int userId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);

            // Se o utilizador ainda não tiver carrinho, criamos um vazio agora
            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                await _cartRepository.CreateCartAsync(cart);
            }

            return cart;
        }

        public async Task AddItemToCartAsync(int userId, int productId, int quantity)
        {
            // 1. Obtemos o carrinho
            var cart = await GetCartAsync(userId);

            // 2. Verificamos se o produto já existe no carrinho
            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);

            if (existingItem != null)
            {
                // Se já existe, apenas aumentamos a quantidade
                existingItem.Quantity += quantity;
                // Aqui poderíamos chamar um método de update no repositório, 
                // mas como o EF segue as mudanças, basta salvar se o contexto estiver partilhado.
                // Para simplificar e garantir, vamos assumir que o repo trata da persistência.
                // Nota: Num cenário real, poderias precisar de um método UpdateItemAsync.
                await _cartRepository.AddItemAsync(existingItem); // Adaptação simples
            }
            else
            {
                // Se não existe, criamos um novo item
                var newItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = productId,
                    Quantity = quantity
                };
                await _cartRepository.AddItemAsync(newItem);
            }
        }

        public async Task RemoveItemFromCartAsync(int cartItemId)
        {
            await _cartRepository.RemoveItemAsync(cartItemId);
        }
    }
}
