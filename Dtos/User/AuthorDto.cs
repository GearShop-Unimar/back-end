namespace GearShop.Dtos.User
{
    public class AuthorDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Avatar { get; set; } // 👈 Corrigido
    }
}
