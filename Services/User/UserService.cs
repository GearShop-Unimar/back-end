// Arquivo: Services/User/UserService.cs

using GearShop.Dtos; // Namespace geral dos DTOs
using GearShop.Dtos.User; // Namespace dos DTOs de User
using GearShop.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
// SOLUÇÃO PARA O ERRO CS0118: Criamos um apelido 'UserModel' para a classe User
using UserModel = GearShop.Models.User;
using GearShop.Models; // Para a enum Role

namespace GearShop.Services.User
{
    // A classe agora implementa a interface IUserService que definimos
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // SOLUÇÃO PARA O ERRO CS0535: Implementação de todos os métodos

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(user => ToUserDto(user));
        }

        public async Task<UserDto?> GetByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user is null ? null : ToUserDto(user);
        }

        public async Task<UserDto> CreateAsync(CreateUserDto dto)
        {
            if (await _userRepository.EmailExistsAsync(dto.Email))
            {
                throw new Exception("Este endereço de email já está em uso.");
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new UserModel
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = passwordHash,
                PhoneNumber = dto.PhoneNumber,
                ProfilePicture = dto.ProfilePicture,
                Cpf = dto.Cpf,
                Estado = dto.Estado,
                Cidade = dto.Cidade,
                Cep = dto.Cep,
                Rua = dto.Rua,
                NumeroCasa = dto.NumeroCasa,
                Role = Role.Client
            };

            var createdUser = await _userRepository.CreateAsync(user);
            return ToUserDto(createdUser);
        }

        public async Task<UserDto?> UpdateAsync(int id, UpdateUserDto dto)
        {
            if (await _userRepository.EmailExistsAsync(dto.Email, id))
            {
                throw new Exception("Este endereço de email já está em uso.");
            }

            var userToUpdate = new UserModel
            {
                Name = dto.Name,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                ProfilePicture = dto.ProfilePicture,
                Cpf = dto.Cpf,
                Estado = dto.Estado,
                Cidade = dto.Cidade,
                Cep = dto.Cep,
                Rua = dto.Rua,
                NumeroCasa = dto.NumeroCasa
            };

            var updatedUser = await _userRepository.UpdateAsync(id, userToUpdate);
            return updatedUser is null ? null : ToUserDto(updatedUser);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _userRepository.DeleteAsync(id);
        }

        // Método privado para mapear o nosso Modelo para um DTO
        private UserDto ToUserDto(UserModel user)
        {
            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                ProfilePicture = user.ProfilePicture,
                Cidade = user.Cidade,
                Estado = user.Estado,
                Role = user.Role.ToString() // Converte a enum para string (ex: "Client")
            };
        }
    }
}