using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace _15_trpo.VAL
{
    public class Count : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var input = (value ?? "").ToString().Trim();
            if (input == string.Empty)
            {
                return new ValidationResult(false, "Ввод обязателен");
            }
            if (!int.TryParse(input, out int intValue))
            {
                return new ValidationResult(false, "Необходимо ввести число");
            }
            if (intValue < 0)
            {
                return new ValidationResult(false, "Число должно быть неотрицательным");
            }
            return ValidationResult.ValidResult;
        }
    } 
}

