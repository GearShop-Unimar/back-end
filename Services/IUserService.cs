using GearShop.Dtos;
using GearShop.Dtos.User;

namespace GearShop.Services
{
    public interface IUserService
    {
        IEnumerable<UserDto> GetAll();
        UserDto? GetById(int id);
        UserDto Create(CreateUserDto dto);
        UserDto? Update(int id, UpdateUserDto dto);
        bool Delete(int id);
        UserDto? Authenticate(string email, string password);
    }
}