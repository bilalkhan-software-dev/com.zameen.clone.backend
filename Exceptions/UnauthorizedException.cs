namespace com.zameen.Exceptions
{
    public class UnauthorizedException : AppException
    {
        public UnauthorizedException(string message = "Unauthorized")
            : base(message, "UNAUTHORIZED", StatusCodes.Status401Unauthorized) { }
    }
}
