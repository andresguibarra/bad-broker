namespace BadBroker.Strategies
{
    public interface IValidationStrategy
    {
        ValidationResult Validate(DateTime startDate, DateTime endDate);
    }
}
