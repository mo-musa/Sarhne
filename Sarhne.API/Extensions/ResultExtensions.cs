using Microsoft.AspNetCore.Mvc;
using Sarhne.Application.Common.Errors;
using Sarhne.Application.Common.Results;

namespace Sarhne.API.Extensions;

public static class ResultExtensions
{
    public static IActionResult Handle(
        this ControllerBase controller,
        Result result)
    {
        if (result.IsSuccess)
        {
            return controller.Ok();
        }

        return controller.CreateProblem(result.Errors);
    }

    public static IActionResult Handle<T>(
        this ControllerBase controller,
        Result<T> result)
    {
        if (result.IsSuccess)
        {
            return controller.Ok(result.Value);
        }

        return controller.CreateProblem(result.Errors);
    }

    private static ObjectResult CreateProblem(
        this ControllerBase controller,
        IReadOnlyList<Error> errors)
    {
        var statusCode = GetStatusCode(errors.First().Type);

        var problem = new ProblemDetails
        {
            Title = "One or more errors occurred.",
            Status = statusCode
        };

        problem.Extensions["errors"] = errors;

        return new ObjectResult(problem)
        {
            StatusCode = statusCode
        };
    }

    private static int GetStatusCode(ErrorType type)
    {
        return type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };
    }
}
