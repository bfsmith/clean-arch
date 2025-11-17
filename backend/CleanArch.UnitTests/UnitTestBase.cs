using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;

namespace CleanArch.UnitTests;

/// <summary>
/// Base class for unit tests that automates the setup of AutoFixture, Moq.AutoMock, and System Under Test (SUT) creation.
/// </summary>
/// <typeparam name="T">The type of the System Under Test.</typeparam>
public abstract class UnitTestBase<T>
{
    private Lazy<T> _lazySystemUnderTest = null!;
    private Lazy<Mock<ILogger>> _lazyMockLogger = null!;

    /// <summary>
    /// Gets the System Under Test instance being tested. The instance is created lazily on first access,
    /// allowing mocks to be configured before the SUT is instantiated.
    /// </summary>
    protected T SystemUnderTest => _lazySystemUnderTest.Value;

    /// <summary>
    /// Gets the AutoFixture instance for test data generation.
    /// </summary>
    protected readonly Fixture Fixture;

    /// <summary>
    /// Gets the AutoMocker instance for SUT creation with auto-mocked dependencies.
    /// </summary>
    protected readonly AutoMocker AutoMock;

    /// <summary>
    /// Gets a Mock&lt;ILogger&gt; instance. The mock is created lazily on first access.
    /// </summary>
    protected Mock<ILogger> MockLogger => _lazyMockLogger.Value;

    protected UnitTestBase()
    {
        Fixture = new Fixture();
        AutoMock = new AutoMocker();
        _lazySystemUnderTest = new Lazy<T>(() => AutoMock.Get<T>());
        _lazyMockLogger = new Lazy<Mock<ILogger>>(() => AutoMock.GetMock<ILogger>());
    }

    /// <summary>
    /// Sets up the test by resetting the mock logger and configuring default behavior.
    /// Override this method if you need custom setup, but call base.SetUp() first.
    /// </summary>
    [SetUp]
    public virtual void SetUp()
    {
        // Reset the mock to clear any previous setups
        MockLogger.Reset();
        // Set up default behavior for BeginScope
        MockLogger.Setup(x => x.BeginScope(It.IsAny<object>())).Returns(Mock.Of<IDisposable>());
    }

    /// <summary>
    /// Tears down the test by resetting the mock logger.
    /// Override this method if you need custom teardown, but call base.TearDown() last.
    /// </summary>
    [TearDown]
    public virtual void TearDown()
    {
        // Reset the mock after each test to ensure clean state
        MockLogger.Reset();
    }
}

