using System.Threading.Tasks;
using FluentAssertions;
using GearShop.Configuration;
using GearShop.Dtos.Auth;
using GearShop.Models;
using GearShop.Repositories;
using GearShop.Services.Auth;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace GearShop.Tests.Services.Auth;

public class AuthServiceTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        var jwtOptions = Options.Create(new JwtOptions
        {
            Key = "supersecretkey_supersecretkey_super"
        });

        _sut = new AuthService(_userRepository, jwtOptions);
    }

    [Fact]
    public async Task Authenticate_ShouldReturnNull_WhenUserDoesNotExist()
    {
        var loginDto = new LoginDto { Email = "user@example.com", Password = "123456" };
        _userRepository.GetByEmailAsync(loginDto.Email).Returns((User?)null);

        var result = await _sut.Authenticate(loginDto);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Authenticate_ShouldReturnResponse_WhenCredentialsAreValid()
    {
        var loginDto = new LoginDto { Email = "client@example.com", Password = "MyPass123" };
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(loginDto.Password);

        var user = new User
        {
            Id = 5,
            Name = "Valid User",
            Email = loginDto.Email,
            PasswordHash = hashedPassword,
            PhoneNumber = "11999999999",
            Cpf = "12345678901",
            Estado = "SP",
            Cidade = "Sao Paulo",
            Cep = "12345678",
            Rua = "Rua Alfa",
            NumeroCasa = "123",
            Role = Role.Client
        };

        _userRepository.GetByEmailAsync(loginDto.Email).Returns(user);

        var result = await _sut.Authenticate(loginDto);

        result.Should().NotBeNull();
        result!.Token.Should().NotBeNullOrWhiteSpace();
        result.User.Email.Should().Be(loginDto.Email);
        result.User.ProfilePicture.Should().Be("https://i.imgur.com/V4RclNb.png");
        result.User.Avatar.Should().Be("https://i.imgur.com/V4RclNb.png");
        result.User.Role.Should().Be(Role.Client.ToString());
    }
}

