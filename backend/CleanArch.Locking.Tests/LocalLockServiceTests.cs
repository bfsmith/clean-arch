using CleanArch.UnitTests;

namespace CleanArch.Locking.Tests;

internal class LocalLockServiceTests : UnitTestBase<LocalLockService>
{
    [Test]
    public void WhenUsingSameKey_ReturnsSameLock()
    {
        var myLock = SystemUnderTest.GetLock("myTest");
        var myLock2 = SystemUnderTest.GetLock("myTest");

        myLock2.Should().BeSameAs(myLock);
    }

    [Test]
    public void WhenUsingDifferentKey_ReturnsDifferentLock()
    {
        var myLock = SystemUnderTest.GetLock("myTest");
        var myLock2 = SystemUnderTest.GetLock("myTest2");
        
        myLock2.Should().NotBeSameAs(myLock);
    }

    [Test]
    public void WhenUsingSameKeyWithDifferentOptions_ReturnsFirstLock()
    {
        var myLock = SystemUnderTest.GetLock("myTest", new LockOptions
        {
            Concurrency = 2
        });
        var myLock2 = SystemUnderTest.GetLock("myTest", new LockOptions
        {
            Concurrency = 1
        });

        myLock2.Should().BeSameAs(myLock);
    }
}
