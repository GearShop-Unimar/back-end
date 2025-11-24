using GearShop.Services.CartService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GearShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // GET: api/cart
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            // Obtém o ID do utilizador logado através do Token
            var userId = GetCurrentUserId();

            var cart = await _cartService.GetCartAsync(userId);
            return Ok(cart);
        }

        // POST: api/cart/add
        [HttpPost("add")]
        public async Task<IActionResult> AddItem([FromBody] AddToCartRequest request)
        {
            var userId = GetCurrentUserId();

            await _cartService.AddItemToCartAsync(userId, request.ProductId, request.Quantity);

            return Ok(new { message = "Item adicionado ao carrinho com sucesso!" });
        }

        // DELETE: api/cart/item/5
        [HttpDelete("item/{itemId}")]
        public async Task<IActionResult> RemoveItem(int itemId)
        {
            await _cartService.RemoveItemFromCartAsync(itemId);
            return Ok(new { message = "Item removido." });
        }

        // --- Método Auxiliar para ler o ID do Token ---
        private int GetCurrentUserId()
        {
            // Procura a Claim que tem o ID (geralmente ClaimTypes.NameIdentifier ou "id")
            // Nota: Dependendo de como geraste o token no AuthService, a chave pode variar.
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("id");

            if (idClaim != null && int.TryParse(idClaim.Value, out int userId))
            {
                return userId;
            }

            // Se não encontrar, lança erro (ou retorna 0, dependendo da tua lógica)
            throw new System.Exception("Utilizador não identificado no Token.");
        }
    }

    // Classe simples para receber os dados do POST (DTO)
    public class AddToCartRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
