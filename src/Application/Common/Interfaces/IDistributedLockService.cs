namespace Heven.Api.Application.Common.Interfaces;

public interface IDistributedLockService
{
    Task<IDistributedLock> AcquireAsync(string resource, CancellationToken cancellationToken = default);
}
