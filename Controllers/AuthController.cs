
using GearShop.Dtos;
using GearShop.Dtos.Auth; // 
using GearShop.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto loginData)
    {
        try
        {
            var token = await _authService.Authenticate(loginData);

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { message = "Email ou senha inválidos." });
            }

            return Ok(new LoginResponseDto { Token = token });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ocorreu um erro inesperado durante a autenticação para o usuário {Email}", loginData.Email);
            return StatusCode(500, "Erro interno do servidor.");
        }
    }
}