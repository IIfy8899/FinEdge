namespace FinEdge.Application.Common.Models;

public class Result<T>
{
    internal Result(T? data, bool succeeded, IEnumerable<string> errors)
    {
        Data = data;
        Succeeded = succeeded;
        Errors = errors.ToArray();
    }

    public T? Data { get; }
    public bool Succeeded { get; }
    public string[] Errors { get; }

    public static Result<T> Success(T data) => new(data, true, Array.Empty<string>());
    public static Result<T> Failure(IEnumerable<string> errors) => new(default, false, errors);
    public static Result<T> Failure(string error) => new(default, false, new[] { error });
}
