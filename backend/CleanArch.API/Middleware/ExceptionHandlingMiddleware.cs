using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace CleanArch.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            _logger.LogError(ex, "An unhandled exception occurred. Request Path: {Path}, Method: {Method}",
                context.Request.Path, context.Request.Method);
            
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var code = HttpStatusCode.InternalServerError;
        var result = JsonSerializer.Serialize(new { error = "An error occurred while processing your request." });

        // Optionally, you can customize the response based on exception type
        // For example, handle specific exceptions differently
        if (exception is ArgumentException || exception is ArgumentNullException)
        {
            code = HttpStatusCode.BadRequest;
            result = JsonSerializer.Serialize(new { error = exception.Message });
        }
        else if (exception is UnauthorizedAccessException)
        {
            code = HttpStatusCode.Unauthorized;
            result = JsonSerializer.Serialize(new { error = "Unauthorized access." });
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;
        return context.Response.WriteAsync(result);
    }
}

