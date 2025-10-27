using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using GearShop.Validators;

namespace GearShop.Models
{
    [Index(nameof(Email), IsUnique = true)]
    [Index(nameof(Cpf), IsUnique = true)]
    public class User
    {
        public int Id { get; set; }
        [Required, MaxLength(120)]
        public string Name { get; set; } = string.Empty;
        [Required, EmailAddress, MaxLength(160)]
        public string Email { get; set; } = string.Empty;
        [Required, StringLength(60)]
        public string PasswordHash { get; set; } = string.Empty;
        [Required, Phone, MaxLength(30)]
        public string PhoneNumber { get; set; } = string.Empty;
        // REMOVED: public string ProfilePicture { get; set; } = string.Empty;
        public byte[]? ProfilePictureData { get; set; } // ADDED
        public string? ProfilePictureMimeType { get; set; } // ADDED
        [Required, CpfValidation]
        [MaxLength(11)]
        public string Cpf { get; set; } = string.Empty;
        [Required, MaxLength(2)]
        public string Estado { get; set; } = string.Empty;
        [Required, MaxLength(100)]
        public string Cidade { get; set; } = string.Empty;
        [Required, CepValidation]
        [MaxLength(8)]
        public string Cep { get; set; } = string.Empty;
        [Required, MaxLength(200)]
        public string Rua { get; set; } = string.Empty;
        [Required, MaxLength(20)]
        public string NumeroCasa { get; set; } = string.Empty;
        public Role Role { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }

    public enum Role
    {
        Client,
        Seller,
        Admin
    }
}