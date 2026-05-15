using Heven.Api.Application.Common.Exceptions;
using Heven.Api.Application.Common.Models;
using Microsoft.AspNetCore.Diagnostics;

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
            Field = x.Key,
            Message = e.ErrorMessage,
            // Ẩn đi ErrorCode hoặc cấu hình tuỳ chỉnh nếu nó kết thúc bằng "Validator"
            Code = e.ErrorCode?.EndsWith("Validator") == true ? "INVALID_INPUT" : e.ErrorCode 
        }));
        
        await httpContext.Response.WriteAsJsonAsync(ApiResponse<object>.Failure(errorDetails));
    }

    private async Task HandleNotFoundException(HttpContext httpContext, Exception ex)
    {
        var exception = (NotFoundException)ex;
        httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
        var message = string.IsNullOrEmpty(exception.Message) ? "The specified resource was not found." : exception.Message;
        
        await httpContext.Response.WriteAsJsonAsync(ApiResponse<object>.Failure(new { Message = message }));
    }

    private async Task HandleUnauthorizedAccessException(HttpContext httpContext, Exception ex)
    {
        httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await httpContext.Response.WriteAsJsonAsync(ApiResponse<object>.Failure(new { Message = "Unauthorized" }));
    }

    private async Task HandleForbiddenAccessException(HttpContext httpContext, Exception ex)
    {
        httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
        await httpContext.Response.WriteAsJsonAsync(ApiResponse<object>.Failure(new { Message = "Forbidden" }));
    }

    private async Task HandleUnknownException(HttpContext httpContext, Exception ex)
    {
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(ApiResponse<object>.Failure(new { Message = "Internal Server Error", Detail = ex.Message }));
    }
}
