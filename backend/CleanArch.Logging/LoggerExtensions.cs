using System.Collections;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace CleanArch.Logging;

public static class LoggerExtensions
{
    public static void Debug(this ILogger logger, string message, object? properties = null)
    {
        LogWithProperties(logger, LogLevel.Debug, message, properties);
    }

    public static void Info(this ILogger logger, string message, object? properties = null)
    {
        LogWithProperties(logger, LogLevel.Information, message, properties);
    }

    public static void Warn(this ILogger logger, string message, object? properties = null)
    {
        LogWithProperties(logger, LogLevel.Warning, message, properties);
    }

    public static void Error(this ILogger logger, string message, object? properties = null)
    {
        LogWithProperties(logger, LogLevel.Error, message, properties);
    }

    public static IDisposable? AddContext(this ILogger logger, object context)
    {
        if (context == null)
            return null;

        var properties = ConvertToDictionary(context);
        return logger.BeginScope(properties);
    }

    private static void LogWithProperties(ILogger logger, LogLevel logLevel, string message, object? properties)
    {
        if (properties == null)
        {
            logger.Log(logLevel, message);
            return;
        }

        var propertyDict = ConvertToDictionary(properties);
        
        // Use BeginScope to add properties without modifying the message template
        using (logger.BeginScope(propertyDict))
        {
            logger.Log(logLevel, message);
        }
    }

    private static Dictionary<string, object?> ConvertToDictionary(object obj)
    {
        var dict = new Dictionary<string, object?>();

        if (obj == null)
            return dict;

        // Handle dictionaries
        if (obj is IDictionary dictionary)
        {
            foreach (DictionaryEntry entry in dictionary)
            {
                var key = entry.Key?.ToString() ?? "null";
                dict[key] = entry.Value;
            }
            return dict;
        }

        // Handle objects using reflection
        var type = obj.GetType();
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            try
            {
                var value = property.GetValue(obj);
                dict[property.Name] = value;
            }
            catch
            {
                // Skip properties that can't be read
            }
        }

        return dict;
    }
}

