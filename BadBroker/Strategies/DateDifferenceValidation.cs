namespace BadBroker.Strategies
{
    public class DateDifferenceValidation : IValidationStrategy
    {
        private readonly int _maxDays;

        public DateDifferenceValidation(int maxDays)
        {
            _maxDays = maxDays;
        }

        public ValidationResult Validate(DateTime startDate, DateTime endDate)
        {
            var diffDays = (endDate - startDate).Days;

            if (diffDays > _maxDays)
            {
                return new ValidationResult($"The difference between the start date and the end date cannot be greater than {_maxDays} days.");
            }
            return ValidationResult.Success;
        }
    }
}
