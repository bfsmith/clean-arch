using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace CleanArch.Logging;

public static class LoggerExtensions
{
    extension(ILogger logger)
    {
        public void Debug(string message, object? properties = null)
        {
            LogWithProperties(logger, LogLevel.Debug, message, properties);
        }

        public void Info(string message, object? properties = null)
        {
            LogWithProperties(logger, LogLevel.Information, message, properties);
        }

        public void Warn(string message, object? properties = null)
        {
            LogWithProperties(logger, LogLevel.Warning, message, properties);
        }

        public void Error(string message, object? properties = null)
        {
            LogWithProperties(logger, LogLevel.Error, message, properties);
        }

        public IDisposable? AddContext(object context)
        {
            if (logger == null || context == null)
                return null;

            try
            {
                var properties = ConvertToDictionary(context);
                return logger.BeginScope(properties);
            }
            catch
            {
                // Swallow exceptions to prevent propagation
                return null;
            }
        }
    }

    private static void LogWithProperties(ILogger logger, LogLevel logLevel, string message, object? properties)
    {
        try
        {
            if (properties == null)
            {
                logger.Log(logLevel, message);
                return;
            }

            var propertyDict = ConvertToDictionary(properties);
            
            // Use BeginScope to add properties without modifying the message template
            try
            {
                using (logger.BeginScope(propertyDict))
                {
                    logger.Log(logLevel, message);
                }
            }
            catch
            {
                // If BeginScope throws, fall back to logging without scope
                logger.Log(logLevel, message);
            }
        }
        catch
        {
            // Swallow exceptions from logger to prevent propagation
        }
    }

    private static Dictionary<string, object?> ConvertToDictionary(object obj)
    {
        return ConvertToDictionary(obj, new HashSet<object>(ReferenceEqualityComparer.Instance));
    }

    private static Dictionary<string, object?> ConvertToDictionary(object obj, HashSet<object> visited)
    {
        var dict = new Dictionary<string, object?>();

        // Prevent circular references
        if (visited.Contains(obj))
        {
            return dict;
        }
        visited.Add(obj);

        try
        {
            // Handle dictionaries
            if (obj is IDictionary dictionary)
            {
                foreach (DictionaryEntry entry in dictionary)
                {
                    // Handle null keys gracefully
                    var key = entry.Key?.ToString() ?? "null";
                    dict[key] = ConvertValue(entry.Value, visited);
                }
                return dict;
            }

            // Handle objects using reflection
            var type = obj.GetType();
            
            // Skip primitive types, strings, and other simple types
            if (IsSimpleType(type))
            {
                return dict;
            }

            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                try
                {
                    var value = property.GetValue(obj);
                    dict[property.Name] = ConvertValue(value, visited);
                }
                catch
                {
                    // Skip properties that can't be read (e.g., throw exceptions)
                }
            }
        }
        finally
        {
            visited.Remove(obj);
        }

        return dict;
    }

    private static object? ConvertValue(object? value, HashSet<object> visited)
    {
        if (value == null)
        {
            return null;
        }

        var type = value.GetType();

        // Filter out reflection types and other unsupported types
        if (IsUnsupportedType(type))
        {
            return value.ToString();
        }

        // Return simple types as-is
        if (IsSimpleType(type))
        {
            return value;
        }

        // Return enums as-is (they'll be converted to strings in SanitizeValue)
        if (type.IsEnum)
        {
            return value;
        }

        // Handle dictionaries before collections (since dictionaries are also IEnumerable)
        if (value is IDictionary dictionary)
        {
            var dict = new Dictionary<string, object?>();
            foreach (DictionaryEntry entry in dictionary)
            {
                var key = entry.Key?.ToString() ?? "null";
                dict[key] = ConvertValue(entry.Value, visited);
            }
            return dict;
        }

        // Handle collections (but not dictionaries, which we handled above)
        if (value is System.Collections.IEnumerable enumerable && !(value is string))
        {
            var list = new List<object?>();
            foreach (var item in enumerable)
            {
                list.Add(ConvertValue(item, visited));
            }
            return list;
        }

        // Recursively convert complex objects
        return ConvertToDictionary(value, visited);
    }

    private static bool IsSimpleType(Type type)
    {
        return type.IsPrimitive ||
               type == typeof(string) ||
               type == typeof(decimal) ||
               type == typeof(DateTime) ||
               type == typeof(DateTimeOffset) ||
               type == typeof(TimeSpan) ||
               type == typeof(Guid) ||
               type == typeof(Uri) ||
               type == typeof(Version) ||
               (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
    }

    /// <summary>
    /// Checks if a type is unsupported for JSON serialization (e.g., reflection types, delegates).
    /// These will be converted to strings.
    /// </summary>
    private static bool IsUnsupportedType(Type type)
    {
        return type.Namespace?.StartsWith("System.Reflection") == true ||
               type == typeof(MemberInfo) ||
               type.IsSubclassOf(typeof(MemberInfo)) ||
               type == typeof(Type) ||
               type == typeof(Assembly) ||
               typeof(Delegate).IsAssignableFrom(type);
    }

    private class ReferenceEqualityComparer : IEqualityComparer<object>
    {
        public static readonly ReferenceEqualityComparer Instance = new();

        public new bool Equals(object? x, object? y)
        {
            return ReferenceEquals(x, y);
        }

        public int GetHashCode(object obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }
    }

}



