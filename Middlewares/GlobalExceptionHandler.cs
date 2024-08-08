using System;
using System.Threading;
using System.Threading.Tasks;
using Main.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Main.Middlewares;

internal sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is BadRequestException) {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        } else {
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        }
        await httpContext.Response.WriteAsync(exception.Message, cancellationToken);
        return true;
    }
}