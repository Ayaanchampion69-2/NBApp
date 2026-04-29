using System.ComponentModel.DataAnnotations;

namespace NBApp.Validators
{
    public class NoReleaseDateInPastAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null)
                return true;

            if (value is DateTime dateTime)
            {
                return dateTime.Date >= DateTime.Today;
            }

            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} cannot be before today's date.";
        }
    }
}
