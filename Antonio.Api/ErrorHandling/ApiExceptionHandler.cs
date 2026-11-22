using Antonio.Core.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Antonio.Api.ErrorHandling;

public sealed class ApiExceptionHandler(ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception ex, CancellationToken ct)
    {
        var (status, title) = ex switch
        {
            ArgumentException       => (StatusCodes.Status400BadRequest,         "Invalid request"),
            LlmAuthException        => (StatusCodes.Status502BadGateway,         "The LLM rejected Antonio's API key"),
            LlmRateLimitedException => (StatusCodes.Status503ServiceUnavailable, "The LLM is rate limiting, try again shortly"),
            LlmUnavailableException => (StatusCodes.Status503ServiceUnavailable, "The LLM is unavailable, try again shortly"),
            LlmException            => (StatusCodes.Status502BadGateway,         "The LLM request failed"),
            _                       => (StatusCodes.Status500InternalServerError, "Unexpected error")
        };

        if (status >= 500)
            logger.LogError(ex, "Request failed: {Title}", title);    // full details stay in the server log
        else
            logger.LogWarning("Bad request: {Message}", ex.Message);

        context.Response.StatusCode = status;
        await context.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = status == 400 ? ex.Message : null   // never leak internal details on 5xx
        }, ct);

        return true;   
    }
}