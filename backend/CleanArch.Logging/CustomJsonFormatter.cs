using System.Reflection;
using Serilog.Events;
using Serilog.Formatting;
using System.Text.Json;

namespace CleanArch.Logging;

public class CustomJsonFormatter : ITextFormatter
{
    public void Format(LogEvent logEvent, TextWriter output)
    {
        if (output == null)
            return;

        try
        {
            // Get the plain message - if MessageTemplate equals the rendered message, use it directly
            // Otherwise use the rendered message (which should be the same since we're not using templates)
            var message = logEvent.MessageTemplate.Text;
            if (string.IsNullOrEmpty(message))
            {
                message = logEvent.RenderMessage();
            }
            
            var logObject = new Dictionary<string, object?>
            {
                ["timestamp"] = logEvent.Timestamp.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ"),
                ["level"] = logEvent.Level.ToString(),
                ["message"] = message
            };

            // Collect user properties (exclude framework properties that go at root level)
            var frameworkProperties = new HashSet<string> { "TraceId", "SpanId", "RequestId", "ConnectionId", "RequestPath", "ActionId", "ActionName", "SourceContext", "EnvironmentName", "MachineName", "ThreadId" };
            var properties = new Dictionary<string, object?>();
            
            // Add structured properties from the log event (excluding framework properties and Scope)
            foreach (var property in logEvent.Properties)
            {
                if (!frameworkProperties.Contains(property.Key))
                {
                    var camelKey = ToCamelCase(property.Key);
                    properties[camelKey] = FormatPropertyValue(property.Value);
                }
            }

            // Add properties object if there are any user properties
            if (properties.Count > 0)
            {
                // Recursively sanitize the entire properties dictionary to ensure all values are serializable
                logObject["properties"] = SanitizeDictionary(properties);
            }

            // Add framework properties at root level with camelCase names
            if (logEvent.Properties.TryGetValue("TraceId", out var traceId))
            {
                logObject["traceId"] = FormatPropertyValue(traceId);
            }
            if (logEvent.Properties.TryGetValue("SpanId", out var spanId))
            {
                logObject["spanId"] = FormatPropertyValue(spanId);
            }
            if (logEvent.Properties.TryGetValue("RequestId", out var requestId))
            {
                logObject["requestId"] = FormatPropertyValue(requestId);
            }
            if (logEvent.Properties.TryGetValue("ConnectionId", out var connectionId))
            {
                logObject["connectionId"] = FormatPropertyValue(connectionId);
            }
            if (logEvent.Properties.TryGetValue("RequestPath", out var requestPath))
            {
                logObject["requestPath"] = FormatPropertyValue(requestPath);
            }
            if (logEvent.Properties.TryGetValue("ActionId", out var actionId))
            {
                logObject["actionId"] = FormatPropertyValue(actionId);
            }
            if (logEvent.Properties.TryGetValue("ActionName", out var actionName))
            {
                logObject["actionName"] = FormatPropertyValue(actionName);
            }
            if (logEvent.Properties.TryGetValue("SourceContext", out var sourceContext))
            {
                logObject["sourceContext"] = FormatPropertyValue(sourceContext);
            }
            if (logEvent.Properties.TryGetValue("EnvironmentName", out var envName))
            {
                logObject["environmentName"] = FormatPropertyValue(envName);
            }
            if (logEvent.Properties.TryGetValue("MachineName", out var machineName))
            {
                logObject["machineName"] = FormatPropertyValue(machineName);
            }
            if (logEvent.Properties.TryGetValue("ThreadId", out var threadId))
            {
                logObject["threadId"] = FormatPropertyValue(threadId);
            }

            var json = JsonSerializer.Serialize(logObject, new JsonSerializerOptions
            {
                WriteIndented = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            
            output.WriteLine(json);
        }
        catch
        {
            // Swallow exceptions from output writer to prevent propagation
        }
    }

    private static object? FormatPropertyValue(LogEventPropertyValue value)
    {
        return value switch
        {
            ScalarValue scalar => SanitizeValue(scalar.Value),
            SequenceValue sequence => sequence.Elements.Select(FormatPropertyValue).ToArray(),
            StructureValue structure => structure.Properties.ToDictionary(
                p => ToCamelCase(p.Name),
                p => FormatPropertyValue(p.Value)
            ),
            DictionaryValue dictionary => dictionary.Elements.ToDictionary(
                kvp => ToCamelCase(FormatPropertyValue(kvp.Key)?.ToString() ?? "null"),
                kvp => FormatPropertyValue(kvp.Value)
            ),
            _ => SanitizeValue(value.ToString())
        };
    }

    /// <summary>
    /// Sanitizes a value to ensure it can be serialized by System.Text.Json.
    /// Converts unsupported types (like reflection types) to strings.
    /// </summary>
    private static object? SanitizeValue(object? value)
    {
        if (value == null)
            return null;

        var type = value.GetType();

        // Check if it's a reflection type or other unsupported type
        if (type.Namespace?.StartsWith("System.Reflection") == true ||
            type == typeof(MemberInfo) ||
            type.IsSubclassOf(typeof(MemberInfo)) ||
            type == typeof(Type) ||
            type == typeof(Assembly))
        {
            // Convert reflection types to string representation
            return value.ToString();
        }

        // Check if it's a delegate or function
        if (value is Delegate)
        {
            return value.ToString();
        }

        // For other types, return as-is (System.Text.Json will handle serializable types)
        return value;
    }

    /// <summary>
    /// Recursively sanitizes a dictionary and all nested dictionaries/collections to ensure all values are serializable.
    /// </summary>
    private static Dictionary<string, object?> SanitizeDictionary(Dictionary<string, object?> dict)
    {
        var result = new Dictionary<string, object?>();
        
        foreach (var kvp in dict)
        {
            result[kvp.Key] = SanitizeObject(kvp.Value);
        }
        
        return result;
    }

    /// <summary>
    /// Recursively sanitizes an object, handling dictionaries, arrays, and nested structures.
    /// </summary>
    private static object? SanitizeObject(object? value)
    {
        if (value == null)
            return null;

        // Handle dictionaries
        if (value is Dictionary<string, object?> dict)
        {
            return SanitizeDictionary(dict);
        }

        // Handle arrays and lists
        if (value is System.Collections.IEnumerable enumerable && !(value is string))
        {
            var list = new List<object?>();
            foreach (var item in enumerable)
            {
                list.Add(SanitizeObject(item));
            }
            return list.ToArray();
        }

        // Sanitize the value itself
        return SanitizeValue(value);
    }

    private static string ToCamelCase(string str)
    {
        if (string.IsNullOrEmpty(str) || char.IsLower(str[0]))
            return str;

        var chars = str.ToCharArray();
        chars[0] = char.ToLowerInvariant(chars[0]);
        return new string(chars);
    }


}

