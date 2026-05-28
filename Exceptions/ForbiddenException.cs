namespace com.zameen.Exceptions
{
    public class ForbiddenException : AppException
    {
        public ForbiddenException(string message = "Forbidden")
            : base(message, "FORBIDDEN", StatusCodes.Status403Forbidden) { }
    }
}
