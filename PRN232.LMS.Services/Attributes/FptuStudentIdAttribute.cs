using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace PRN232.LMS.Services.Attributes
{
    /// <summary>
    /// Custom validation attribute for FPTU Student ID format.
    /// Valid format examples: SE19886, CE18793 (2 uppercase letters + 5 digits)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class FptuStudentIdAttribute : ValidationAttribute
    {
        private static readonly Regex _pattern = new Regex(@"^[A-Z]{2}\d{5}$", RegexOptions.Compiled);

        public FptuStudentIdAttribute()
        {
            ErrorMessage = "StudentCode must follow FPTU format: 2 uppercase letters + 5 digits (e.g. SE19886, CE18793)";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // Null is allowed — field is optional
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return ValidationResult.Success;

            if (_pattern.IsMatch(value.ToString()!))
                return ValidationResult.Success;

            return new ValidationResult(ErrorMessage);
        }
    }
}
