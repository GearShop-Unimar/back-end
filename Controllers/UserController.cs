using Microsoft.AspNetCore.Mvc;
using GearShop.Models;

namespace GearShop.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class UsersController : ControllerBase
	{
		// In-memory "database" (static list for testing)
		private static List<User> users = new List<User>
		{
			new User { Id = 1, Name = "Gabriel", Email = "gabriel@email.com" },
			new User { Id = 2, Name = "Maria", Email = "maria@email.com" }
		};

		// GET api/users
		[HttpGet]
		public IActionResult GetAll()
		{
			return Ok(users);
		}

		// GET api/users/{id}
		[HttpGet("{id}")]
		public IActionResult GetById(int id)
		{
			var user = users.FirstOrDefault(u => u.Id == id);
			if (user == null)
				return NotFound(new { message = "User not found" });

			return Ok(user);
		}

		// POST api/users
		[HttpPost]
		public IActionResult Create([FromBody] User newUser)
		{
			newUser.Id = users.Max(u => u.Id) + 1;
			users.Add(newUser);

			return CreatedAtAction(nameof(GetById), new { id = newUser.Id }, newUser);
		}

		// PUT api/users/{id}
		[HttpPut("{id}")]
		public IActionResult Update(int id, [FromBody] User updatedUser)
		{
			var user = users.FirstOrDefault(u => u.Id == id);
			if (user == null)
				return NotFound(new { message = "User not found" });

			user.Name = updatedUser.Name;
			user.Email = updatedUser.Email;

			return Ok(user);
		}

		// DELETE api/users/{id}
		[HttpDelete("{id}")]
		public IActionResult Delete(int id)
		{
			var user = users.FirstOrDefault(u => u.Id == id);
			if (user == null)
				return NotFound(new { message = "User not found" });

			users.Remove(user);
			return NoContent();
		}
	}
}