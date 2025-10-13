using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using GearShop.Models;
using GearShop.Dtos;
using GearShop.Repositories;
using BCrypt.Net;

namespace GearShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        // Injeção da interface do repositório
        private readonly IUserRepository _repository;

        // Construtor para injetar IUserRepository
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

            // Verifica se o email já está em uso
            if (await _repository.EmailExistsAsync(email))
                return Conflict(new { message = "Email já está em uso" });

            // Hash da senha
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            // Cria o objeto User mapeando DTOs
            var user = new User
            {
                Name = dto.Name.Trim(),
                Email = email,
                PasswordHash = passwordHash,
                PhoneNumber = dto.PhoneNumber.Trim(),
                ProfilePicture = dto.ProfilePicture.Trim(),
                Cpf = dto.Cpf.Trim(),
                Estado = dto.Estado.Trim(),
                Cidade = dto.Cidade.Trim(),
                Cep = dto.Cep.Trim(),
                Rua = dto.Rua.Trim(),
                NumeroCasa = dto.NumeroCasa.Trim()
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

            var updated = await _repository.UpdateAsync(id, new User
            {
                Id = id,
                Name = dto.Name.Trim(),
                Email = email,
                PhoneNumber = dto.PhoneNumber.Trim(),
                ProfilePicture = dto.ProfilePicture.Trim(),
                Cpf = dto.Cpf.Trim(),
                Estado = dto.Estado.Trim(),
                Cidade = dto.Cidade.Trim(),
                Cep = dto.Cep.Trim(),
                Rua = dto.Rua.Trim(),
                NumeroCasa = dto.NumeroCasa.Trim()
            });

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