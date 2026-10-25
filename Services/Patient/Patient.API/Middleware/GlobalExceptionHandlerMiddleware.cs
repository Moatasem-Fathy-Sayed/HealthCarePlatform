using System.Net;
using System.Text.Json;
using FluentValidation;

namespace Patient.API.Middleware;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred during request processing.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var response = context.Response;

        string message = exception.Message;
        object? errors = null;

        switch (exception)
        {
            case KeyNotFoundException:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                break;

            case ValidationException validationException:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                message = "Validation failed";
                errors = validationException.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }).ToList();
                break;

            case InvalidOperationException:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                break;

            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                break;
        }

        var errorResponse = new
        {
            Success = false,
            Message = message,
            Errors = errors
        };

        var result = JsonSerializer.Serialize(errorResponse);
        await response.WriteAsync(result);
    }
}