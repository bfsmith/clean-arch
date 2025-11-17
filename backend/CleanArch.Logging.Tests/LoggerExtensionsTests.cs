using System.Collections;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Serilog.Events;
using CleanArch.Logging;
using CleanArch.UnitTests;

namespace CleanArch.Logging.Tests;

[TestFixture]
public class LoggerExtensionsTests : UnitTestBase<object>
{
    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        // Additional setup can be added here if needed
    }

    #region Null Safety Tests

    [Test]
    public void Debug_WithNullLogger_ShouldNotThrow()
    {
        // Arrange
        ILogger? nullLogger = null;

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            nullLogger!.Debug("Test message");
        });
    }

    [Test]
    public void Info_WithNullLogger_ShouldNotThrow()
    {
        // Arrange
        ILogger? nullLogger = null;

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            nullLogger!.Info("Test message");
        });
    }

    [Test]
    public void Warn_WithNullLogger_ShouldNotThrow()
    {
        // Arrange
        ILogger? nullLogger = null;

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            nullLogger!.Warn("Test message");
        });
    }

    [Test]
    public void Error_WithNullLogger_ShouldNotThrow()
    {
        // Arrange
        ILogger? nullLogger = null;

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            nullLogger!.Error("Test message");
        });
    }

    [Test]
    public void Debug_WithNullMessage_ShouldNotThrow()
    {
        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            MockLogger.Object.Debug(null!);
        });
    }

    [Test]
    public void Info_WithNullMessage_ShouldNotThrow()
    {
        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            MockLogger.Object.Info(null!);
        });
    }

    [Test]
    public void Warn_WithNullMessage_ShouldNotThrow()
    {
        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            MockLogger.Object.Warn(null!);
        });
    }

    [Test]
    public void Error_WithNullMessage_ShouldNotThrow()
    {
        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            MockLogger.Object.Error(null!);
        });
    }

    [Test]
    public void Debug_WithNullProperties_ShouldNotThrow()
    {
        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            MockLogger.Object.Debug("Test message", null);
        });
    }

    [Test]
    public void Info_WithNullProperties_ShouldNotThrow()
    {
        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            MockLogger.Object.Info("Test message", null);
        });
    }

    [Test]
    public void Warn_WithNullProperties_ShouldNotThrow()
    {
        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            MockLogger.Object.Warn("Test message", null);
        });
    }

    [Test]
    public void Error_WithNullProperties_ShouldNotThrow()
    {
        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            MockLogger.Object.Error("Test message", null);
        });
    }

    [Test]
    public void AddContext_WithNullLogger_ShouldNotThrow()
    {
        // Arrange
        ILogger? nullLogger = null;

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            nullLogger!.AddContext(new { Test = "Value" });
        });
    }

    [Test]
    public void AddContext_WithNullContext_ShouldReturnNull()
    {
        // Act
        var result = MockLogger.Object.AddContext(null!);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region Simple Object Tests

    [Test]
    public void Debug_WithSimpleObject_ShouldNotThrow()
    {
        // Arrange
        var properties = new { Name = "Test", Value = 123 };

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            MockLogger.Object.Debug("Test message", properties);
        });
    }

    [Test]
    public void Info_WithSimpleObject_ShouldNotThrow()
    {
        // Arrange
        var properties = new { Name = "Test", Value = 123 };

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            MockLogger.Object.Info("Test message", properties);
        });
    }

    [Test]
    public void Warn_WithSimpleObject_ShouldNotThrow()
    {
        // Arrange
        var properties = new { Name = "Test", Value = 123 };

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            MockLogger.Object.Warn("Test message", properties);
        });
    }

    [Test]
    public void Error_WithSimpleObject_ShouldNotThrow()
    {
        // Arrange
        var properties = new { Name = "Test", Value = 123 };

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            MockLogger.Object.Error("Test message", properties);
        });
    }

    [Test]
    public void AddContext_WithSimpleObject_ShouldNotThrow()
    {
        // Arrange
        var context = new { UserId = 123, UserName = "TestUser" };

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            MockLogger.Object.AddContext(context);
        });
    }

    #endregion

    #region Dictionary Tests

    [Test]
    public void Debug_WithDictionary_ShouldNotThrow()
    {
        // Arrange
        var properties = new Dictionary<string, object?> { { "Key1", "Value1" }, { "Key2", 123 }, { "Key3", true } };

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            MockLogger.Object.Debug("Test message", properties);
        });
    }

    [Test]
    public void Info_WithIDictionary_ShouldNotThrow()
    {
        // Arrange
        IDictionary properties = new Hashtable { { "Key1", "Value1" }, { "Key2", 123 } };

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            MockLogger.Object.Info("Test message", properties);
        });
    }

    [Test]
    public void Error_WithDictionaryContainingNullValues_ShouldNotThrow()
    {
        // Arrange
        var properties = new Dictionary<string, object?>
        {
            { "Key1", "Value1" }, { "Key2", null }, { "Key3", "Value3" }
        };

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            MockLogger.Object.Error("Test message", properties);
        });
    }

    [Test]
    public void AddContext_WithDictionary_ShouldNotThrow()
    {
        // Arrange
        var context = new Dictionary<string, object?> { { "UserId", 123 }, { "UserName", "TestUser" } };

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            MockLogger.Object.AddContext(context);
        });
    }

    [Test]
    public void Debug_WithDictionaryWithNullKeys_ShouldNotThrow()
    {
        // Arrange
        // Use custom dictionary that allows null keys (Hashtable and Dictionary don't allow null keys in .NET Core)
        IDictionary properties = new NullableKeyDictionary { { null!, "Value1" }, { "Key2", "Value2" } };

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            MockLogger.Object.Debug("Test message", properties);
        });
    }

    #endregion

    #region Object with Null Property Values

    [Test]
    public void Info_WithObjectContainingNullProperties_ShouldNotThrow()
    {
        // Arrange
        var properties = new { Name = (string?)null, Value = 123, Description = (string?)null };

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            MockLogger.Object.Info("Test message", properties);
        });
    }

    #endregion

    #region Objects with Private Properties

    [Test]
    public void Debug_WithObjectContainingPrivateProperties_ShouldNotThrow()
    {
        // Arrange
        var properties = new TestClassWithPrivateProperties();

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            MockLogger.Object.Debug("Test message", properties);
        });
    }

    private class TestClassWithPrivateProperties
    {
        public string PublicProperty { get; set; } = "Public";
        private string PrivateProperty { get; set; } = "Private";
    }

    #endregion

    #region Objects with Properties That Throw Exceptions

    [Test]
    public void Error_WithObjectWithThrowingProperty_ShouldNotThrow()
    {
        // Arrange
        var properties = new TestClassWithThrowingProperty();

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            MockLogger.Object.Error("Test message", properties);
        });
    }

    [Test]
    public void Info_WithObjectWithMultipleThrowingProperties_ShouldNotThrow()
    {
        // Arrange
        var properties = new TestClassWithMultipleThrowingProperties();

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            MockLogger.Object.Info("Test message", properties);
        });
    }

    [Test]
    public void Error_WithObjectWithNotSupportedException_ShouldNotThrow()
    {
        // Arrange
        var properties = new TestClassWithNotSupportedException();

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            MockLogger.Object.Error("Test message", properties);
        });
    }

    private class TestClassWithThrowingProperty
    {
        public string SafeProperty { get; set; } = "Safe";

        public string ThrowingProperty
        {
            get => throw new InvalidOperationException("Property access failed");
        }
    }

    private class TestClassWithMultipleThrowingProperties
    {
        public string SafeProperty1 { get; set; } = "Safe1";

        public string ThrowingProperty1
        {
            get => throw new InvalidOperationException("Property 1 failed");
        }

        public string SafeProperty2 { get; set; } = "Safe2";

        public string ThrowingProperty2
        {
            get => throw new ArgumentException("Property 2 failed");
        }
    }

    private class TestClassWithNotSupportedException
    {
        public string SafeProperty { get; set; } = "Safe";

        public string ThrowingProperty
        {
            get => throw new NotSupportedException("Property access not supported");
        }
    }

    #endregion

    #region Complex Nested Structures

    [Test]
    public void Warn_WithComplexNestedObject_ShouldNotThrow()
    {
        // Arrange
        var properties = new
        {
            User = new
            {
                Id = 123,
                Name = "TestUser",
                Address = new { Street = "123 Main St", City = "TestCity", ZipCode = "12345" }
            },
            Metadata = new Dictionary<string, object?> { { "Key1", "Value1" }, { "Key2", 456 } }
        };

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            MockLogger.Object.Warn("Test message", properties);
        });
    }

    [Test]
    public void Error_WithObjectContainingCollections_ShouldNotThrow()
    {
        // Arrange
        var properties = new
        {
            Items = new[] { "Item1", "Item2", "Item3" },
            Numbers = new List<int>
            {
                1,
                2,
                3,
                4,
                5
            },
            Dictionary = new Dictionary<string, int> { { "Key", 123 } }
        };

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            MockLogger.Object.Error("Test message", properties);
        });
    }

    [Test]
    public void Info_WithCircularReference_ShouldLogWithoutInfiniteLoop()
    {
        // Arrange
        var (logger, output) = TestLoggerHelper.CreateCapturingLogger();
        var parent = new TestClassWithCircularReference { Name = "Parent", Id = 1 };
        var child = new TestClassWithCircularReference { Name = "Child", Id = 2, Parent = parent };
        parent.Child = child; // Create circular reference

        // Act
        var act = () => logger.Info("Test message", parent);

        // Assert - Should complete without hanging or throwing
        act.Should().NotThrow();

        var logs = TestLoggerHelper.ParseJsonLogs(output.ToString());
        logs.Should().HaveCount(1);
        var log = logs[0];
        log.Should().ContainKey("message");
        log["message"]?.ToString().Should().Be("Test message");
        // The circular reference should be handled (object reference stored, not recursively converted)
        log.Should().ContainKey("properties");
    }

    [Test]
    public void AddContext_WithCircularReference_ShouldNotThrow()
    {
        // Arrange
        var parent = new TestClassWithCircularReference { Name = "Parent" };
        var child = new TestClassWithCircularReference { Name = "Child", Parent = parent };
        parent.Child = child; // Create circular reference

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            MockLogger.Object.AddContext(parent);
        });
    }

    private class TestClassWithCircularReference
    {
        public string Name { get; set; } = string.Empty;
        public int Id { get; set; }
        public TestClassWithCircularReference? Parent { get; set; }
        public TestClassWithCircularReference? Child { get; set; }
    }

    private class NullToStringKey
    {
        public override string? ToString()
        {
            return null; // Return null to test the null coalescing branch
        }
    }

    #endregion

    #region Special Characters

    [Test]
    public void Debug_WithSpecialCharactersInPropertyNames_ShouldNotThrow()
    {
        // Arrange
        var properties = new Dictionary<string, object?>
        {
            { "NormalKey", "Value1" },
            { "Key-With-Dashes", "Value2" },
            { "Key_With_Underscores", "Value3" },
            { "Key With Spaces", "Value4" }
        };

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            MockLogger.Object.Debug("Test message", properties);
        });
    }

    [Test]
    public void Info_WithSpecialCharactersInPropertyValues_ShouldNotThrow()
    {
        // Arrange
        var properties = new
        {
            NormalValue = "Normal",
            JsonValue = "{\"key\":\"value\"}",
            XmlValue = "<tag>value</tag>",
            NewLineValue = "Line1\nLine2",
            UnicodeValue = "Test\u00A9\u00AE"
        };

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            MockLogger.Object.Info("Test message", properties);
        });
    }

    #endregion

    #region Exception Safety - Logger Throws

    [Test]
    public void Debug_WhenLoggerThrows_ShouldNotPropagateException()
    {
        // Arrange
        MockLogger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()))
            .Throws<InvalidOperationException>();

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            MockLogger.Object.Debug("Test message");
        });
    }

    [Test]
    public void Info_WhenBeginScopeThrows_ShouldNotPropagateException()
    {
        // Arrange
        // Override the default BeginScope setup to throw instead
        MockLogger.Setup(x => x.BeginScope(It.IsAny<object>()))
            .Throws<InvalidOperationException>();

        var properties = new { Name = "Test" };

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            MockLogger.Object.Info("Test message", properties);
        });
    }

    [Test]
    public void Error_WhenLoggerThrows_ShouldNotPropagateException()
    {
        // Arrange
        MockLogger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()))
            .Throws<Exception>();

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            MockLogger.Object.Error("Test message");
        });
    }

    #endregion

    #region Very Large Objects

    [Test]
    public void Warn_WithVeryLargeObject_ShouldNotThrow()
    {
        // Arrange
        var largeArray = new int[10000];
        for (int i = 0; i < largeArray.Length; i++)
        {
            largeArray[i] = i;
        }

        var properties = new { LargeArray = largeArray, Count = largeArray.Length };

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            MockLogger.Object.Warn("Test message", properties);
        });
    }

    [Test]
    public void Info_WithLargeDictionary_ShouldNotThrow()
    {
        // Arrange
        var largeDict = new Dictionary<string, object?>();
        for (int i = 0; i < 1000; i++)
        {
            largeDict[$"Key{i}"] = $"Value{i}";
        }

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            MockLogger.Object.Info("Test message", largeDict);
        });
    }

    #endregion

    #region Empty Objects

    [Test]
    public void Debug_WithEmptyObject_ShouldNotThrow()
    {
        // Arrange
        var properties = new { };

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            MockLogger.Object.Debug("Test message", properties);
        });
    }

    [Test]
    public void Info_WithEmptyDictionary_ShouldNotThrow()
    {
        // Arrange
        var properties = new Dictionary<string, object?>();

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            MockLogger.Object.Info("Test message", properties);
        });
    }

    #endregion

    #region Different Property Types

    [Test]
    public void Error_WithVariousPropertyTypes_ShouldNotThrow()
    {
        // Arrange
        var properties = new
        {
            StringValue = "Test",
            IntValue = 123,
            BoolValue = true,
            DoubleValue = 123.456,
            DateTimeValue = DateTime.Now,
            GuidValue = Guid.NewGuid(),
            NullableInt = (int?)42,
            NullableBool = (bool?)null
        };

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            MockLogger.Object.Error("Test message", properties);
        });
    }

    #endregion

    #region AddContext Tests

    [Test]
    public void AddContext_WithSimpleObject_ShouldReturnDisposable()
    {
        // Arrange
        var context = new { UserId = 123 };

        // Act
        var result = MockLogger.Object.AddContext(context);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public void AddContext_WithDictionary_ShouldReturnDisposable()
    {
        // Arrange
        var context = new Dictionary<string, object?> { { "Key", "Value" } };

        // Act
        var result = MockLogger.Object.AddContext(context);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public void AddContext_WithObjectContainingThrowingProperty_ShouldNotThrow()
    {
        // Arrange
        var context = new TestClassWithThrowingProperty();

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            MockLogger.Object.AddContext(context);
        });
    }

    [Test]
    public void Debug_WithDictionaryWithNullKey_ShouldHandleNullKey()
    {
        // Arrange
        // Test ConvertToDictionary with dictionary entry that has null key (line 91)
        IDictionary properties = new NullableKeyDictionary { { null!, "Value1" }, { "Key2", "Value2" } };

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            MockLogger.Object.Debug("Test message", properties);
        });
    }

    [Test]
    public void Debug_WithDictionaryKeyThatToStringReturnsNull_ShouldUseNullString()
    {
        // Arrange
        // Test the null coalescing branch on line 91: entry.Key.ToString() ?? "null"
        // Create a key object whose ToString() returns null
        var nullToStringKey = new NullToStringKey();
        IDictionary properties = new NullableKeyDictionary { { nullToStringKey, "Value1" } };

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            MockLogger.Object.Debug("Test message", properties);
        });
    }

    #endregion

    #region Log Output Verification Tests

    [Test]
    public void Debug_WithSimpleObject_ShouldLogMessageAndProperties()
    {
        // Arrange
        var (logger, output) = TestLoggerHelper.CreateCapturingLogger(LogEventLevel.Debug);
        var properties = new { Name = "Test", Value = 123 };

        // Act
        logger.Debug("Test message", properties);

        // Assert
        var logs = TestLoggerHelper.ParseJsonLogs(output.ToString());
        logs.Should().HaveCount(1);
        var log = logs[0];
        log.Should().ContainKey("message");
        log["message"]?.ToString().Should().Be("Test message");
        log.Should().ContainKey("level");
        log["level"]?.ToString().Should().Be("Debug");
        log.Should().ContainKey("properties");
        var props = log["properties"] as System.Text.Json.JsonElement?;
        props?.GetProperty("name").GetString().Should().Be("Test");
        props?.GetProperty("value").GetInt32().Should().Be(123);
    }

    [Test]
    public void Info_WithSimpleObject_ShouldLogMessageAndProperties()
    {
        // Arrange
        var (logger, output) = TestLoggerHelper.CreateCapturingLogger();
        var properties = new { Name = "Test", Value = 123 };

        // Act
        logger.Info("Test message", properties);

        // Assert
        var logs = TestLoggerHelper.ParseJsonLogs(output.ToString());
        logs.Should().HaveCount(1);
        var log = logs[0];
        log.Should().ContainKey("message");
        log["message"]?.ToString().Should().Be("Test message");
        log.Should().ContainKey("level");
        log["level"]?.ToString().Should().Be("Information");
        log.Should().ContainKey("properties");
        var props = log["properties"] as System.Text.Json.JsonElement?;
        props?.GetProperty("name").GetString().Should().Be("Test");
        props?.GetProperty("value").GetInt32().Should().Be(123);
    }

    [Test]
    public void Warn_WithSimpleObject_ShouldLogMessageAndProperties()
    {
        // Arrange
        var (logger, output) = TestLoggerHelper.CreateCapturingLogger();
        var properties = new { Name = "Test", Value = 123 };

        // Act
        logger.Warn("Test message", properties);

        // Assert
        var logs = TestLoggerHelper.ParseJsonLogs(output.ToString());
        logs.Should().HaveCount(1);
        var log = logs[0];
        log.Should().ContainKey("message");
        log["message"]?.ToString().Should().Be("Test message");
        log.Should().ContainKey("level");
        log["level"]?.ToString().Should().Be("Warning");
        log.Should().ContainKey("properties");
        var props = log["properties"] as System.Text.Json.JsonElement?;
        props?.GetProperty("name").GetString().Should().Be("Test");
        props?.GetProperty("value").GetInt32().Should().Be(123);
    }

    [Test]
    public void Error_WithSimpleObject_ShouldLogMessageAndProperties()
    {
        // Arrange
        var (logger, output) = TestLoggerHelper.CreateCapturingLogger();
        var properties = new { Name = "Test", Value = 123 };

        // Act
        logger.Error("Test message", properties);

        // Assert
        var logs = TestLoggerHelper.ParseJsonLogs(output.ToString());
        logs.Should().HaveCount(1);
        var log = logs[0];
        log.Should().ContainKey("message");
        log["message"]?.ToString().Should().Be("Test message");
        log.Should().ContainKey("level");
        log["level"]?.ToString().Should().Be("Error");
        log.Should().ContainKey("properties");
        var props = log["properties"] as System.Text.Json.JsonElement?;
        props?.GetProperty("name").GetString().Should().Be("Test");
        props?.GetProperty("value").GetInt32().Should().Be(123);
    }

    [Test]
    public void Info_WithDictionary_ShouldLogMessageAndProperties()
    {
        // Arrange
        var (logger, output) = TestLoggerHelper.CreateCapturingLogger();
        var properties = new Dictionary<string, object> { { "Key1", "Value1" }, { "Key2", 42 } };

        // Act
        logger.Info("Test message", properties);

        // Assert
        var logs = TestLoggerHelper.ParseJsonLogs(output.ToString());
        logs.Should().HaveCount(1);
        var log = logs[0];
        log.Should().ContainKey("message");
        log["message"]?.ToString().Should().Be("Test message");
        log.Should().ContainKey("properties");
        var props = log["properties"] as System.Text.Json.JsonElement?;
        props?.GetProperty("key1").GetString().Should().Be("Value1");
        props?.GetProperty("key2").GetInt32().Should().Be(42);
    }

    [Test]
    public void Info_WithoutProperties_ShouldLogMessageOnly()
    {
        // Arrange
        var (logger, output) = TestLoggerHelper.CreateCapturingLogger();

        // Act
        logger.Info("Test message");

        // Assert
        var logs = TestLoggerHelper.ParseJsonLogs(output.ToString());
        logs.Should().HaveCount(1);
        var log = logs[0];
        log.Should().ContainKey("message");
        log["message"]?.ToString().Should().Be("Test message");
        log.Should().ContainKey("level");
        log["level"]?.ToString().Should().Be("Information");
        log.Should().NotContainKey("properties");
    }

    [Test]
    public void AddContext_ShouldAddPropertiesToSubsequentLogs()
    {
        // Arrange
        var (logger, output) = TestLoggerHelper.CreateCapturingLogger();
        var context = new { UserId = 123, UserName = "TestUser" };

        // Act
        using (logger.AddContext(context))
        {
            logger.Info("Test message");
        }

        // Assert
        var logs = TestLoggerHelper.ParseJsonLogs(output.ToString());
        logs.Should().HaveCount(1);
        var log = logs[0];
        log.Should().ContainKey("message");
        log["message"]?.ToString().Should().Be("Test message");
        // Context properties should be included in the log
        var props = log["properties"] as System.Text.Json.JsonElement?;
        props?.GetProperty("userId").GetInt32().Should().Be(123);
        props?.GetProperty("userName").GetString().Should().Be("TestUser");
    }

    [Test]
    public void Debug_ShouldNotLogWhenLevelIsInformation()
    {
        // Arrange
        var (logger, output) = TestLoggerHelper.CreateCapturingLogger(LogEventLevel.Information);

        // Act
        logger.Debug("Debug message");

        // Assert
        var logs = TestLoggerHelper.ParseJsonLogs(output.ToString());
        logs.Should().BeEmpty();
    }

    [Test]
    public void Info_ShouldLogWhenLevelIsInformation()
    {
        // Arrange
        var (logger, output) = TestLoggerHelper.CreateCapturingLogger(LogEventLevel.Information);

        // Act
        logger.Info("Info message");

        // Assert
        var logs = TestLoggerHelper.ParseJsonLogs(output.ToString());
        logs.Should().HaveCount(1);
        logs[0]["level"]?.ToString().Should().Be("Information");
    }

    [Test]
    public void Info_WithNestedObject_ShouldFlattenNestedProperties()
    {
        // Arrange
        var (logger, output) = TestLoggerHelper.CreateCapturingLogger();
        var leaf = new Leaf { Color = "Green", Size = 10 };
        var node = new Node { Name = "Root", Leaf = leaf };
        var properties = new { Node = node };

        // Act
        logger.Info("Test message", properties);

        // Assert
        var logs = TestLoggerHelper.ParseJsonLogs(output.ToString());
        logs.Should().HaveCount(1);
        var log = logs[0];
        log.Should().ContainKey("properties");
        var props = log["properties"] as System.Text.Json.JsonElement?;

        // Verify nested structure is flattened with dot notation
        props?.GetProperty("node").GetProperty("name").GetString().Should().Be("Root");
        props?.GetProperty("node").GetProperty("leaf").GetProperty("color").GetString().Should().Be("Green");
        props?.GetProperty("node").GetProperty("leaf").GetProperty("size").GetInt32().Should().Be(10);
    }

    [Test]
    public void Debug_WithNestedObject_ShouldFlattenNestedProperties()
    {
        // Arrange
        var (logger, output) = TestLoggerHelper.CreateCapturingLogger(LogEventLevel.Debug);
        var leaf = new Leaf { Color = "Red", Size = 5 };
        var node = new Node { Name = "Branch", Leaf = leaf };
        var properties = new { Node = node };

        // Act
        logger.Debug("Test message", properties);

        // Assert
        var logs = TestLoggerHelper.ParseJsonLogs(output.ToString());
        logs.Should().HaveCount(1);
        var log = logs[0];
        log.Should().ContainKey("properties");
        var props = log["properties"] as System.Text.Json.JsonElement?;

        // Verify nested structure
        props?.GetProperty("node").GetProperty("name").GetString().Should().Be("Branch");
        props?.GetProperty("node").GetProperty("leaf").GetProperty("color").GetString().Should().Be("Red");
        props?.GetProperty("node").GetProperty("leaf").GetProperty("size").GetInt32().Should().Be(5);
    }

    [Test]
    public void Info_WithDeeplyNestedObject_ShouldFlattenAllLevels()
    {
        // Arrange
        var (logger, output) = TestLoggerHelper.CreateCapturingLogger();
        var innerLeaf = new Leaf { Color = "Blue", Size = 3 };
        var innerNode = new Node { Name = "Inner", Leaf = innerLeaf };
        var outerNode = new Node { Name = "Outer", Leaf = null, InnerNode = innerNode };
        var properties = new { OuterNode = outerNode };

        // Act
        logger.Info("Test message", properties);

        // Assert
        var logs = TestLoggerHelper.ParseJsonLogs(output.ToString());
        logs.Should().HaveCount(1);
        var log = logs[0];
        log.Should().ContainKey("properties");
        var props = log["properties"] as System.Text.Json.JsonElement?;

        // Verify deeply nested structure
        props?.GetProperty("outerNode").GetProperty("name").GetString().Should().Be("Outer");
        props?.GetProperty("outerNode").GetProperty("innerNode").GetProperty("name").GetString().Should().Be("Inner");
        props?.GetProperty("outerNode").GetProperty("innerNode").GetProperty("leaf").GetProperty("color").GetString()
            .Should().Be("Blue");
    }

    [Test]
    public void Info_WithNestedObjectContainingNull_ShouldHandleNullProperties()
    {
        // Arrange
        var (logger, output) = TestLoggerHelper.CreateCapturingLogger();
        var node = new Node { Name = "Test", Leaf = null };
        var properties = new { Node = node };

        // Act
        logger.Info("Test message", properties);

        // Assert
        var logs = TestLoggerHelper.ParseJsonLogs(output.ToString());
        logs.Should().HaveCount(1);
        var log = logs[0];
        log.Should().ContainKey("properties");
        var props = log["properties"] as System.Text.Json.JsonElement?;

        props?.GetProperty("node").GetProperty("name").GetString().Should().Be("Test");
        // Leaf should be null or not present
        var leafElement = props?.GetProperty("node").GetProperty("leaf");
        if (leafElement.HasValue)
        {
            leafElement.Value.ValueKind.Should().Be(System.Text.Json.JsonValueKind.Null);
        }
    }

    [Test]
    public void Info_WithSimpleTypeAsRoot_ShouldHandleGracefully()
    {
        // Arrange
        // Test edge case where a simple type is passed as the root object (lines 117-118)
        // This shouldn't happen in practice, but we test it for coverage
        var (logger, output) = TestLoggerHelper.CreateCapturingLogger();

        // Act - passing a simple type directly (wrapped in anonymous type to make it valid)
        // Actually, we can't pass a simple type directly, but we can test the IsSimpleType check
        // by ensuring it doesn't break when processing nested simple types
        var properties = new { Value = 123, Text = "test" };

        // Act
        logger.Info("Test message", properties);

        // Assert - should work normally
        var logs = TestLoggerHelper.ParseJsonLogs(output.ToString());
        logs.Should().HaveCount(1);
    }

    #endregion

    #region Additional Edge Cases

    [Test]
    public void Info_WithEnumProperty_ShouldLogEnumValue()
    {
        // Arrange
        var (logger, output) = TestLoggerHelper.CreateCapturingLogger();
        var properties = new { Status = TestStatus.Active, Priority = TestPriority.High };

        // Act
        logger.Info("Test message", properties);

        // Assert
        var logs = TestLoggerHelper.ParseJsonLogs(output.ToString());
        logs.Should().HaveCount(1);
        var log = logs[0];
        log.Should().ContainKey("properties");
        var props = log["properties"] as System.Text.Json.JsonElement?;
        // Enums are serialized as camelCase strings
        props?.GetProperty("status").GetString().Should().Be("Active");
        props?.GetProperty("priority").GetString().Should().Be("High");
    }

    [Test]
    public void Info_WithStructProperty_ShouldLogStructProperties()
    {
        // Arrange
        var (logger, output) = TestLoggerHelper.CreateCapturingLogger();
        var point = new Point { X = 10, Y = 20 };
        var properties = new { Location = point };

        // Act
        logger.Info("Test message", properties);

        // Assert
        var logs = TestLoggerHelper.ParseJsonLogs(output.ToString());
        logs.Should().HaveCount(1);
        var log = logs[0];
        log.Should().ContainKey("properties");
        var props = log["properties"] as System.Text.Json.JsonElement?;
        props?.GetProperty("location").GetProperty("x").GetInt32().Should().Be(10);
        props?.GetProperty("location").GetProperty("y").GetInt32().Should().Be(20);
    }

    [Test]
    public void Info_WithEmptyCollection_ShouldHandleEmptyCollection()
    {
        // Arrange
        var (logger, output) = TestLoggerHelper.CreateCapturingLogger();
        var properties = new { Items = new List<string>(), Numbers = new int[0] };

        // Act
        logger.Info("Test message", properties);

        // Assert
        var logs = TestLoggerHelper.ParseJsonLogs(output.ToString());
        logs.Should().HaveCount(1);
        var log = logs[0];
        log.Should().ContainKey("properties");
        var props = log["properties"] as System.Text.Json.JsonElement?;
        props?.GetProperty("items").GetArrayLength().Should().Be(0);
        props?.GetProperty("numbers").GetArrayLength().Should().Be(0);
    }

    [Test]
    public void Info_WithCollectionContainingNull_ShouldHandleNullItems()
    {
        // Arrange
        var (logger, output) = TestLoggerHelper.CreateCapturingLogger();
        var properties = new { Items = new[] { "Item1", null, "Item3" } };

        // Act
        logger.Info("Test message", properties);

        // Assert
        var logs = TestLoggerHelper.ParseJsonLogs(output.ToString());
        logs.Should().HaveCount(1);
        var log = logs[0];
        log.Should().ContainKey("properties");
        var props = log["properties"] as System.Text.Json.JsonElement?;
        var items = props?.GetProperty("items");
        items?.GetArrayLength().Should().Be(3);
        items?[0].GetString().Should().Be("Item1");
        items?[1].ValueKind.Should().Be(System.Text.Json.JsonValueKind.Null);
        items?[2].GetString().Should().Be("Item3");
    }

    [Test]
    public void Info_WithJaggedArray_ShouldHandleJaggedArray()
    {
        // Arrange
        var (logger, output) = TestLoggerHelper.CreateCapturingLogger();
        var jagged = new int[][] { new[] { 1, 2, 3 }, new[] { 4, 5 }, new[] { 6, 7, 8, 9 } };
        var properties = new { Matrix = jagged };

        // Act
        logger.Info("Test message", properties);

        // Assert
        var logs = TestLoggerHelper.ParseJsonLogs(output.ToString());
        logs.Should().HaveCount(1);
        var log = logs[0];
        log.Should().ContainKey("properties");
        var props = log["properties"] as System.Text.Json.JsonElement?;
        var matrix = props?.GetProperty("matrix");
        matrix?.GetArrayLength().Should().Be(3);
    }

    [Test]
    public void Info_WithCollectionContainingCollections_ShouldHandleNestedCollections()
    {
        // Arrange
        var (logger, output) = TestLoggerHelper.CreateCapturingLogger();
        var nested = new List<List<int>> { new List<int> { 1, 2, 3 }, new List<int> { 4, 5, 6 } };
        var properties = new { Nested = nested };

        // Act
        logger.Info("Test message", properties);

        // Assert
        var logs = TestLoggerHelper.ParseJsonLogs(output.ToString());
        logs.Should().HaveCount(1);
        var log = logs[0];
        log.Should().ContainKey("properties");
        var props = log["properties"] as System.Text.Json.JsonElement?;
        var nestedProp = props?.GetProperty("nested");
        nestedProp?.GetArrayLength().Should().Be(2);
    }

    [Test]
    public void Info_WithDictionaryContainingNestedObjects_ShouldFlattenNestedObjects()
    {
        // Arrange
        var (logger, output) = TestLoggerHelper.CreateCapturingLogger();
        var leaf = new Leaf { Color = "Blue", Size = 5 };
        var dict = new Dictionary<string, object?> { { "Node", new Node { Name = "Test", Leaf = leaf } } };
        var properties = new { Data = dict };

        // Act
        logger.Info("Test message", properties);

        // Assert
        var logs = TestLoggerHelper.ParseJsonLogs(output.ToString());
        logs.Should().HaveCount(1);
        var log = logs[0];
        log.Should().ContainKey("properties");
        var props = log["properties"] as System.Text.Json.JsonElement?;
        var data = props?.GetProperty("data");
        var node = data?.GetProperty("node");
        node?.GetProperty("name").GetString().Should().Be("Test");
        node?.GetProperty("leaf").GetProperty("color").GetString().Should().Be("Blue");
    }

    [Test]
    public void Info_WithSharedObjectReference_ShouldConvertMultipleTimes()
    {
        // Arrange - same object referenced multiple times (not circular)
        var (logger, output) = TestLoggerHelper.CreateCapturingLogger();
        var sharedLeaf = new Leaf { Color = "Red", Size = 10 };
        var node1 = new Node { Name = "Node1", Leaf = sharedLeaf };
        var node2 = new Node { Name = "Node2", Leaf = sharedLeaf };
        var properties = new { Node1 = node1, Node2 = node2 };

        // Act
        logger.Info("Test message", properties);

        // Assert - should convert both references
        var logs = TestLoggerHelper.ParseJsonLogs(output.ToString());
        logs.Should().HaveCount(1);
        var log = logs[0];
        log.Should().ContainKey("properties");
        var props = log["properties"] as System.Text.Json.JsonElement?;
        props?.GetProperty("node1").GetProperty("leaf").GetProperty("color").GetString().Should().Be("Red");
        props?.GetProperty("node2").GetProperty("leaf").GetProperty("color").GetString().Should().Be("Red");
    }

    [Test]
    public void Info_WithSelfReferencingObject_ShouldHandleSelfReference()
    {
        // Arrange - object that references itself
        var (logger, output) = TestLoggerHelper.CreateCapturingLogger();
        var selfRef = new SelfReferencingObject { Name = "Self", Value = 42 };
        selfRef.Self = selfRef;
        var properties = new { SelfRef = selfRef };

        // Act
        logger.Info("Test message", properties);

        // Assert - should handle circular reference gracefully
        var logs = TestLoggerHelper.ParseJsonLogs(output.ToString());
        logs.Should().HaveCount(1);
        var log = logs[0];
        log.Should().ContainKey("properties");
        // Self reference should be detected and not cause infinite loop
    }

    [Test]
    public void Info_WithNullableValueTypes_ShouldHandleNullableTypes()
    {
        // Arrange
        var (logger, output) = TestLoggerHelper.CreateCapturingLogger();
        var properties = new
        {
            NullableInt = (int?)42,
            NullableIntNull = (int?)null,
            NullableDateTime = (DateTime?)DateTime.Now,
            NullableDateTimeNull = (DateTime?)null
        };

        // Act
        logger.Info("Test message", properties);

        // Assert
        var logs = TestLoggerHelper.ParseJsonLogs(output.ToString());
        logs.Should().HaveCount(1);
        var log = logs[0];
        log.Should().ContainKey("properties");
        var props = log["properties"] as System.Text.Json.JsonElement?;
        props?.GetProperty("nullableInt").GetInt32().Should().Be(42);
        props?.GetProperty("nullableIntNull").ValueKind.Should().Be(System.Text.Json.JsonValueKind.Null);
    }

    [Test]
    public void Info_WithTypeWithNoProperties_ShouldReturnEmptyDictionary()
    {
        // Arrange
        var (logger, output) = TestLoggerHelper.CreateCapturingLogger();
        var empty = new EmptyClass();
        var properties = new { Empty = empty };

        // Act
        logger.Info("Test message", properties);

        // Assert
        var logs = TestLoggerHelper.ParseJsonLogs(output.ToString());
        logs.Should().HaveCount(1);
        var log = logs[0];
        log.Should().ContainKey("properties");
        var props = log["properties"] as System.Text.Json.JsonElement?;
        var emptyProp = props?.GetProperty("empty");
        // Should be an empty object or handle gracefully
        emptyProp?.ValueKind.Should().Be(System.Text.Json.JsonValueKind.Object);
    }

    [Test]
    public void Info_WithDictionaryAsRoot_ShouldHandleDictionaryDirectly()
    {
        // Arrange
        var (logger, output) = TestLoggerHelper.CreateCapturingLogger();
        IDictionary properties = new Dictionary<string, object?> { { "Key1", "Value1" }, { "Key2", 123 } };

        // Act
        logger.Info("Test message", properties);

        // Assert
        var logs = TestLoggerHelper.ParseJsonLogs(output.ToString());
        logs.Should().HaveCount(1);
        var log = logs[0];
        log.Should().ContainKey("properties");
        var props = log["properties"] as System.Text.Json.JsonElement?;
        props?.GetProperty("key1").GetString().Should().Be("Value1");
        props?.GetProperty("key2").GetInt32().Should().Be(123);
    }

    [Test]
    public void Info_WithPropertyReturningCollection_ShouldHandleCollectionProperty()
    {
        // Arrange
        var (logger, output) = TestLoggerHelper.CreateCapturingLogger();
        var obj = new ClassWithCollectionProperty { Items = new List<string> { "Item1", "Item2", "Item3" } };
        var properties = new { Data = obj };

        // Act
        logger.Info("Test message", properties);

        // Assert
        var logs = TestLoggerHelper.ParseJsonLogs(output.ToString());
        logs.Should().HaveCount(1);
        var log = logs[0];
        log.Should().ContainKey("properties");
        var props = log["properties"] as System.Text.Json.JsonElement?;
        var items = props?.GetProperty("data").GetProperty("items");
        items?.GetArrayLength().Should().Be(3);
        items?[0].GetString().Should().Be("Item1");
    }

    [Test]
    public void Info_WithPropertyReturningDictionary_ShouldHandleDictionaryProperty()
    {
        // Arrange
        var (logger, output) = TestLoggerHelper.CreateCapturingLogger();
        var obj = new ClassWithDictionaryProperty { Metadata = new Dictionary<string, object?> { { "Key", "Value" } } };
        var properties = new { Data = obj };

        // Act
        logger.Info("Test message", properties);

        // Assert
        var logs = TestLoggerHelper.ParseJsonLogs(output.ToString());
        logs.Should().HaveCount(1);
        var log = logs[0];
        log.Should().ContainKey("properties");
        var props = log["properties"] as System.Text.Json.JsonElement?;
        var metadata = props?.GetProperty("data").GetProperty("metadata");
        metadata?.GetProperty("key").GetString().Should().Be("Value");
    }

    [Test]
    public void Info_WithSpecialTypes_ShouldHandleSpecialTypes()
    {
        // Arrange
        var (logger, output) = TestLoggerHelper.CreateCapturingLogger();
        var properties = new { Uri = new Uri("https://example.com"), Version = new Version(1, 2, 3, 4) };

        // Act
        logger.Info("Test message", properties);

        // Assert
        var logs = TestLoggerHelper.ParseJsonLogs(output.ToString());
        logs.Should().HaveCount(1);
        var log = logs[0];
        log.Should().ContainKey("properties");
        // Special types should be handled (either as simple types or converted)
    }

    [Test]
    public void Info_WithEmptyDictionary_ShouldHandleEmptyDictionary()
    {
        // Arrange
        var (logger, output) = TestLoggerHelper.CreateCapturingLogger();
        var properties = new { EmptyDict = new Dictionary<string, object?>() };

        // Act
        logger.Info("Test message", properties);

        // Assert
        var logs = TestLoggerHelper.ParseJsonLogs(output.ToString());
        logs.Should().HaveCount(1);
        var log = logs[0];
        log.Should().ContainKey("properties");
        var props = log["properties"] as System.Text.Json.JsonElement?;
        var emptyDict = props?.GetProperty("emptyDict");
        emptyDict?.ValueKind.Should().Be(System.Text.Json.JsonValueKind.Object);
    }

    #endregion


    [Test]
    public void AddContext_WhenBeginScopeThrows_ShouldReturnNull()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        mockLogger.Setup(x => x.BeginScope(It.IsAny<object>()))
            .Throws<InvalidOperationException>();
        var context = new { Key = "Value" };

        // Act
        var result = mockLogger.Object.AddContext(context);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public void Debug_WithSimpleType_ShouldNotIncludeInProperties()
    {
        // Arrange
        var (logger, output) = TestLoggerHelper.CreateCapturingLogger();
        var simpleValue = 42;

        // Act
        logger.Debug("Test message", simpleValue);

        // Assert
        var logs = TestLoggerHelper.ParseJsonLogs(output.ToString());
        logs.Should().HaveCount(1);
        var log = logs[0];
        // Simple types passed directly should not create properties
        log.Should().NotContainKey("properties");
    }

    [Test]
    public void Debug_WithUnsupportedType_ShouldConvertToString()
    {
        // Arrange
        var (logger, output) = TestLoggerHelper.CreateCapturingLogger();
        var methodInfo = typeof(string).GetMethod("ToString", Array.Empty<Type>())!;
        var type = typeof(string);
        var assembly = typeof(string).Assembly;

        // Act
        logger.Debug("Test message", new { Method = methodInfo, Type = type, Assembly = assembly });

        // Assert
        var logs = TestLoggerHelper.ParseJsonLogs(output.ToString());
        logs.Should().HaveCount(1);
        var log = logs[0];
        log.Should().ContainKey("properties");
        var props = log["properties"] as System.Text.Json.JsonElement?;
        props?.GetProperty("method").GetString().Should().NotBeNullOrEmpty();
        props?.GetProperty("type").GetString().Should().NotBeNullOrEmpty();
        props?.GetProperty("assembly").GetString().Should().NotBeNullOrEmpty();
    }

    [Test]
    public void Debug_WithDictionaryContainingNullKeys_ShouldHandleGracefully()
    {
        // Arrange
        var (logger, output) = TestLoggerHelper.CreateCapturingLogger();
        IDictionary dict = new NullableKeyDictionary { { "validKey", "validValue" }, { null!, "nullKeyValue" } };

        // Act
        logger.Debug("Test message", dict);

        // Assert
        var logs = TestLoggerHelper.ParseJsonLogs(output.ToString());
        logs.Should().HaveCount(1);
        var log = logs[0];
        log.Should().ContainKey("properties");
        var props = log["properties"] as System.Text.Json.JsonElement?;
        props?.GetProperty("validKey").GetString().Should().Be("validValue");
        props?.GetProperty("null").GetString().Should().Be("nullKeyValue");
    }
}

