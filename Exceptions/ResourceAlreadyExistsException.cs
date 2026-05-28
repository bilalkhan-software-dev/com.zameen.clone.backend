namespace com.zameen.Exceptions
{
    public class ResourceAlreadyExistsException : AppException
    {
        public ResourceAlreadyExistsException(string message = "Resource already exists")
            : base(message, "RESOURCE_ALREADY_EXISTS", StatusCodes.Status409Conflict) { }
    }
}
