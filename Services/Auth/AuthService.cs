// Arquivo: Services/Auth/AuthService.cs
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using GearShop.Dtos;
using GearShop.Models; // Usamos o namespace diretamente
using GearShop.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using BCrypt.Net;

namespace GearShop.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<string> Authenticate(LoginDto loginData)
        {
            var user = await _userRepository.GetByEmailAsync(loginData.Email);

            if (user == null)
            {
                return string.Empty; // Usuário não encontrado
            }

            // Verificação correta da senha com BCrypt
            if (!BCrypt.Net.BCrypt.Verify(loginData.Password, user.PasswordHash))
            {
                return string.Empty; // Senha inválida
            }

            // Se as credenciais estiverem corretas, gerar o Token JWT
            return GenerateJwtToken(user);
        }

        // A assinatura do método agora usa o nome completo da classe User
        private string GenerateJwtToken(GearShop.Models.User user)
        {
            var claims = new[]
            {
                // Todos os 'new Claim' agora funcionam porque os 'usings' estão corretos
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)
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