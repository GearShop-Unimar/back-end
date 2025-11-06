using GearShop.Dtos.Auth;
using GearShop.Dtos.User;
using GearShop.Repositories;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using GearShop.Configuration;

namespace GearShop.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtOptions _jwtOptions;

        public AuthService(IUserRepository userRepository, IOptions<JwtOptions> jwtOptions)
        {
            _userRepository = userRepository;
            _jwtOptions = jwtOptions.Value;
        }

        private string ConvertProfilePictureToDataUri(byte[]? data, string? mimeType)
        {
            if (data == null || data.Length == 0 || string.IsNullOrEmpty(mimeType))
            {
                return "https://i.imgur.com/V4RclNb.png";
            }
            string base64String = Convert.ToBase64String(data);
            return $"data:{mimeType};base64,{base64String}";
        }

        public async Task<LoginResponseDto?> Authenticate(LoginDto loginData)
        {
            var user = await _userRepository.GetByEmailAsync(loginData.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginData.Password, user.PasswordHash))
            {
                return null;
            }

            var tokenString = GenerateJwtToken(user);

            // --- CORREÇÃO APLICADA AQUI ---
            string avatarUri = ConvertProfilePictureToDataUri(user.ProfilePictureData, user.ProfilePictureMimeType);

            var userDto = new UserDto
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

            return new LoginResponseDto
            {
                Token = tokenString,
                User = userDto
            };
        }

        private string GenerateJwtToken(GearShop.Models.User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var secretKey = _jwtOptions.Key;

            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("A chave JWT está vazia. Verifique a configuração em appsettings.json (Jwt:Key).");
            }

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(secretKey)
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(8),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
