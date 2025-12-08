namespace CleanArch.Locking;

/// <summary>
/// Options for configuring locks
/// </summary>
public class LockOptions
{
    /// <summary>
    /// Maximum amount of concurrency allowed for the lock. Set to 1 for only a single thread being able to enter the lock at a time
    /// </summary>
    public int Concurrency { get; set; }
}
