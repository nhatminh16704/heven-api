using Heven.Api.Application.Common.Interfaces;
using RedLockNet.SERedis;

namespace Heven.Api.Infrastructure.Services;

public class RedLockDistributedLockService : IDistributedLockService
{
    private static readonly TimeSpan Expiry = TimeSpan.FromSeconds(30);
    private static readonly TimeSpan Wait = TimeSpan.FromSeconds(10);
    private static readonly TimeSpan Retry = TimeSpan.FromMilliseconds(200);

    private readonly RedLockFactory _redLockFactory;

    public RedLockDistributedLockService(RedLockFactory redLockFactory)
    {
        _redLockFactory = redLockFactory;
    }

    public async Task<IDistributedLock> AcquireAsync(string resource, CancellationToken cancellationToken = default)
    {
        var redLock = await _redLockFactory.CreateLockAsync(resource, Expiry, Wait, Retry, cancellationToken);
        return new RedLockDistributedLock(redLock);
    }

    private sealed class RedLockDistributedLock(RedLockNet.IRedLock redLock) : IDistributedLock
    {
        public bool IsAcquired => redLock.IsAcquired;

        public ValueTask DisposeAsync() => redLock.DisposeAsync();
    }
}
