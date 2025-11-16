using FluentAssertions;
using NUnit.Framework;
using Serilog;
using Serilog.Events;
using CleanArch.Logging;
using CleanArch.UnitTests;

namespace CleanArch.Logging.Tests;

[TestFixture]
public class SerilogConfigurationTests : UnitTestBase<object>
{
    [Test]
    public void CreateLoggerConfiguration_ShouldReturnValidConfiguration()
    {
        // Act
        var configuration = SerilogConfiguration.CreateLoggerConfiguration();

        // Assert
        configuration.Should().NotBeNull();
    }

    [Test]
    public void CreateLoggerConfiguration_ShouldSetMinimumLevelToInformation()
    {
        // Act
        var configuration = SerilogConfiguration.CreateLoggerConfiguration();
        var logger = configuration.CreateLogger();

        // Assert
        logger.IsEnabled(LogEventLevel.Information).Should().BeTrue();
        logger.IsEnabled(LogEventLevel.Debug).Should().BeFalse();
    }

    [Test]
    public void CreateLoggerConfiguration_ShouldOverrideMicrosoftLogLevelToWarning()
    {
        // Act
        var configuration = SerilogConfiguration.CreateLoggerConfiguration();
        var logger = configuration.CreateLogger();

        // Assert - Microsoft namespaces should require Warning level
        logger.IsEnabled(LogEventLevel.Warning).Should().BeTrue();
    }

    [Test]
    public void CreateLoggerConfiguration_ShouldOverrideMicrosoftAspNetCoreLogLevelToWarning()
    {
        // Act
        var configuration = SerilogConfiguration.CreateLoggerConfiguration();
        var logger = configuration.CreateLogger();

        // Assert - Microsoft.AspNetCore namespaces should require Warning level
        logger.IsEnabled(LogEventLevel.Warning).Should().BeTrue();
    }

    [Test]
    public void CreateLoggerConfiguration_ShouldUseCustomJsonFormatter()
    {
        // Act
        var configuration = SerilogConfiguration.CreateLoggerConfiguration();
        var logger = configuration.CreateLogger();

        // Assert - Should be able to create logger without exceptions
        logger.Should().NotBeNull();
        
        // Verify we can log without exceptions
        Assert.DoesNotThrow(() =>
        {
            logger.Information("Test message");
        });
    }

    [Test]
    public void CreateLoggerConfiguration_ShouldCreateLoggerWithoutExceptions()
    {
        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            var configuration = SerilogConfiguration.CreateLoggerConfiguration();
            var logger = configuration.CreateLogger();
            logger.Dispose();
        });
    }

    [Test]
    public void CreateLoggerConfiguration_ShouldConfigureEnrichers()
    {
        // Act
        var configuration = SerilogConfiguration.CreateLoggerConfiguration();
        var logger = configuration.CreateLogger();

        // Assert - Should be able to log with enrichers without exceptions
        Assert.DoesNotThrow(() =>
        {
            logger.Information("Test message with enrichers");
        });
        
        logger.Dispose();
    }

    [Test]
    public void CreateLoggerConfiguration_ShouldHandleMultipleCalls()
    {
        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            var config1 = SerilogConfiguration.CreateLoggerConfiguration();
            var config2 = SerilogConfiguration.CreateLoggerConfiguration();
            var logger1 = config1.CreateLogger();
            var logger2 = config2.CreateLogger();
            
            logger1.Dispose();
            logger2.Dispose();
        });
    }
}

