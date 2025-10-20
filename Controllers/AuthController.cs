using GearShop.Dtos.Auth;
using GearShop.Services.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
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
            var loginResponse = await _authService.Authenticate(loginData);

            if (loginResponse == null)
            {
                return Unauthorized(new { message = "Email ou senha inválidos." });
            }

            return Ok(loginResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ocorreu um erro inesperado durante a autenticação para o usuário {Email}", loginData.Email);
            return StatusCode(500, "Erro interno do servidor.");
        }
    }
}