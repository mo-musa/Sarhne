using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Sarhne.API.Middlewares;

public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IHostEnvironment environment) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        Console.WriteLine("___________________________________________________");
        Console.WriteLine("___________________________________________________");
        logger.LogError(
            exception,
            "Unhandled exception occurred.");
        Console.WriteLine("___________________________________________________");
        Console.WriteLine("___________________________________________________");

        if (exception is ValidationException validationException)
        {
            var errors = validationException.Errors
                .Select(e => new
                {
                    Field = e.PropertyName,
                    Message = e.ErrorMessage
                })
                .ToList();

            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

            await httpContext.Response.WriteAsJsonAsync(new
            {
                Title = "One or more validation errors occurred.",
                Status = StatusCodes.Status400BadRequest,
                TraceId = httpContext.TraceIdentifier,
                Errors = errors
            }, cancellationToken);

            return true;
        }

        var problem = new ProblemDetails
        {
            Title = "An unexpected error occurred.",
            Status = StatusCodes.Status500InternalServerError,
            Instance = httpContext.Request.Path
        };

        problem.Extensions["traceId"] = httpContext.TraceIdentifier;

        if (environment.IsDevelopment())
        {
            problem.Detail = exception.Message;
            problem.Extensions["exception"] = exception.GetType().Name;
            problem.Extensions["stackTrace"] = exception.StackTrace;
            problem.Extensions["innerException"] = exception.InnerException?.Message;
        }
        else
        {
            problem.Detail = "Something went wrong. Please try again later.";
        }

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);

        return true;
    }
}
