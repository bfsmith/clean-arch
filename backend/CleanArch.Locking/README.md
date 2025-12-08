# Locking Service

Locking is used to prevent multiple threads or processes from accessing the same resources at the same time. It can be used to ensure thread safety, enforce rate limiting, or consolidate duplicate requests for a resource.

# Quick Start

```csharp
// In your Program.cs file, or wherever you configure your services
builder.Services.AddLocalLocking();

// In some code that wants to lock
public class MyService
{
    private readonly ILocalLockService _lockService;

    // Inject the lock service
    public MyService(ILocalLockService lockService)
    {
        _lockService = lockService;
    }
    
    public async Task DoSomethingAsync()
    {
        // Get the lock with a consistent key
        var myLock = _lockService.GetLock("myKey");
        // Acquire access to the lock
        using (await myLock.AcquireAsync())
        {
            // Do something within lock
        }
    }
}
```

## Basic Usage

1. Request a lock from the service. Provide a key and amount of concurrency (optional, defaults to 1).
2. Given back a lock that can be used to request access.
3. When granted access, a disposable will be returned. Access is released when the disposable is disposed.

```csharp
var myLock = lockService.GetLock("myKey", new LockOptions { Concurrency = 1 });
using (await myLock.AcquireAsync())
{
    // Do something within lock
} // acquired access is released
```

# Keys

A lock is requested with a key. Locks are mapped to a key, so the same key will always return the same lock. And different keys will always return different locks.

```csharp
var myLock = lockService.GetLock("myKey");
var myOtherLock = lockService.GetLock("myOtherKey");

// myLock and myOtherLock are different locks, they WILL NOT block each other
```

```csharp
var myLock = lockService.GetLock("myKey");
var myLock2 = lockService.GetLock("myKey");

// myLock and myLock2 are the same lock, they WILL block each other
```

Use this to control what resources are locked and which are locked independently.

# Concurrency

By default, the concurrency is set to 1. This means that only one thread or process can enter the lock at a time.

```csharp
var myLock = lockService.GetLock("myKey"); // concurrency is 1 by default

// only one thread or process can enter the lock at a time
```

However, there are times when you want to allow more than one thread or process to enter the lock at a time, such as rate limiting or throttling some task.

```csharp
// Set the concurrency of the lock to 2
var myLock = lockService.GetLock("myKey", new LockOptions { Concurrency = 2 });
for(int i = 0; i < 100; i++)
{
  using(await myLock.AcquireAsync())
  {
    // Only two calls will be made at a time due to the lock having a concurrency of 2.
    // As soon as one call completes, its lock will be released when disposed by the using scope ending
    // then the next call will be able to enter the lock
    await _api.MakeRequestAsync(i);
  }
}
```

# Troubleshooting

**Only one thread ever acquires the lock, the rest hang**

The acquired disposable is not being disposed of properly.

```csharp
// Make sure you're wrapping the code in a using statement.
using(await myLock.AcquireAsync())
{
  // code
}
```
or
```csharp
// Use using and assign to an unused variable so it disposes when the scope ends
using var _ = await myLock.AcquireAsync();
// code
```

# TODO

Add a background job that cleans up locks that are no longer in use.
