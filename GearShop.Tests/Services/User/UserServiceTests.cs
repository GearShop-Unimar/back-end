using System.Threading.Tasks;
using FluentAssertions;
using GearShop.Dtos;
using GearShop.Dtos.User;
using GearShop.Models;
using GearShop.Repositories;
using GearShop.Services.User;
using NSubstitute;
using UserModel = GearShop.Models.User;

namespace GearShop.Tests.Services.Users;

public class UserServiceTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly UserService _sut;

    public UserServiceTests()
    {
        _sut = new UserService(_userRepository);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenEmailAlreadyExists()
    {
        var dto = BuildCreateDto();
        _userRepository.EmailExistsAsync(dto.Email).Returns(true);

        await FluentActions
            .Invoking(() => _sut.CreateAsync(dto))
            .Should().ThrowAsync<Exception>()
            .WithMessage("Este endereço de email já está em uso.");
    }

    [Fact]
    public async Task CreateAsync_ShouldHashPassword_AndReturnDto()
    {
        var dto = BuildCreateDto();
        _userRepository.EmailExistsAsync(dto.Email).Returns(false);

        UserModel? capturedUser = null;
        _userRepository.CreateAsync(Arg.Do<UserModel>(user =>
            {
                capturedUser = user;
            }))
            .Returns(call =>
            {
                var user = call.Arg<UserModel>();
                user.Id = 15;
                return user;
            });

        var result = await _sut.CreateAsync(dto);

        await _userRepository.Received(1).CreateAsync(Arg.Any<UserModel>());

        capturedUser.Should().NotBeNull();
        capturedUser!.Name.Should().Be(dto.Name);
        capturedUser.Email.Should().Be(dto.Email);
        capturedUser.Role.Should().Be(Role.Client);
        BCrypt.Net.BCrypt.Verify(dto.Password, capturedUser.PasswordHash).Should().BeTrue();

        result.Id.Should().Be(15);
        result.ProfilePicture.Should().Be("https://i.imgur.com/V4RclNb.png");
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        var dto = BuildUpdateDto();
        _userRepository.EmailExistsAsync(dto.Email, 30).Returns(false);
        _userRepository.GetByIdAsync(30).Returns((UserModel?)null);

        var result = await _sut.UpdateAsync(30, dto);

        result.Should().BeNull();
        await _userRepository.DidNotReceive().UpdateAsync(Arg.Any<int>(), Arg.Any<UserModel>());
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrow_WhenEmailBelongsToAnotherUser()
    {
        var dto = BuildUpdateDto();
        _userRepository.EmailExistsAsync(dto.Email, 11).Returns(true);

        await FluentActions
            .Invoking(() => _sut.UpdateAsync(11, dto))
            .Should().ThrowAsync<Exception>()
            .WithMessage("Este endereço de email já está em uso.");
    }

    private static CreateUserDto BuildCreateDto() => new()
    {
        Name = "Maria",
        Email = "maria@example.com",
        Password = "Secret!23",
        PhoneNumber = "11988887777",
        Cpf = "12345678901",
        Estado = "SP",
        Cidade = "São Paulo",
        Cep = "01001000",
        Rua = "Rua A",
        NumeroCasa = "100"
    };

    private static UpdateUserDto BuildUpdateDto() => new()
    {
        Name = "João",
        Email = "joao@example.com",
        PhoneNumber = "21988887777",
        Cpf = "10987654321",
        Estado = "RJ",
        Cidade = "Rio",
        Cep = "22041001",
        Rua = "Rua B",
        NumeroCasa = "200"
    };
}

