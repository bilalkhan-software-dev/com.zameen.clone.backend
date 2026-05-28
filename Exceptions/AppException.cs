namespace com.zameen.Exceptions
{
    public abstract class AppException : Exception
    {
        public string ErrorCode { get; }
        public int HttpStatusCode { get; }

        protected AppException(string message, string errorCode, int httpStatusCode)
            : base(message)
        {
            ErrorCode = errorCode;
            HttpStatusCode = httpStatusCode;
        }
    }
}
