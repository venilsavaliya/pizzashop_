using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace DAL.Validators
{
    public class RequiredIfCountryPresent : ValidationAttribute
    {
        private readonly string _countryProperty;

        public RequiredIfCountryPresent(string countryProperty)
        {
            _countryProperty = countryProperty;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // Get the Country property dynamically using Reflection
            var countryPropertyInfo = validationContext.ObjectType.GetProperty(_countryProperty);

            if (countryPropertyInfo == null)
            {
                return new ValidationResult($"Unknown property: {_countryProperty}");
            }

            var countryValue = countryPropertyInfo.GetValue(validationContext.ObjectInstance) as string;

            // If Country is present, the current field (State or City) must also be provided
            if (!string.IsNullOrEmpty(countryValue) && string.IsNullOrEmpty(value as string))
            {
                return new ValidationResult($"{validationContext.DisplayName} is required when {_countryProperty} is provided.");
            }

            return ValidationResult.Success;
        }
    }
}
