// Services/IAuthService.cs
using GearShop.Dtos;

namespace GearShop.Services
{
    public interface IAuthService
    {
        // Retorna o token JWT se o login for bem-sucedido
        Task<string> Authenticate(LoginDto loginData);
    }
}