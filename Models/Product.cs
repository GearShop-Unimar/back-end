using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema; // Added

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
        public byte[]? ImageData { get; set; } // ADDED
        public string? ImageMimeType { get; set; } // ADDED
        [Required, MaxLength(100)]
        public string Category { get; set; } = string.Empty;
        public int SellerId { get; set; }
        [ForeignKey("SellerId")] // Good practice to add ForeignKey attribute
        public User? Seller { get; set; } // Changed to nullable to match previous fixes
    }
}