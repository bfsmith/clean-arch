namespace CleanArch.Locking.Tests;

public class LocalLockTests
{
    [Test]
    public async Task WhenMultipleRequestsToTheLock_DoesNotRunThemConcurrently()
    {
        var semaphore = new SemaphoreSlim(1, 1);
        var myLock = new LocalLock(semaphore);
        
        int count = 3;
        int wait = 10;

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
}
