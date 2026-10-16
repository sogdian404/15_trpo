using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace _15_trpo.VAL
{
    class Rating:ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo
       cultureInfo)
        {
            var input = (value ?? "").ToString().Trim();
            if (input == string.Empty)
            {
                return new ValidationResult(false, "Ввод обязателен");
            }
            if (!float.TryParse(input, out float floatValue))
            {
                return new ValidationResult(false, "Необходимо ввести число");
            }
            if (floatValue < 0 || floatValue>5)
            {
                return new ValidationResult(false, "Число должно быть в районе 0 - 5");
            }
            return ValidationResult.ValidResult;
        }
    }
}
