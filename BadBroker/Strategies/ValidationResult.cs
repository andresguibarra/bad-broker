namespace BadBroker.Strategies
{
    public class ValidationResult
    {
        public bool IsValid { get; }
        public string? ErrorMessage { get; }

        public ValidationResult(string errorMessage)
        {
            IsValid = false;
            ErrorMessage = errorMessage;
        }

        private ValidationResult()
        {
            IsValid = true;
        }

        public static ValidationResult Success { get; } = new ValidationResult();
    }

}
