using System.ComponentModel.DataAnnotations;
using GearShop.Validators;

namespace GearShop.Dtos
{
    public class CreateUserDto
    {
        [Required, MaxLength(120)]
        public string Name { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(160)]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required, Phone, MaxLength(30)]
        public string PhoneNumber { get; set; } = string.Empty;

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
    }
}