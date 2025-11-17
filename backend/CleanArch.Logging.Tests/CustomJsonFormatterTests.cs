using System.Text;
using FluentAssertions;
using NUnit.Framework;
using Serilog.Events;
using Serilog.Parsing;
using CleanArch.Logging;
using CleanArch.UnitTests;

namespace CleanArch.Logging.Tests;

[TestFixture]
public class CustomJsonFormatterTests : UnitTestBase<CustomJsonFormatter>
{
    private StringWriter _output = null!;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        _output = new StringWriter();
    }

    [TearDown]
    public override void TearDown()
    {
        _output?.Dispose();
        base.TearDown();
    }

    #region Basic Functionality Tests

    [Test]
    public void Format_WithStandardLogEvent_ShouldNotThrow()
    {
        // Arrange
        var logEvent = CreateLogEvent("Test message", LogEventLevel.Information);

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            SystemUnderTest.Format(logEvent, _output);
        });
    }

    [Test]
    public void Format_WithStandardLogEvent_ShouldProduceValidJson()
    {
        // Arrange
        var logEvent = CreateLogEvent("Test message", LogEventLevel.Information);

        // Act
        SystemUnderTest.Format(logEvent, _output);
        var json = _output.ToString();

        // Assert
        json.Should().NotBeNullOrEmpty();
        json.Should().Contain("timestamp");
        json.Should().Contain("level");
        json.Should().Contain("message");
        json.Should().Contain("Test message");
    }

    [Test]
    public void Format_WithDifferentLogLevels_ShouldNotThrow()
    {
        // Act & Assert
        foreach (var level in new[] { LogEventLevel.Debug, LogEventLevel.Information, LogEventLevel.Warning, LogEventLevel.Error, LogEventLevel.Fatal })
        {
            var logEvent = CreateLogEvent("Test message", level);
            Assert.DoesNotThrow(() =>
            {
                SystemUnderTest.Format(logEvent, _output);
            });
            _output.GetStringBuilder().Clear();
        }
    }

    [Test]
    public void Format_WithProperties_ShouldIncludeProperties()
    {
        // Arrange
        var properties = new Dictionary<string, LogEventPropertyValue>
        {
            { "UserId", new ScalarValue(123) },
            { "UserName", new ScalarValue("TestUser") }
        };
        var logEvent = CreateLogEvent("Test message", LogEventLevel.Information, properties);

        // Act
        SystemUnderTest.Format(logEvent, _output);
        var json = _output.ToString();

        // Assert
        json.Should().Contain("properties");
        json.Should().Contain("userId");
        json.Should().Contain("userName");
    }

    [Test]
    public void Format_ShouldConvertToCamelCase()
    {
        // Arrange
        var properties = new Dictionary<string, LogEventPropertyValue>
        {
            { "UserId", new ScalarValue(123) },
            { "User_Name", new ScalarValue("Test") }
        };
        var logEvent = CreateLogEvent("Test message", LogEventLevel.Information, properties);

        // Act
        SystemUnderTest.Format(logEvent, _output);
        var json = _output.ToString();

        // Assert
        json.Should().Contain("userId");
        json.Should().Contain("user_Name");
    }

    #endregion

    #region Edge Cases

    [Test]
    public void Format_WithNullLogEvent_ShouldNotThrow()
    {
        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            SystemUnderTest.Format(null!, _output);
        });
    }

    [Test]
    public void Format_WithNullOutput_ShouldNotThrow()
    {
        // Arrange
        var logEvent = CreateLogEvent("Test message", LogEventLevel.Information);

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            SystemUnderTest.Format(logEvent, null!);
        });
    }

    [Test]
    public void Format_WithEmptyMessageTemplate_ShouldNotThrow()
    {
        // Arrange
        var logEvent = CreateLogEvent("", LogEventLevel.Information);

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            SystemUnderTest.Format(logEvent, _output);
        });
    }

    [Test]
    public void Format_WithNullMessageTemplate_ShouldNotThrow()
    {
        // Arrange
        // Note: Serilog's MessageTemplateParser doesn't allow null templates,
        // so we test with an empty string template instead, which is the closest equivalent
        var messageTemplate = new MessageTemplateParser().Parse("");
        var logEvent = new LogEvent(
            DateTimeOffset.Now,
            LogEventLevel.Information,
            null,
            messageTemplate,
            Array.Empty<LogEventProperty>());

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            SystemUnderTest.Format(logEvent, _output);
        });
    }

    [Test]
    public void Format_WithNoProperties_ShouldNotThrow()
    {
        // Arrange
        var logEvent = CreateLogEvent("Test message", LogEventLevel.Information);

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            SystemUnderTest.Format(logEvent, _output);
        });

        var json = _output.ToString();
        json.Should().NotContain("properties");
    }

    [Test]
    public void Format_WithFrameworkProperties_ShouldIncludeAtRootLevel()
    {
        // Arrange
        var properties = new Dictionary<string, LogEventPropertyValue>
        {
            { "TraceId", new ScalarValue("trace-123") },
            { "SpanId", new ScalarValue("span-456") },
            { "RequestId", new ScalarValue("req-789") },
            { "SourceContext", new ScalarValue("TestClass") }
        };
        var logEvent = CreateLogEvent("Test message", LogEventLevel.Information, properties);

        // Act
        SystemUnderTest.Format(logEvent, _output);
        var json = _output.ToString();

        // Assert
        json.Should().Contain("traceId");
        json.Should().Contain("spanId");
        json.Should().Contain("requestId");
        json.Should().Contain("sourceContext");
        json.Should().NotContain("\"properties\":{\"traceId\"");
    }

    [Test]
    public void Format_WithMixedFrameworkAndUserProperties_ShouldSeparateCorrectly()
    {
        // Arrange
        var properties = new Dictionary<string, LogEventPropertyValue>
        {
            { "TraceId", new ScalarValue("trace-123") },
            { "UserId", new ScalarValue(123) },
            { "UserName", new ScalarValue("TestUser") }
        };
        var logEvent = CreateLogEvent("Test message", LogEventLevel.Information, properties);

        // Act
        SystemUnderTest.Format(logEvent, _output);
        var json = _output.ToString();

        // Assert
        json.Should().Contain("traceId");
        json.Should().Contain("properties");
        json.Should().Contain("userId");
        json.Should().Contain("userName");
    }

    #endregion

    #region Property Value Formatting

    [Test]
    public void Format_WithScalarStringValue_ShouldNotThrow()
    {
        // Arrange
        var properties = new Dictionary<string, LogEventPropertyValue>
        {
            { "StringValue", new ScalarValue("Test String") }
        };
        var logEvent = CreateLogEvent("Test message", LogEventLevel.Information, properties);

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            SystemUnderTest.Format(logEvent, _output);
        });
    }

    [Test]
    public void Format_WithScalarIntValue_ShouldNotThrow()
    {
        // Arrange
        var properties = new Dictionary<string, LogEventPropertyValue>
        {
            { "IntValue", new ScalarValue(123) }
        };
        var logEvent = CreateLogEvent("Test message", LogEventLevel.Information, properties);

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            SystemUnderTest.Format(logEvent, _output);
        });
    }

    [Test]
    public void Format_WithScalarBoolValue_ShouldNotThrow()
    {
        // Arrange
        var properties = new Dictionary<string, LogEventPropertyValue>
        {
            { "BoolValue", new ScalarValue(true) }
        };
        var logEvent = CreateLogEvent("Test message", LogEventLevel.Information, properties);

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            SystemUnderTest.Format(logEvent, _output);
        });
    }

    [Test]
    public void Format_WithScalarNullValue_ShouldNotThrow()
    {
        // Arrange
        var properties = new Dictionary<string, LogEventPropertyValue>
        {
            { "NullValue", new ScalarValue(null) }
        };
        var logEvent = CreateLogEvent("Test message", LogEventLevel.Information, properties);

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            SystemUnderTest.Format(logEvent, _output);
        });
    }

    [Test]
    public void Format_WithSequenceValue_ShouldNotThrow()
    {
        // Arrange
        var sequence = new SequenceValue(new[]
        {
            new ScalarValue("Item1"),
            new ScalarValue("Item2"),
            new ScalarValue("Item3")
        });
        var properties = new Dictionary<string, LogEventPropertyValue>
        {
            { "Items", sequence }
        };
        var logEvent = CreateLogEvent("Test message", LogEventLevel.Information, properties);

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            SystemUnderTest.Format(logEvent, _output);
        });
    }

    [Test]
    public void Format_WithStructureValue_ShouldNotThrow()
    {
        // Arrange
        var structure = new StructureValue(new[]
        {
            new LogEventProperty("Name", new ScalarValue("Test")),
            new LogEventProperty("Value", new ScalarValue(123))
        });
        var properties = new Dictionary<string, LogEventPropertyValue>
        {
            { "Object", structure }
        };
        var logEvent = CreateLogEvent("Test message", LogEventLevel.Information, properties);

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            SystemUnderTest.Format(logEvent, _output);
        });
    }

    [Test]
    public void Format_WithDictionaryValue_ShouldNotThrow()
    {
        // Arrange
        var dictionary = new DictionaryValue(new[]
        {
            new KeyValuePair<ScalarValue, LogEventPropertyValue>(
                new ScalarValue("Key1"),
                new ScalarValue("Value1")),
            new KeyValuePair<ScalarValue, LogEventPropertyValue>(
                new ScalarValue("Key2"),
                new ScalarValue("Value2"))
        });
        var properties = new Dictionary<string, LogEventPropertyValue>
        {
            { "Dictionary", dictionary }
        };
        var logEvent = CreateLogEvent("Test message", LogEventLevel.Information, properties);

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            SystemUnderTest.Format(logEvent, _output);
        });
    }

    [Test]
    public void Format_WithNestedStructures_ShouldNotThrow()
    {
        // Arrange
        var innerStructure = new StructureValue(new[]
        {
            new LogEventProperty("InnerName", new ScalarValue("Inner"))
        });
        var outerStructure = new StructureValue(new[]
        {
            new LogEventProperty("OuterName", new ScalarValue("Outer")),
            new LogEventProperty("Inner", innerStructure)
        });
        var properties = new Dictionary<string, LogEventPropertyValue>
        {
            { "Nested", outerStructure }
        };
        var logEvent = CreateLogEvent("Test message", LogEventLevel.Information, properties);

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            SystemUnderTest.Format(logEvent, _output);
        });
    }

    [Test]
    public void Format_WithSpecialCharactersInValues_ShouldNotThrow()
    {
        // Arrange
        var properties = new Dictionary<string, LogEventPropertyValue>
        {
            { "JsonValue", new ScalarValue("{\"key\":\"value\"}") },
            { "XmlValue", new ScalarValue("<tag>value</tag>") },
            { "NewLineValue", new ScalarValue("Line1\nLine2") },
            { "UnicodeValue", new ScalarValue("Test\u00A9\u00AE") }
        };
        var logEvent = CreateLogEvent("Test message", LogEventLevel.Information, properties);

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            SystemUnderTest.Format(logEvent, _output);
        });
    }

    [Test]
    public void Format_WithVeryLargePropertyValue_ShouldNotThrow()
    {
        // Arrange
        var largeString = new string('A', 100000);
        var properties = new Dictionary<string, LogEventPropertyValue>
        {
            { "LargeValue", new ScalarValue(largeString) }
        };
        var logEvent = CreateLogEvent("Test message", LogEventLevel.Information, properties);

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            SystemUnderTest.Format(logEvent, _output);
        });
    }

    [Test]
    public void Format_WithAllFrameworkProperties_ShouldIncludeAll()
    {
        // Arrange
        var properties = new Dictionary<string, LogEventPropertyValue>
        {
            { "TraceId", new ScalarValue("trace-123") },
            { "SpanId", new ScalarValue("span-456") },
            { "RequestId", new ScalarValue("req-789") },
            { "ConnectionId", new ScalarValue("conn-101") },
            { "RequestPath", new ScalarValue("/api/test") },
            { "ActionId", new ScalarValue("action-202") },
            { "ActionName", new ScalarValue("TestAction") },
            { "SourceContext", new ScalarValue("TestClass") },
            { "EnvironmentName", new ScalarValue("Development") },
            { "MachineName", new ScalarValue("TestMachine") },
            { "ThreadId", new ScalarValue(12345) }
        };
        var logEvent = CreateLogEvent("Test message", LogEventLevel.Information, properties);

        // Act
        SystemUnderTest.Format(logEvent, _output);
        var json = _output.ToString();

        // Assert
        json.Should().Contain("traceId");
        json.Should().Contain("spanId");
        json.Should().Contain("requestId");
        json.Should().Contain("connectionId");
        json.Should().Contain("requestPath");
        json.Should().Contain("actionId");
        json.Should().Contain("actionName");
        json.Should().Contain("sourceContext");
        json.Should().Contain("environmentName");
        json.Should().Contain("machineName");
        json.Should().Contain("threadId");
    }

    #endregion

    #region Exception Safety

    [Test]
    public void Format_WithThrowingOutputWriter_ShouldNotPropagateException()
    {
        // Arrange
        var throwingWriter = new ThrowingTextWriter();
        var logEvent = CreateLogEvent("Test message", LogEventLevel.Information);

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            SystemUnderTest.Format(logEvent, throwingWriter);
        });
    }

    [Test]
    public void Format_WithInvalidPropertyValue_ShouldNotThrow()
    {
        // Arrange
        // Create a property value that might cause issues during formatting
        var properties = new Dictionary<string, LogEventPropertyValue>
        {
            { "InvalidValue", new ScalarValue(new object()) } // Non-serializable object
        };
        var logEvent = CreateLogEvent("Test message", LogEventLevel.Information, properties);

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            SystemUnderTest.Format(logEvent, _output);
        });
    }

    [Test]
    public void Format_WithComplexNestedStructures_ShouldNotThrow()
    {
        // Arrange
        var deepNested = new StructureValue(new[]
        {
            new LogEventProperty("Level1", new StructureValue(new[]
            {
                new LogEventProperty("Level2", new StructureValue(new[]
                {
                    new LogEventProperty("Level3", new ScalarValue("Deep"))
                }))
            }))
        });
        var properties = new Dictionary<string, LogEventPropertyValue>
        {
            { "DeepNested", deepNested }
        };
        var logEvent = CreateLogEvent("Test message", LogEventLevel.Information, properties);

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            SystemUnderTest.Format(logEvent, _output);
        });
    }

    [Test]
    public void Format_WithEmptySequence_ShouldNotThrow()
    {
        // Arrange
        var emptySequence = new SequenceValue(Array.Empty<LogEventPropertyValue>());
        var properties = new Dictionary<string, LogEventPropertyValue>
        {
            { "EmptyArray", emptySequence }
        };
        var logEvent = CreateLogEvent("Test message", LogEventLevel.Information, properties);

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            SystemUnderTest.Format(logEvent, _output);
        });
    }

    [Test]
    public void Format_WithEmptyDictionary_ShouldNotThrow()
    {
        // Arrange
        var emptyDictionary = new DictionaryValue(Array.Empty<KeyValuePair<ScalarValue, LogEventPropertyValue>>());
        var properties = new Dictionary<string, LogEventPropertyValue>
        {
            { "EmptyDict", emptyDictionary }
        };
        var logEvent = CreateLogEvent("Test message", LogEventLevel.Information, properties);

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            SystemUnderTest.Format(logEvent, _output);
        });
    }

    [Test]
    public void Format_WithNullInSequence_ShouldNotThrow()
    {
        // Arrange
        var sequence = new SequenceValue(new[]
        {
            new ScalarValue("Item1"),
            new ScalarValue(null),
            new ScalarValue("Item3")
        });
        var properties = new Dictionary<string, LogEventPropertyValue>
        {
            { "Items", sequence }
        };
        var logEvent = CreateLogEvent("Test message", LogEventLevel.Information, properties);

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            SystemUnderTest.Format(logEvent, _output);
        });
    }

    [Test]
    public void Format_WithDictionaryValueWithNullKey_ShouldHandleNullKey()
    {
        // Arrange
        // Create a dictionary value with a null key to test the null coalescing branch
        var dictionary = new DictionaryValue(new[]
        {
            new KeyValuePair<ScalarValue, LogEventPropertyValue>(
                new ScalarValue(null), // Null key
                new ScalarValue("Value1"))
        });
        var properties = new Dictionary<string, LogEventPropertyValue>
        {
            { "Dictionary", dictionary }
        };
        var logEvent = CreateLogEvent("Test message", LogEventLevel.Information, properties);

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            SystemUnderTest.Format(logEvent, _output);
        });
        var json = _output.ToString();
        json.Should().Contain("null"); // Should convert null key to "null" string
    }

    [Test]
    public void Format_WithUnknownPropertyValueType_ShouldUseToString()
    {
        // Arrange
        // Create a custom property value type that doesn't match any known type
        // This will hit the default case in the switch expression (line 125)
        var customValue = new CustomPropertyValue("CustomValue");
        var properties = new Dictionary<string, LogEventPropertyValue>
        {
            { "Custom", customValue }
        };
        var logEvent = CreateLogEvent("Test message", LogEventLevel.Information, properties);

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            SystemUnderTest.Format(logEvent, _output);
        });
        var json = _output.ToString();
        json.Should().Contain("CustomValue"); // Should use ToString() result
    }

    [Test]
    public void Format_WithLowercaseFirstCharPropertyName_ShouldNotConvert()
    {
        // Arrange
        // Test ToCamelCase with string that already starts with lowercase (line 132 - return str branch)
        var properties = new Dictionary<string, LogEventPropertyValue>
        {
            { "alreadyCamelCase", new ScalarValue("Value") }
        };
        var logEvent = CreateLogEvent("Test message", LogEventLevel.Information, properties);

        // Act
        SystemUnderTest.Format(logEvent, _output);
        var json = _output.ToString();

        // Assert
        json.Should().Contain("alreadyCamelCase"); // Should remain unchanged
    }

    [Test]
    public void Format_WithDictionaryValueWithEmptyStringKey_ShouldHandleEmptyKey()
    {
        // Arrange
        // Test ToCamelCase with empty string from dictionary key formatting (line 132 - return str branch for empty string)
        // FormatPropertyValue on ScalarValue("") returns "", and ToString() on "" returns "", which hits the empty string branch
        var dictionary = new DictionaryValue(new[]
        {
            new KeyValuePair<ScalarValue, LogEventPropertyValue>(
                new ScalarValue(""), // Empty string key
                new ScalarValue("Value1"))
        });
        var properties = new Dictionary<string, LogEventPropertyValue>
        {
            { "Dictionary", dictionary }
        };
        var logEvent = CreateLogEvent("Test message", LogEventLevel.Information, properties);

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            SystemUnderTest.Format(logEvent, _output);
        });
    }

    #endregion

    #region Helper Methods

    private static LogEvent CreateLogEvent(
        string message,
        LogEventLevel level,
        Dictionary<string, LogEventPropertyValue>? properties = null)
    {
        var messageTemplate = new MessageTemplateParser().Parse(message);
        var logEventProperties = properties?.Select(kvp => new LogEventProperty(kvp.Key, kvp.Value))
            .ToList() ?? new List<LogEventProperty>();

        return new LogEvent(
            DateTimeOffset.UtcNow,
            level,
            null,
            messageTemplate,
            logEventProperties);
    }

    private class ThrowingTextWriter : TextWriter
    {
        public override Encoding Encoding => Encoding.UTF8;

        public override void Write(char value)
        {
            throw new InvalidOperationException("Writer throws");
        }

        public override void Write(string? value)
        {
            throw new InvalidOperationException("Writer throws");
        }

        public override void WriteLine(string? value)
        {
            throw new InvalidOperationException("Writer throws");
        }
    }

    private class CustomPropertyValue : LogEventPropertyValue
    {
        private readonly string _value;

        public CustomPropertyValue(string value)
        {
            _value = value;
        }

        public override void Render(TextWriter output, string? format = null, IFormatProvider? formatProvider = null)
        {
            output.Write(_value);
        }

        public override string ToString()
        {
            return _value;
        }
    }

    #endregion

    [Test]
    public void Format_WithReflectionTypes_ShouldConvertToString()
    {
        // Arrange
        var (logger, output) = TestLoggerHelper.CreateCapturingLogger();
        var methodInfo = typeof(string).GetMethod("ToString", Array.Empty<Type>())!;
        var propertyInfo = typeof(string).GetProperty("Length")!;
        var type = typeof(string);
        var assembly = typeof(string).Assembly;

        // Act
        logger.Debug("Test message", new
        {
            Method = methodInfo,
            Property = propertyInfo,
            Type = type,
            Assembly = assembly
        });

        // Assert
        var logs = TestLoggerHelper.ParseJsonLogs(output.ToString());
        logs.Should().HaveCount(1);
        var log = logs[0];
        log.Should().ContainKey("properties");
        var props = log["properties"] as System.Text.Json.JsonElement?;
        props?.GetProperty("method").GetString().Should().NotBeNullOrEmpty();
        props?.GetProperty("property").GetString().Should().NotBeNullOrEmpty();
        props?.GetProperty("type").GetString().Should().NotBeNullOrEmpty();
        props?.GetProperty("assembly").GetString().Should().NotBeNullOrEmpty();
    }

    [Test]
    public void Format_WithDelegate_ShouldConvertToString()
    {
        // Arrange
        var (logger, output) = TestLoggerHelper.CreateCapturingLogger();
        Action action = () => Console.WriteLine("test");
        Func<int, int> func = x => x * 2;

        // Act
        logger.Debug("Test message", new
        {
            Action = action,
            Func = func
        });

        // Assert
        var logs = TestLoggerHelper.ParseJsonLogs(output.ToString());
        logs.Should().HaveCount(1);
        var log = logs[0];
        log.Should().ContainKey("properties");
        var props = log["properties"] as System.Text.Json.JsonElement?;
        props?.GetProperty("action").GetString().Should().NotBeNullOrEmpty();
        props?.GetProperty("func").GetString().Should().NotBeNullOrEmpty();
    }

    [Test]
    public void Format_WithReflectionTypesAsScalarValue_ShouldConvertToString()
    {
        // Arrange
        // Create LogEvent directly with reflection types as ScalarValue to test SanitizeValue
        var methodInfo = typeof(string).GetMethod("ToString", Array.Empty<Type>())!;
        var propertyInfo = typeof(string).GetProperty("Length")!;
        var type = typeof(string);
        var assembly = typeof(string).Assembly;
        Action action = () => Console.WriteLine("test");
        Func<int, int> func = x => x * 2;

        var properties = new Dictionary<string, LogEventPropertyValue>
        {
            { "Method", new ScalarValue(methodInfo) },
            { "Property", new ScalarValue(propertyInfo) },
            { "Type", new ScalarValue(type) },
            { "Assembly", new ScalarValue(assembly) },
            { "Action", new ScalarValue(action) },
            { "Func", new ScalarValue(func) }
        };
        var logEvent = CreateLogEvent("Test message", LogEventLevel.Information, properties);

        // Act
        SystemUnderTest.Format(logEvent, _output);
        var json = _output.ToString();

        // Assert
        json.Should().Contain("properties");
        json.Should().Contain("method");
        json.Should().Contain("property");
        json.Should().Contain("type");
        json.Should().Contain("assembly");
        json.Should().Contain("action");
        json.Should().Contain("func");
        // All should be strings (not objects)
        var logs = TestLoggerHelper.ParseJsonLogs(json);
        logs.Should().HaveCount(1);
        var log = logs[0];
        log.Should().ContainKey("properties");
        var props = log["properties"] as System.Text.Json.JsonElement?;
        props?.GetProperty("method").ValueKind.Should().Be(System.Text.Json.JsonValueKind.String);
        props?.GetProperty("property").ValueKind.Should().Be(System.Text.Json.JsonValueKind.String);
        props?.GetProperty("type").ValueKind.Should().Be(System.Text.Json.JsonValueKind.String);
        props?.GetProperty("assembly").ValueKind.Should().Be(System.Text.Json.JsonValueKind.String);
        props?.GetProperty("action").ValueKind.Should().Be(System.Text.Json.JsonValueKind.String);
        props?.GetProperty("func").ValueKind.Should().Be(System.Text.Json.JsonValueKind.String);
    }
}

