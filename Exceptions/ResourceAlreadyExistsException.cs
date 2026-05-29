namespace com.zameen.Exceptions;

public class ResourceAlreadyExistsException(string message = "Resource already exists")
    : AppException(message, "RESOURCE_ALREADY_EXISTS", StatusCodes.Status409Conflict) { }
