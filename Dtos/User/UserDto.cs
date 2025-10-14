namespace GearShop.Dtos.User
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // Papel do usuário: Vendedor ou Admin
        public string Role { get; set; } = "Vendedor";
    }
}