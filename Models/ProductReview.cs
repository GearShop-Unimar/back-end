using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GearShop.Models
{
    [Index(nameof(ProductId), nameof(AuthorId), IsUnique = true)]
    public class ProductReview
    {
        public int Id { get; set; }

        [Required]
        [Range(1, 5)] public int Rating { get; set; }

        [MaxLength(1000)]
        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; } = null!;

        public int AuthorId { get; set; }
        [ForeignKey("AuthorId")]
        public User Author { get; set; } = null!;
    }
}
