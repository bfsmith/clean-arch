using AutoFixture;
using Moq.AutoMock;

namespace CleanArch.UnitTests;

/// <summary>
/// Base class for unit tests that automates the setup of AutoFixture, Moq.AutoMock, and System Under Test (SUT) creation.
/// </summary>
/// <typeparam name="T">The type of the System Under Test.</typeparam>
public abstract class UnitTestBase<T>
{
    private Lazy<T> _lazySystemUnderTest = null!;

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

    protected UnitTestBase()
    {
        Fixture = new Fixture();
        AutoMock = new AutoMocker();
        _lazySystemUnderTest = new Lazy<T>(() => AutoMock.Get<T>());
    }
}

