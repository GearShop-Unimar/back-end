using Microsoft.AspNetCore.Mvc;
using GearShop.Models;
using System.Collections.Generic;
using System.Linq;

namespace GearShop.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class UsersController : ControllerBase
	{
	{
		private static List<User> users = new List<User>
		{
			new User
			{
				Id = 1,
				Name = "Gabriel",
				Email = "gabriel@email.com",
				Phone = "11987654321",
				ProfilePictureUrl = "https://example.com/gabriel.jpg"
			},
			new User
			{
				Id = 2,
				Name = "Maria",
				Email = "maria@email.com",
				Phone = "21998765432",
				ProfilePictureUrl = "https://example.com/maria.jpg"
			}
		};

		[HttpGet]
		public IActionResult GetAll()
		{
			return Ok(users);
		}

	}

	[HttpGet]
		public IActionResult GetAll()
		{
			return Ok(users);
		}

		[HttpGet("{id}")]
		public IActionResult GetById(int id)
		{
			var user = users.FirstOrDefault(u => u.Id == id);
			if (user == null)
				return NotFound(new { message = "User not found" });

			return Ok(user);
		}

		[HttpPost]
		public IActionResult Create([FromBody] User newUser)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			newUser.Id = users.Count > 0 ? users.Max(u => u.Id) + 1 : 1;
			users.Add(newUser);

			return CreatedAtAction(nameof(GetById), new { id = newUser.Id }, newUser);
		}

		[HttpPut("{id}")]
		public IActionResult Update(int id, [FromBody] User updatedUser)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var user = users.FirstOrDefault(u => u.Id == id);
			if (user == null)
				return NotFound(new { message = "User not found" });

			user.Name = updatedUser.Name;
			user.Email = updatedUser.Email;

			// Novos campos adicionados aqui!
			user.Phone = updatedUser.Phone;
			user.ProfilePictureUrl = updatedUser.ProfilePictureUrl;

			return Ok(user);
		}

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