using System.Globalization;
using System.Linq;
using System.Windows.Controls;

namespace StaffRandomSelect.Domain
{
    public class NotCorrectValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string preStr1 = "精";
            string[] preStr2 = { "零", "一", "二" };
            string sufStr = "级";
            string input = value.ToString();
            if (input.Length > 3 && input.Length < 6 && input[0].ToString() == preStr1 && preStr2.Count(str => str == input[1].ToString()) > 0 && input.EndsWith(sufStr))
            {
                string rank = input[2..^1];
                if (!rank.StartsWith("0") && int.TryParse(rank, out int newRank) && newRank > 0 && newRank <= 90)
                {
                    return ValidationResult.ValidResult;
                }
            }
            return new ValidationResult(false, "示例:精零6级");
        }
    }
}