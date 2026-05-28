namespace com.zameen.Exceptions
{
    public class ResourceNotFoundException : AppException
    {
        public ResourceNotFoundException(string message = "Resource not found")
            : base(message, "RESOURCE_NOT_FOUND", StatusCodes.Status404NotFound) { }
    }
}
