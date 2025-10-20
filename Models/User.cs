using System.Collections.Generic; // Adicionado para ICollection
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

        // O hash do BCrypt tem 60 caracteres. 255 é seguro, mas 60 é mais preciso.
        [Required, StringLength(60)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required, Phone, MaxLength(30)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required, Url, MaxLength(500)]
        public string ProfilePicture { get; set; } = string.Empty;

        // Campos de endereço e identificação
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

        // A enum agora se chama Role
        public Role Role { get; set; }

        // Propriedade de navegação para os produtos que este usuário vende
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }

    // O nome foi simplificado e a opção 'Client' foi adicionada
    public enum Role
    {
        Client, 
        Seller,
        Admin
    }
}