namespace Heven.Api.Application.Common.Models;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public object? Error { get; set; }

    public static ApiResponse<T> Succeeded(T? data)
    {
        return new ApiResponse<T> 
        { 
            Success = true, 
            Data = data, 
            Error = null 
        };
    }

    public static ApiResponse<T> Failure(object error)
    {
        return new ApiResponse<T> 
        { 
            Success = false, 
            Data = default, 
            Error = error 
        };
    }
}
