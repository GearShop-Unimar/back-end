using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using GearShop.Models;
using GearShop.Dtos;
using GearShop.Repositories;
using GearShop.Repositories.Factories;

namespace GearShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class UsersController : ControllerBase
    {
        private readonly UserRepositoryCreator _creator;
        public UsersController(UserRepositoryCreator creator) => _creator = creator;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAll()
        {
            var repo = _creator.CreateRepository();
            var items = await repo.GetAllAsync();
            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<User>> GetById(int id)
        {
            var repo = _creator.CreateRepository();
            var user = await repo.GetByIdAsync(id);
            if (user is null) return NotFound(new { message = "User not found" });
            return Ok(user);
        }

        [HttpPost]
        [Consumes("application/json")]
        public async Task<ActionResult<User>> Create([FromBody] CreateUserDto dto)
        {
            var repo = _creator.CreateRepository();
            var email = dto.Email.Trim().ToLowerInvariant();

            if (await repo.EmailExistsAsync(email))
                return Conflict(new { message = "Email already in use" });

            var user = new User
            {
                Name = dto.Name.Trim(),
                Email = email,
                phoneNumber = dto.phoneNumber.Trim(),
                profilePicture = dto.profilePicture.Trim()
            };

            var created = await repo.CreateAsync(user);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        [Consumes("application/json")]
        public async Task<ActionResult<User>> Update(int id, [FromBody] UpdateUserDto dto)
        {
            var repo = _creator.CreateRepository();
            var email = dto.Email.Trim().ToLowerInvariant();

            if (await repo.EmailExistsAsync(email, id))
                return Conflict(new { message = "Email already in use" });

            var updated = await repo.UpdateAsync(id, new User
            {
                Id = id,
                Name = dto.Name.Trim(),
                Email = email,
                phoneNumber = dto.phoneNumber.Trim(),
                profilePicture = dto.profilePicture.Trim()
            });

            if (updated is null) return NotFound(new { message = "User not found" });
            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var repo = _creator.CreateRepository();
            var ok = await repo.DeleteAsync(id);
            return ok ? NoContent() : NotFound(new { message = "User not found" });
        }
    }
}
