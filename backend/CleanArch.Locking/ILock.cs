namespace CleanArch.Locking;

/// <summary>
/// An object that is used to lock access to a resource. Use its methods to request access to the resource.
/// </summary>
public interface ILock
{
    /// <summary>
    /// Acquire access to the locked resource. This method waits indefinitely for access. Once the Task resolves, the
    /// returned disposable is used to release the resource.
    /// </summary>
    /// <returns>A disposable object that can be used to release the resource.</returns>
    Task<IDisposable> AcquireAsync();
}
