using GearShop.Dtos.User;

namespace GearShop.Dtos.Product
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ProductId { get; set; }

        public AuthorDto Author { get; set; } = null!;
    }
}
