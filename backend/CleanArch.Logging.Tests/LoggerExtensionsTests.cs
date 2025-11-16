using System.Collections;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using CleanArch.Logging;
using CleanArch.UnitTests;

namespace CleanArch.Logging.Tests;

[TestFixture]
public class LoggerExtensionsTests : UnitTestBase<object>
{
    [SetUp]
    public void SetUp()
    {
        MockLogger.Setup(x => x.BeginScope(It.IsAny<object>())).Returns(Mock.Of<IDisposable>());
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
        var properties = new Dictionary<string, object?>
        {
            { "Key1", "Value1" },
            { "Key2", 123 },
            { "Key3", true }
        };

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
        IDictionary properties = new Hashtable
        {
            { "Key1", "Value1" },
            { "Key2", 123 }
        };

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
            { "Key1", "Value1" },
            { "Key2", null },
            { "Key3", "Value3" }
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
        var context = new Dictionary<string, object?>
        {
            { "UserId", 123 },
            { "UserName", "TestUser" }
        };

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
        IDictionary properties = new NullableKeyDictionary
        {
            { null, "Value1" },
            { "Key2", "Value2" }
        };

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
                Address = new
                {
                    Street = "123 Main St",
                    City = "TestCity",
                    ZipCode = "12345"
                }
            },
            Metadata = new Dictionary<string, object?>
            {
                { "Key1", "Value1" },
                { "Key2", 456 }
            }
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
            Numbers = new List<int> { 1, 2, 3, 4, 5 },
            Dictionary = new Dictionary<string, int> { { "Key", 123 } }
        };

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            MockLogger.Object.Error("Test message", properties);
        });
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
        var throwingLogger = new Mock<ILogger>();
        throwingLogger.Setup(x => x.Log(
            It.IsAny<LogLevel>(),
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()))
            .Throws<InvalidOperationException>();

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            throwingLogger.Object.Debug("Test message");
        });
    }

    [Test]
    public void Info_WhenBeginScopeThrows_ShouldNotPropagateException()
    {
        // Arrange
        var throwingLogger = new Mock<ILogger>();
        throwingLogger.Setup(x => x.BeginScope(It.IsAny<object>()))
            .Throws<InvalidOperationException>();

        var properties = new { Name = "Test" };

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            throwingLogger.Object.Info("Test message", properties);
        });
    }

    [Test]
    public void Error_WhenLoggerThrows_ShouldNotPropagateException()
    {
        // Arrange
        var throwingLogger = new Mock<ILogger>();
        throwingLogger.Setup(x => x.Log(
            It.IsAny<LogLevel>(),
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()))
            .Throws<Exception>();

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            throwingLogger.Object.Error("Test message");
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

    #endregion
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
        foreach (var item in _items)
        {
            array.SetValue(new DictionaryEntry(item.Key, item.Value), index++);
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

        public DictionaryEntry Entry => new(_enumerator.Current.Key, _enumerator.Current.Value);
        public object Key => _enumerator.Current.Key!;
        public object? Value => _enumerator.Current.Value;
        public object Current => Entry;
        public bool MoveNext() => _enumerator.MoveNext();
        public void Reset() => _enumerator.Reset();
    }
}

