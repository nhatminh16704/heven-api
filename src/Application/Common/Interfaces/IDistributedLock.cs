namespace Heven.Api.Application.Common.Interfaces;

public interface IDistributedLock : IAsyncDisposable
{
    bool IsAcquired { get; }
}
