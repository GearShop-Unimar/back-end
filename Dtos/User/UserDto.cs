namespace GearShop.Dtos.User
{
    public class UserDto
    {
        public int Id { get; set; }
        public required string Name { get; set; } = string.Empty;
        public required string Email { get; set; } = string.Empty;
        public required string PhoneNumber { get; set; } = string.Empty;
        public string ProfilePicture { get; set; } = string.Empty;
        public required string Cidade { get; set; } = string.Empty;
        public required string Estado { get; set; } = string.Empty;
        public required string Role { get; set; } = string.Empty;
        public required string Avatar { get; set; } = string.Empty;
        public bool IsPremium { get; set; }
    }
}
