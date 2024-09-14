using FluentValidation;
using System.Net;
using System.Text.Json;

namespace AssignmentApp.Middlewares;

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
            // Call the next middleware in the pipeline
            await _next(context);
        }
        catch (Exception ex)
        {
            // Log the exception
            _logger.LogError($"Something went wrong: {ex.Message}");

            // Handle the exception
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        if (exception is ValidationException validationException)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            var errors = validationException.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });

            var response = new
            {
                StatusCode = context.Response.StatusCode,
                Message = "Validation failed.",
                Errors = errors
            };

            var jsonResponse = JsonSerializer.Serialize(response);
            return context.Response.WriteAsync(jsonResponse);
        }

        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        var defaultResponse = new
        {
            StatusCode = context.Response.StatusCode,
            Message = "Internal Server Error. Please try again later."
        };
        return context.Response.WriteAsync(JsonSerializer.Serialize(defaultResponse));
    }
}

