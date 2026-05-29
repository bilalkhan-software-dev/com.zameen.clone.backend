namespace com.zameen.Exceptions
{
    public abstract class AppException(string message, string errorCode, int httpStatusCode)
        : Exception(message)
    {
        public string ErrorCode { get; } = errorCode;
        public int HttpStatusCode { get; } = httpStatusCode;
    }
}
