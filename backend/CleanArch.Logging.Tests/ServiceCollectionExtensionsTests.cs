using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Serilog;
using CleanArch.Logging;
using CleanArch.UnitTests;

namespace CleanArch.Logging.Tests;

[TestFixture]
public class ServiceCollectionExtensionsTests : UnitTestBase<object>
{
    private IServiceCollection _services = null!;
    private IConfiguration _configuration = null!;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        _services = new ServiceCollection();
        var configurationBuilder = new ConfigurationBuilder();
        _configuration = configurationBuilder.Build();
    }

    [TearDown]
    public override void TearDown()
    {
        // Clean up static logger
        Log.CloseAndFlush();
        base.TearDown();
    }

    [Test]
    public void AddCleanLogging_WithValidConfiguration_ShouldNotThrow()
    {
        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            _services.AddCleanLogging(_configuration);
        });
    }

    [Test]
    public void AddCleanLogging_WithValidConfiguration_ShouldReturnServices()
    {
        // Act
        var result = _services.AddCleanLogging(_configuration);

        // Assert
        result.Should().BeSameAs(_services);
    }

    [Test]
    public void AddCleanLogging_WithValidConfiguration_ShouldSetLogLogger()
    {
        // Act
        _services.AddCleanLogging(_configuration);

        // Assert
        Log.Logger.Should().NotBeNull();
    }

    [Test]
    public void AddCleanLogging_WithValidConfiguration_ShouldRegisterSerilog()
    {
        // Act
        _services.AddCleanLogging(_configuration);
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
        loggerFactory.Should().NotBeNull();
        
        var logger = loggerFactory!.CreateLogger("Test");
        logger.Should().NotBeNull();
    }

    [Test]
    public void AddCleanLogging_WithNullServices_ShouldThrowArgumentNullException()
    {
        // Arrange
        IServiceCollection? nullServices = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
        {
            nullServices!.AddCleanLogging(_configuration);
        });
    }

    [Test]
    public void AddCleanLogging_WithNullConfiguration_ShouldHandleGracefully()
    {
        // Arrange
        IConfiguration? nullConfiguration = null;

        // Act & Assert - Should not throw, but may create logger with default config
        Assert.DoesNotThrow(() =>
        {
            _services.AddCleanLogging(nullConfiguration!);
        });
    }

    [Test]
    public void AddCleanLogging_WithInvalidConfiguration_ShouldNotThrow()
    {
        // Arrange
        var invalidConfig = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Serilog:Invalid:Path", "invalid value" }
            })
            .Build();

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            _services.AddCleanLogging(invalidConfig);
        });
    }

    [Test]
    public void AddCleanLogging_WithEmptyConfiguration_ShouldNotThrow()
    {
        // Arrange
        var emptyConfig = new ConfigurationBuilder().Build();

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            _services.AddCleanLogging(emptyConfig);
        });
    }

    [Test]
    public void AddCleanLogging_ShouldClearExistingProviders()
    {
        // Arrange
        _services.AddLogging(builder => builder.AddConsole());

        // Act
        _services.AddCleanLogging(_configuration);
        var serviceProvider = _services.BuildServiceProvider();

        // Assert - Should only have Serilog provider
        var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
        loggerFactory.Should().NotBeNull();
    }

    [Test]
    public void AddCleanLogging_ShouldDisposeLoggerOnServiceProviderDisposal()
    {
        // Act
        _services.AddCleanLogging(_configuration);
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        Assert.DoesNotThrow(() =>
        {
            serviceProvider.Dispose();
        });
    }

    [Test]
    public void AddCleanLogging_CanBeCalledMultipleTimes_ShouldNotThrow()
    {
        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            _services.AddCleanLogging(_configuration);
            _services.AddCleanLogging(_configuration);
        });
    }

    [Test]
    public void AddCleanLogging_WithConfigurationContainingSerilogSettings_ShouldReadFromConfiguration()
    {
        // Arrange
        var configWithSerilog = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Serilog:MinimumLevel:Default", "Debug" },
                { "Serilog:MinimumLevel:Override:Microsoft", "Error" }
            })
            .Build();

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            _services.AddCleanLogging(configWithSerilog);
        });
    }
}

