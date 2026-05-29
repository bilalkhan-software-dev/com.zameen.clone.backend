namespace com.zameen.Exceptions
{
    public class ForbiddenException(string message = "Forbidden")
        : AppException(message, "FORBIDDEN", StatusCodes.Status403Forbidden) { }
}
