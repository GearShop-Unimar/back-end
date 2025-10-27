using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using GearShop.Models;
using GearShop.Dtos; // Assuming CreateUserDto and UpdateUserDto are here
using GearShop.Repositories;
using BCrypt.Net;

namespace GearShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _repository;

        public UserController(IUserRepository repository) => _repository = repository;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAll()
        {
            var items = await _repository.GetAllAsync();
            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<User>> GetById(int id)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user is null) return NotFound(new { message = "Usuário não encontrado" });
            return Ok(user);
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