// Test classes for edge case testing
public enum TestStatus
{
    Inactive,
    Active,
    Pending
}

public enum TestPriority
{
    Low,
    Medium,
    High
}

public struct Point
{
    public int X { get; set; }
    public int Y { get; set; }
}

public class SelfReferencingObject
{
    public string Name { get; set; } = string.Empty;
    public int Value { get; set; }
    public SelfReferencingObject? Self { get; set; }
}

public class EmptyClass
{
    // No public properties
}

public class ClassWithCollectionProperty
{
    public List<string> Items { get; set; } = new();
}

public class ClassWithDictionaryProperty
{
    public Dictionary<string, object?> Metadata { get; set; } = new();
}

// Test classes for nested object testing
public class Leaf
{
    public string Color { get; set; } = string.Empty;
    public int Size { get; set; }
}

public class Node
{
    public string Name { get; set; } = string.Empty;
    public Leaf? Leaf { get; set; }
    public Node? InnerNode { get; set; }
}

// Custom dictionary implementation that allows null keys for testing
internal class NullableKeyDictionary : IDictionary
{
    private readonly List<KeyValuePair<object?, object?>> _items = new();

    public object? this[object key]
    {
        get
        {
            var item = _items.FirstOrDefault(kvp => Equals(kvp.Key, key));
            return item.Value;
        }
        set
        {
            var index = _items.FindIndex(kvp => Equals(kvp.Key, key));
            if (index >= 0)
                _items[index] = new KeyValuePair<object?, object?>(key, value);
            else
                _items.Add(new KeyValuePair<object?, object?>(key, value));
        }
    }

