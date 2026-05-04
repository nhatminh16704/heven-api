using Heven.Api.Application.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Heven.Api.Web.Infrastructure;

public class ApiResponseFilter : IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (context.Result is ObjectResult objectResult)
        {
            var statusCode = objectResult.StatusCode ?? context.HttpContext.Response.StatusCode;

            // Bỏ qua nếu response đã được bọc vào ApiResponse từ trước
            if (objectResult.Value?.GetType().IsGenericType == true &&
                objectResult.Value.GetType().GetGenericTypeDefinition() == typeof(ApiResponse<>))
            {
                await next();
                return;
            }

            // Chỉ bọc lại các Status Code 2xx (Thành công)
            if (statusCode >= 200 && statusCode < 300)
            {
                var response = ApiResponse<object>.Succeeded(objectResult.Value);
                context.Result = new ObjectResult(response) { StatusCode = statusCode };
            }
        }
        // Xử lý controller trả về void (hoặc Task không có kiểu dữ liệu)
        else if (context.Result is EmptyResult || context.Result is OkResult)
        {
            var response = ApiResponse<object>.Succeeded(null);
            context.Result = new ObjectResult(response) { StatusCode = 200 };
        }

        await next();
    }
}
