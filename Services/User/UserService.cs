using GearShop.Dtos;
using GearShop.Dtos.User;
using GearShop.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserModel = GearShop.Models.User;
using GearShop.Models;

namespace GearShop.Services.User
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // --- FUNÇÃO UTILITÁRIA ADICIONADA ---
        private string ConvertProfilePictureToDataUri(byte[]? data, string? mimeType)
        {
            if (data == null || data.Length == 0 || string.IsNullOrEmpty(mimeType))
            {
                return "https://i.imgur.com/V4RclNb.png"; // Placeholder
            }
            string base64String = Convert.ToBase64String(data);
            return $"data:{mimeType};base64,{base64String}";
        }

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
                // ProfilePictureData and MimeType would be set separately if uploaded
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

            var existingUser = await _userRepository.GetByIdAsync(id);
            if (existingUser is null) return null;

            existingUser.Name = dto.Name;
            existingUser.Email = dto.Email;
            existingUser.PhoneNumber = dto.PhoneNumber;
            existingUser.Cpf = dto.Cpf;
            existingUser.Estado = dto.Estado;
            existingUser.Cidade = dto.Cidade;
            existingUser.Cep = dto.Cep;
            existingUser.Rua = dto.Rua;
            existingUser.NumeroCasa = dto.NumeroCasa;

            var updatedUser = await _userRepository.UpdateAsync(id, existingUser);
            return updatedUser is null ? null : ToUserDto(updatedUser);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _userRepository.DeleteAsync(id);
        }

        private UserDto ToUserDto(UserModel user)
        {
            string avatarUri = ConvertProfilePictureToDataUri(user.ProfilePictureData, user.ProfilePictureMimeType);

            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Cidade = user.Cidade,
                Estado = user.Estado,
                Role = user.Role.ToString(),

                // Campos 'required' adicionados
                ProfilePicture = avatarUri,
                Avatar = avatarUri
            };
        }
    }
}