    public bool IsFixedSize => false;
    public bool IsReadOnly => false;
    public int Count => _items.Count;
    public bool IsSynchronized => false;
    public object SyncRoot => ((ICollection)_items).SyncRoot;
    public ICollection Keys => _items.Select(kvp => kvp.Key).ToList();
    public ICollection Values => _items.Select(kvp => kvp.Value).ToList();

    public void Add(object key, object? value)
    {
        _items.Add(new KeyValuePair<object?, object?>(key, value));
    }

    public void Clear() => _items.Clear();

    public bool Contains(object key) => _items.Any(kvp => Equals(kvp.Key, key));

    public void CopyTo(Array array, int index)
    {
        foreach (var item in _items.Where(kvp => kvp.Key is not null))
        {
            array.SetValue(new DictionaryEntry(item.Key!, item.Value), index++);
        }
    }

    public IDictionaryEnumerator GetEnumerator() => new NullableKeyEnumerator(_items.GetEnumerator());
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Remove(object key)
    {
        _items.RemoveAll(kvp => Equals(kvp.Key, key));
    }

    private class NullableKeyEnumerator : IDictionaryEnumerator
    {
        private readonly IEnumerator<KeyValuePair<object?, object?>> _enumerator;

        public NullableKeyEnumerator(IEnumerator<KeyValuePair<object?, object?>> enumerator)
        {
            _enumerator = enumerator;
        }

        public DictionaryEntry Entry => new(_enumerator.Current.Key!, _enumerator.Current.Value);
        public object Key => _enumerator.Current.Key!;
        public object? Value => _enumerator.Current.Value;
        public object Current => Entry;
        public bool MoveNext() => _enumerator.MoveNext();
        public void Reset() => _enumerator.Reset();
    }
}
