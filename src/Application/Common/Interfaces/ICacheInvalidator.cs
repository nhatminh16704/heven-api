namespace Heven.Api.Application.Common.Interfaces;

public interface ICacheInvalidator
{
    // Trả về danh sách các Key cần phải xóa khỏi Redis / MemoryCache
    string[] CacheKeys { get; }
}
