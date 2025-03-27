using FinEdge.Application.Exceptions;
using UnauthorizedAccessException = FinEdge.Application.Exceptions.UnauthorizedAccessException;
using System.Text.Json;
using FinEdge.Application.Common.Models;

namespace FinEdge.API.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            if (httpContext.Response.HasStarted)
            {
                _logger.LogWarning("Response has already started, exception handling skipped");
                throw;
            }

            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, result) = exception switch
        {
            ValidationException ex => HandleValidationException(ex),
            NotFoundException ex => HandleNotFoundException(ex),
            UnauthorizedAccessException ex => HandleUnauthorizedException(ex),
            _ => HandleGenericException(exception)
        };

        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(result, _jsonOptions));
    }

    private (int StatusCode, Result<object> Result) HandleValidationException(ValidationException ex)
    {
        _logger.LogWarning(ex, "Validation error occurred");
        var errors = ex.Errors.SelectMany(e => e.Value).ToArray();
        return (StatusCodes.Status400BadRequest, Result<object>.Failure(errors));
    }

    private (int StatusCode, Result<object> Result) HandleNotFoundException(NotFoundException ex)
    {
        _logger.LogWarning(ex, "Resource not found");
        return (StatusCodes.Status404NotFound, Result<object>.Failure(ex.Message));
    }

    private (int StatusCode, Result<object> Result) HandleUnauthorizedException(UnauthorizedAccessException ex)
    {
        _logger.LogWarning(ex, "Unauthorized access");
        return (StatusCodes.Status403Forbidden, Result<object>.Failure("Forbidden"));
    }

    private (int StatusCode, Result<object> Result) HandleGenericException(Exception ex)
    {
        _logger.LogError(ex, "An unhandled exception occurred");
        return (StatusCodes.Status500InternalServerError, Result<object>.Failure("An unexpected error occurred"));
    }
}
