using Heven.Api.Application.Common.Exceptions;
using Heven.Api.Application.Common.Models;
using Microsoft.AspNetCore.Diagnostics;
using NotFoundException = Heven.Api.Application.Common.Exceptions.NotFoundException;

namespace Heven.Api.Web.Infrastructure;

public class CustomExceptionHandler : IExceptionHandler
{
    private readonly Dictionary<Type, Func<HttpContext, Exception, Task>> _exceptionHandlers;

    public CustomExceptionHandler()
    {
        _exceptionHandlers = new()
        {
            { typeof(ValidationException), HandleValidationException },
            { typeof(NotFoundException), HandleNotFoundException },
            { typeof(UnauthorizedAccessException), HandleUnauthorizedAccessException },
            { typeof(ForbiddenAccessException), HandleForbiddenAccessException },
            { typeof(ConflictException), HandleConflictException },
        };
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var exceptionType = exception.GetType();

        if (_exceptionHandlers.TryGetValue(exceptionType, out var handler))
        {
            await handler.Invoke(httpContext, exception);
            return true;
        }

        await HandleUnknownException(httpContext, exception);
        return true;
    }

    private async Task HandleValidationException(HttpContext httpContext, Exception ex)
    {
        var exception = (ValidationException)ex;
        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        
        var errorDetails = exception.Errors.SelectMany(x => x.Value.Select(e => new 
        {
            Message = e.ErrorMessage,
            Code = e.ErrorCode?.EndsWith("Validator") == true ? "INVALID_INPUT" : e.ErrorCode
        }));
        
        await httpContext.Response.WriteAsJsonAsync(ApiResponse<object>.Failure(errorDetails));
    }

    private async Task HandleNotFoundException(HttpContext httpContext, Exception ex)
    {
        var exception = (NotFoundException)ex;
        httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
        var message = string.IsNullOrEmpty(exception.Message) ? "The specified resource was not found." : exception.Message;
        
        await httpContext.Response.WriteAsJsonAsync(ApiResponse<object>.Failure(new { Message = message, Code = exception.ErrorCode }));
    }

    private async Task HandleUnauthorizedAccessException(HttpContext httpContext, Exception ex)
    {
        httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await httpContext.Response.WriteAsJsonAsync(ApiResponse<object>.Failure(new { Message = "Unauthorized", Code = "UNAUTHORIZED" }));
    }

    private async Task HandleForbiddenAccessException(HttpContext httpContext, Exception ex)
    {
        var exception = (ForbiddenAccessException)ex;
        httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
        var message = string.IsNullOrEmpty(exception.Message) || exception.Message.Contains("Exception of type") ? "Forbidden" : exception.Message;

        await httpContext.Response.WriteAsJsonAsync(ApiResponse<object>.Failure(new { Message = message, Code = exception.ErrorCode }));
    }

    private async Task HandleConflictException(HttpContext httpContext, Exception ex)
    {
        var exception = (ConflictException)ex;
        httpContext.Response.StatusCode = StatusCodes.Status409Conflict;
        var message = string.IsNullOrEmpty(exception.Message) ? "A conflict occurred." : exception.Message;

        await httpContext.Response.WriteAsJsonAsync(ApiResponse<object>.Failure(new { Message = message, Code = exception.ErrorCode }));
    }

    private async Task HandleUnknownException(HttpContext httpContext, Exception ex)
    {
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(ApiResponse<object>.Failure(new { Message = "Internal Server Error", Code = "INTERNAL_SERVER_ERROR", Detail = ex.Message }));
    }
}
