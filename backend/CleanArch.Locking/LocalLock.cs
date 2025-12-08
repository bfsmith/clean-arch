namespace CleanArch.Locking;

internal class LocalLock : ILock
{
    private readonly SemaphoreSlim _semaphore;

    internal LocalLock(SemaphoreSlim semaphore)
    {
        _semaphore = semaphore;
    }

    public async Task<IDisposable> AcquireAsync()
    {
        await _semaphore.WaitAsync();
        return new LocalLockHold(_semaphore, () => _semaphore.Release());
    }

    private class LocalLockHold : IDisposable
    {
        private bool _disposed;
        private readonly SemaphoreSlim _semaphore;
        private readonly Action _dispose;

        // Takes an action, instead of the semaphore, so there's no chance to change the semaphore's state
        // in some unexpected way.
        internal LocalLockHold(SemaphoreSlim semaphore, Action dispose)
        {
            _semaphore = semaphore;
            _dispose = dispose;
        }

        public void Dispose()
        {
            _semaphore.Release();
            // ObjectDisposedException.ThrowIf(_disposed, this);
            // _dispose();
            // _disposed = true;
        }
    }
}
