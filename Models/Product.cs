using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace GearShop.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        [Url, MaxLength(500)]
        public string MainImageUrl { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Category { get; set; } = string.Empty;

        public int SellerId { get; set; }
        public User Seller { get; set; }
    }
}