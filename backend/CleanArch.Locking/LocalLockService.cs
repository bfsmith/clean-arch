using System.Collections.Concurrent;

namespace CleanArch.Locking;

internal class LocalLockService : ILocalLockService
{
    private readonly ConcurrentDictionary<string, ILock> _locks = new();

    private static readonly LockOptions DefaultLockOptions = new() { Concurrency = 1 };

    public ILock GetLock(string key, LockOptions? options = null)
    {
        var @lock = _locks.GetOrAdd(key, _ =>
        {
            var lockOptions = options ?? DefaultLockOptions;
            var localLock = new LocalLock(new SemaphoreSlim(lockOptions.Concurrency, lockOptions.Concurrency));
            return localLock;
        });
        return @lock;
    }
    
}
