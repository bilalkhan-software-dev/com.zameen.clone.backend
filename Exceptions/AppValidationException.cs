namespace com.zameen.Exceptions
{
    public class AppValidationException : AppException
    {
        public IEnumerable<string> Errors { get; }

        public AppValidationException(
            IEnumerable<string> errors,
            string message = "Validation failed"
        )
            : base(message, "VALIDATION_ERROR", StatusCodes.Status400BadRequest)
        {
            Errors = errors;
        }
    }
}
