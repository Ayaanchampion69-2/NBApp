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
                return dateTime.Date >= DateTime.Today && dateTime.Date <= DateTime.Today.AddMonths(3);
            }
            /*
            else if (value is )
            */

            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} cannot be before today's date and cannot be further than 3 months in the future.";
        }
    }
}
