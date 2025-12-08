namespace CleanArch.Locking.Tests;

internal static class Utils
{
    internal static Task CreateAcquireAndSleepTask(ILock @lock, int ms)
    {
        return Task.Run(async () =>
        {
            using (await @lock.AcquireAsync())
            {
                Thread.Sleep(ms);
            }
        });
    }

    /// <summary>
    /// Waits until a given condition is met
    /// </summary>
    /// <param name="pauseMs">Time between checks, in milliseconds</param>
    /// <param name="maxWaitMs">Maximum milliseconds to wait</param>
    /// <param name="check">Function that returns true once the condition is met</param>
    /// <returns>True if condition is met, false if not</returns>
    internal static async Task<bool> WaitUntilAsync(int pauseMs, int maxWaitMs, Func<bool> check)
    {
        for (int i = 0; i < maxWaitMs; i += pauseMs)
        {
            if (check())
            {
                return true;
            }
            await SleepAsync(pauseMs);
        }

        return false;
    }

    internal static Task SleepAsync(int ms) => Task.Run(() => Thread.Sleep(ms));
}
