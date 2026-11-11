using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

public class MyAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext context)
    {
        if (value == null)
            return ValidationResult.Success;

        string dateStr = value.ToString();

        //// Регулярное выражение для формата dd.MM.yyyy
        //string pattern = @"^(0[1-9]|[12][0-9]|3[01])\." +   // день 01-31
        //                 @"(0[1-9]|1[0-2])\." +           // месяц 01-12
        //                 @"(\d{4})$";                     // год 4 цифры

        //if (!Regex.IsMatch(dateStr, pattern))
        //    return new ValidationResult("Неверный формат даты. Используйте dd.MM.yyyy");

        if (!DateTime.TryParse(dateStr, out DateTime date))
            return new ValidationResult("Неверная дата");

        if (date > DateTime.Now)
            return new ValidationResult("Дата не может быть в будущем");

        return ValidationResult.Success;
    }
}