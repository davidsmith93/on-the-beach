namespace Search.Exception;

public class FileParseException(string message, System.Exception cause) : System.Exception(message, cause)
{
}