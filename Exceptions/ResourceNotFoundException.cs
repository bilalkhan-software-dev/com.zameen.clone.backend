namespace com.zameen.Exceptions;

public class ResourceNotFoundException(string message = "Resource not found")
    : AppException(message, "RESOURCE_NOT_FOUND", StatusCodes.Status404NotFound) { }
