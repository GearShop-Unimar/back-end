using System.ComponentModel.DataAnnotations;

namespace GearShop.Validators
{
    public class CepValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null) return false;
            
            var cep = value.ToString()?.Replace("-", "").Trim();
            
            if (string.IsNullOrEmpty(cep) || cep.Length != 8)
                return false;

            // Verifica se contém apenas dígitos
            return cep.All(char.IsDigit);
        }

        public override string FormatErrorMessage(string name)
        {
            return $"O {name} deve conter exatamente 8 dígitos.";
        }
    }
}
