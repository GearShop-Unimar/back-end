using GearShop.Dtos.Auth;
using GearShop.Dtos.User;
using GearShop.Repositories;
using Microsoft.Extensions.Configuration; // Ainda é necessário se outras classes o usarem, mas não mais para a JWT Key
using Microsoft.Extensions.Options; // NOVO: Para usar o padrão IOptions<T>
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

// Nota: Certifique-se que o namespace da sua classe JwtOptions está aqui ou use o caminho completo.
using GearShop.Configuration;

namespace GearShop.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        // SUBSTITUÍDO: Removemos a dependência direta do IConfiguration para a chave JWT
        // private readonly IConfiguration _configuration; 
        private readonly JwtOptions _jwtOptions; // NOVO: Campo tipado para as opções JWT

        // CONSTRUTOR MODIFICADO: Recebe IOptions<JwtOptions>
        public AuthService(IUserRepository userRepository, IOptions<JwtOptions> jwtOptions)
        {
            _userRepository = userRepository;
            _jwtOptions = jwtOptions.Value; // Acesso ao objeto de configuração real
        }

        // Se precisar do IConfiguration para outras coisas, pode mantê-lo, mas neste exemplo foi removido.
        // Se o IConfiguration não for mais usado nesta classe, remova-o do construtor.
        // public AuthService(IUserRepository userRepository, IConfiguration configuration) { ... }

        public async Task<LoginResponseDto?> Authenticate(LoginDto loginData)
        {
            var user = await _userRepository.GetByEmailAsync(loginData.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginData.Password, user.PasswordHash))
            {
                return null;
            }

            var tokenString = GenerateJwtToken(user);

            var userDto = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                ProfilePicture = user.ProfilePicture,
                Cidade = user.Cidade,
                Estado = user.Estado,
                Role = user.Role.ToString()
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

            // LEITURA DA CHAVE AGORA SEGURA E TIPADA: Resolve o problema do SonarQube
            var secretKey = _jwtOptions.Key;

            if (string.IsNullOrEmpty(secretKey))
            {
                // Garante que a aplicação falha se a chave não estiver configurada
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