namespace CleanArch.Locking.Tests;

public class LocalLockTests
{
    [Test]
    public async Task WhenMultipleRequestsToTheLock_DoesNotRunThemConcurrently()
    {
        var semaphore = new SemaphoreSlim(1, 1);
        var myLock = new LocalLock(semaphore);
        
        using var _ = await myLock.AcquireAsync();
        
        int count = 3;
        int wait = 4;

        var startTime = Environment.TickCount64;
        List<Task> tasks = new();
        for (int i = 0; i < count; i++)
        {
            tasks.Add(
                Utils.CreateAcquireAndSleepTask(myLock, wait)
            );
        }

        await Task.WhenAll(tasks);
        var endTime = Environment.TickCount64;

        var elapsed = (endTime - startTime);
        elapsed.Should().BeGreaterOrEqualTo(count * wait);
    }

    [Test]
    public async Task WhenLocksHaveDifferentSemaphores_TheyDoNotBlockEachOther()
    {
        var myLock = new LocalLock(new SemaphoreSlim(1, 1));
        var myLock2 = new LocalLock(new SemaphoreSlim(1, 1));

        var startTime = Environment.TickCount64;
        List<Task> tasks =
        [
            Utils.CreateAcquireAndSleepTask(myLock, 10),
            Utils.CreateAcquireAndSleepTask(myLock2, 10),
        ];
        await Task.WhenAll(tasks);
        var endTime = Environment.TickCount64;

        var elapsed = (endTime - startTime);
        elapsed.Should().BeGreaterOrEqualTo(10);
        elapsed.Should().BeLessThan(20);
    }

    [Test]
    public async Task WhenLockIsDeconstructed_SemaphoreIsDisposed()
    {
        var semaphore = new SemaphoreSlim(1, 1);
        // Wrap in a scope so the lock immediately is unreferenced
        {
            var myLock = new LocalLock(semaphore);
            weakRef = new WeakReference(myLock);
        }
        
        // Force aggressive garbage collection to ensure the lock is collected
        // Multiple passes to ensure collection and finalization
        for (int i = 0; i < 3; i++)
        {
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
            GC.WaitForPendingFinalizers();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
        }
        
        // Verify the lock was actually collected
        weakRef.IsAlive.Should().BeFalse("The LocalLock should have been garbage collected");
        
        // Assert that the semaphore was disposed by trying to use it
        var act = async () => await semaphore.WaitAsync();
        await act.Should().ThrowAsync<ObjectDisposedException>();
    }
}
