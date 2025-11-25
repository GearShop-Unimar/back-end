using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using GearShop.Models;
using GearShop.Dtos;
using GearShop.Dtos.User; // Adicionado para UserDto
using GearShop.Repositories;
using GearShop.Services.Premium; // Adicionado para IPremiumAccountService
using GearShop.Models.Enums; // Adicionado para SubscriptionStatus
using BCrypt.Net;

namespace GearShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _repository;
        private readonly IPremiumAccountService _premiumAccountService; // Adicionado

        public UserController(IUserRepository repository, IPremiumAccountService premiumAccountService) // Modificado
        {
            _repository = repository;
            _premiumAccountService = premiumAccountService; // Adicionado
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
        {
            var users = await _repository.GetAllAsync();
            var userDtos = new List<UserDto>();
            foreach (var user in users)
            {
                userDtos.Add(new UserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    ProfilePicture = user.ProfilePictureMimeType ?? string.Empty, // Ou a URL/Base64 da imagem
                    Cidade = user.Cidade,
                    Estado = user.Estado,
                    Role = user.Role.ToString(),
                    Avatar = user.ProfilePictureMimeType ?? string.Empty, // ou a URL/Base64 da imagem
                    IsPremium = user.PremiumAccount != null
                });
        }
            return Ok(userDtos);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserDto>> GetById(int id)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user is null) return NotFound(new { message = "Usuário não encontrado" });

            var userDto = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                ProfilePicture = user.ProfilePictureMimeType ?? string.Empty, // Ou a URL/Base64 da imagem
                Cidade = user.Cidade,
                Estado = user.Estado,
                Role = user.Role.ToString(),
                Avatar = user.ProfilePictureMimeType ?? string.Empty, // ou a URL/Base64 da imagem
                IsPremium = user.PremiumAccount != null
            };
            return Ok(userDto);
        }

        [HttpPost]
        [Consumes("application/json")]
        public async Task<ActionResult<User>> Create([FromBody] CreateUserDto dto)
        {
            var email = dto.Email.Trim().ToLowerInvariant();

            if (await _repository.EmailExistsAsync(email))
                return Conflict(new { message = "Email já está em uso" });

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new User
            {
                Name = dto.Name.Trim(),
                Email = email,
                PasswordHash = passwordHash,
                PhoneNumber = dto.PhoneNumber.Trim(),
                // ProfilePictureData/MimeType would be handled separately
                Cpf = dto.Cpf.Trim(),
                Estado = dto.Estado.Trim(),
                Cidade = dto.Cidade.Trim(),
                Cep = dto.Cep.Trim(),
                Rua = dto.Rua.Trim(),
                NumeroCasa = dto.NumeroCasa.Trim(),
                Role = Role.Client // Assuming default role is Client
            };

            var created = await _repository.CreateAsync(user);

            if (dto.IsPremium)
            {
                await _premiumAccountService.ActivatePremiumAsync(created.Id, 30, 15.99m); // 30 dias de duração e preço fixo
            }

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        [Consumes("application/json")]
        public async Task<ActionResult<User>> Update(int id, [FromBody] UpdateUserDto dto)
        {
            var email = dto.Email.Trim().ToLowerInvariant();

            if (await _repository.EmailExistsAsync(email, id))
                return Conflict(new { message = "Email já está em uso" });

            // Pass DTO data to repository for update logic
            // Note: Repository needs logic to handle potential null User returned from UpdateAsync
            var userToUpdate = new User // Create a temporary User object with updated data
            {
                // Do NOT set Id here if UpdateAsync finds user by id param
                Name = dto.Name.Trim(),
                Email = email,
                PhoneNumber = dto.PhoneNumber.Trim(),
                // ProfilePictureData/MimeType update would be handled if dto included image data
                Cpf = dto.Cpf.Trim(),
                Estado = dto.Estado.Trim(),
                Cidade = dto.Cidade.Trim(),
                Cep = dto.Cep.Trim(),
                Rua = dto.Rua.Trim(),
                NumeroCasa = dto.NumeroCasa.Trim()
                // Do not update PasswordHash or Role here unless intended
            };


            var updated = await _repository.UpdateAsync(id, userToUpdate);

            if (updated is null) return NotFound(new { message = "Usuário não encontrado" });

            // Lógica para atualizar status premium
            if (dto.IsPremium && updated.PremiumAccount == null)
            {
                await _premiumAccountService.ActivatePremiumAsync(updated.Id, 30, 15.99m); // 30 dias de duração e preço fixo
            }
            else if (!dto.IsPremium && updated.PremiumAccount != null)
            {
                await _premiumAccountService.CancelPremiumAsync(updated.Id);
            }

            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _repository.DeleteAsync(id);
            return ok ? NoContent() : NotFound(new { message = "Usuário não encontrado" });
        }
    }
}
