using System.ComponentModel.DataAnnotations;

namespace GearShop.Dtos
{
    public class CreateUserDto
    {
        [Required, MaxLength(120)]
        public string Name { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(160)]
        public string Email { get; set; } = string.Empty;

        [Required, Phone, MaxLength(30)]
        public string phoneNumber { get; set; } = string.Empty;

        [Required, Url, MaxLength(500)]
        public string profilePicture { get; set; } = string.Empty;
    }
}
