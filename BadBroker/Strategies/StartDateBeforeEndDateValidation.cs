namespace BadBroker.Strategies
{
    public class StartDateBeforeEndDateValidation : IValidationStrategy
    {
        public ValidationResult Validate(DateTime startDate, DateTime endDate)
        {
            if (startDate >= endDate)
            {
                return new ValidationResult("The start date must be earlier than the end date.");
            }
            return ValidationResult.Success;
        }
    }
}
