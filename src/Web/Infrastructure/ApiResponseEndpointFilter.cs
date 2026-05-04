using Heven.Api.Application.Common.Models;
using Microsoft.AspNetCore.Http;

namespace Heven.Api.Web.Infrastructure;

public class ApiResponseEndpointFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var result = await next(context);

        // Trường hợp API không trả về gì
        if (result is null)
        {
            return Results.Ok(ApiResponse<object>.Succeeded(null));
        }

        // Bỏ qua nếu đã được bọc trong ApiResponse từ trước
        if (result.GetType().IsGenericType && result.GetType().GetGenericTypeDefinition() == typeof(ApiResponse<>))
        {
            return result;
        }

        int statusCode = StatusCodes.Status200OK;
        object? data = result;

        // Bóc tách dữ liệu nếu API dùng Results.Ok(), Results.NoContent()...
        if (result is IResult)
        {
            if (result is IStatusCodeHttpResult statusCodeResult)
            {
                statusCode = statusCodeResult.StatusCode ?? StatusCodes.Status200OK;
            }

            if (result is IValueHttpResult valueResult)
            {
                data = valueResult.Value;
            }
            else if (result is IStatusCodeHttpResult && result is not IValueHttpResult)
            {
                // Các dạng như Results.NoContent() hoặc Results.Ok() không có data
                data = null;
            }
            else
            {
                // Các dạng khác như trả về File, Stream, Redirect... thì giữ nguyên
                return result;
            }
        }

        // Bọc thành công (Status code 2xx)
        if (statusCode >= 200 && statusCode < 300)
        {
            var response = ApiResponse<object>.Succeeded(data);
            return Results.Json(response, statusCode: statusCode);
        }

        // Trả ra nguyên gốc nếu là lỗi (ví dụ 400 BadRequest), ExceptionHandler sẽ lo
        return result;
    }
}
