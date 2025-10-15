using GearShop.Dtos; // Supondo que seus DTOs estão aqui
using GearShop.Dtos.User; // Supondo que seus DTOs estão aqui
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GearShop.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<UserDto?> GetByIdAsync(int id);
        Task<UserDto> CreateAsync(CreateUserDto dto);
        Task<UserDto?> UpdateAsync(int id, UpdateUserDto dto);
        Task<bool> DeleteAsync(int id);
    }
}