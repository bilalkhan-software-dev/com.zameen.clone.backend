namespace com.zameen.Exceptions
{
    public class AppValidationException(
        IEnumerable<string> errors,
        string message = "Validation failed"
    ) : AppException(message, "VALIDATION_ERROR", StatusCodes.Status400BadRequest)
    {
        public IEnumerable<string> Errors { get; } = errors;
    }
}
