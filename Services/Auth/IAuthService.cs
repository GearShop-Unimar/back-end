using GearShop.Dtos.Auth;
using System.Threading.Tasks;

namespace GearShop.Services.Auth
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> Authenticate(LoginDto loginData);
    }
}