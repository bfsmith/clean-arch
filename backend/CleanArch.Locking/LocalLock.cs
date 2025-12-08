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
        return new LocalLockHold(() => _semaphore.Release());
    }

    private class LocalLockHold : IDisposable
    {
        private bool _disposed;
        private readonly Action _dispose;

        // Takes an action, instead of the semaphore, so there's no chance to change the semaphore's state
        // in some unexpected way.
        internal LocalLockHold(Action dispose)
        {
            _dispose = dispose;
        }

        public void Dispose()
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            _dispose();
            _disposed = true;
        }
    }
}
