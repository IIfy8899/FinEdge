namespace FinEdge.Domain.Exceptions;

public class UnauthorizedAccessException(string message) : Exception(message)
{
}
