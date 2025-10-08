using System.ComponentModel.DataAnnotations;

namespace GearShop.Validators
{
    public class CpfValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null) return false;
            
            var cpf = value.ToString()?.Replace(".", "").Replace("-", "").Trim();
            
            if (string.IsNullOrEmpty(cpf) || cpf.Length != 11)
                return false;

            // Verifica se todos os dígitos são iguais
            if (cpf.All(c => c == cpf[0]))
                return false;

            // Validação do CPF
            var digits = cpf.Select(c => int.Parse(c.ToString())).ToArray();
            
            // Primeiro dígito verificador
            var sum1 = 0;
            for (int i = 0; i < 9; i++)
                sum1 += digits[i] * (10 - i);
            
            var remainder1 = sum1 % 11;
            var digit1 = remainder1 < 2 ? 0 : 11 - remainder1;
            
            if (digits[9] != digit1)
                return false;
            
            // Segundo dígito verificador
            var sum2 = 0;
            for (int i = 0; i < 10; i++)
                sum2 += digits[i] * (11 - i);
            
            var remainder2 = sum2 % 11;
            var digit2 = remainder2 < 2 ? 0 : 11 - remainder2;
            
            return digits[10] == digit2;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"O {name} fornecido não é válido.";
        }
    }
}
