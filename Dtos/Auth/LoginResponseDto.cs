using GearShop.Dtos.User;

namespace GearShop.Dtos.Auth
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public required UserDto User { get; set; }
    }
}