namespace CleanArch.Locking;

/// <summary>
/// Service for retrieving local locks. The locks are local to the running service and are not distributed or shared across services.
/// </summary>
public interface ILocalLockService
{
    ILock GetLock(string key, LockOptions? options = null);
}
