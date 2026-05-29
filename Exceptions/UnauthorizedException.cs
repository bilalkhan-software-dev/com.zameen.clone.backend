namespace com.zameen.Exceptions
{
    public class UnauthorizedException(string message = "Unauthorized")
        : AppException(message, "UNAUTHORIZED", StatusCodes.Status401Unauthorized) { }
}
