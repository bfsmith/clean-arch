using System.Collections;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Serilog.Context;

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
                // Use Serilog's LogContext to leverage its built-in destructuring
                return PushPropertiesToLogContext(context);
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

            // Use Serilog's LogContext.PushProperty to leverage its built-in destructuring
            // This handles exceptions, circular references, and complex objects automatically
            // The CustomJsonFormatter will flatten the scope into the properties object
            try
            {
                using (PushPropertiesToLogContext(properties))
                {
                    logger.Log(logLevel, message);
                }
            }
            catch
            {
                // If LogContext throws, fall back to logging without scope
                logger.Log(logLevel, message);
            }
        }
        catch
        {
            // Swallow exceptions from logger to prevent propagation
        }
    }

    /// <summary>
    /// Pushes properties to Serilog's LogContext using its built-in destructuring.
    /// This leverages Serilog's powerful destructuring which handles exceptions,
    /// circular references, and complex objects automatically.
    /// </summary>
    private static IDisposable? PushPropertiesToLogContext(object properties)
    {
        if (properties == null)
            return null;

        var disposables = new List<IDisposable>();
        
        try
        {
            // If it's already a dictionary, push each key-value pair
            if (properties is IDictionary dictionary)
            {
                foreach (DictionaryEntry entry in dictionary)
                {
                    var key = entry.Key?.ToString() ?? "null";
                    // Serilog will destructure the value automatically, handling exceptions, circular refs, etc.
                    var disposable = LogContext.PushProperty(key, entry.Value, destructureObjects: true);
                    disposables.Add(disposable);
                }
            }
            else
            {
                // For objects, use reflection to get properties but let Serilog destructure the values
                // This gives us flat property names while leveraging Serilog's destructuring for complex values
                var type = properties.GetType();
                var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                
                foreach (var prop in props)
                {
                    try
                    {
                        var value = prop.GetValue(properties);
                        // Serilog will destructure the value, handling exceptions, circular references, etc.
                        var disposable = LogContext.PushProperty(prop.Name, value, destructureObjects: true);
                        disposables.Add(disposable);
                    }
                    catch
                    {
                        // Skip properties that can't be read (e.g., throw exceptions)
                        // Serilog's destructuring would handle this, but we catch it here to continue with other properties
                    }
                }
            }

            // Return a composite disposable that disposes all
            return disposables.Count > 0 ? new CompositeDisposable(disposables) : null;
        }
        catch
        {
            // Dispose any that were successfully created
            foreach (var disposable in disposables)
            {
                try { disposable.Dispose(); } catch { }
            }
            return null;
        }
    }

    private class CompositeDisposable : IDisposable
    {
        private readonly List<IDisposable> _disposables;

        public CompositeDisposable(List<IDisposable> disposables)
        {
            _disposables = disposables;
        }

        public void Dispose()
        {
            foreach (var disposable in _disposables)
            {
                try
                {
                    disposable.Dispose();
                }
                catch
                {
                    // Swallow exceptions during disposal
                }
            }
        }
    }

}



