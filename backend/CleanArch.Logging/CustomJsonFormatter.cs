using Serilog.Events;
using Serilog.Formatting;
using System.Text.Json;

namespace CleanArch.Logging;

public class CustomJsonFormatter : ITextFormatter
{
    public void Format(LogEvent logEvent, TextWriter output)
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
        
        // Add structured properties from the log event (excluding framework properties)
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
            logObject["properties"] = properties;
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

    private static object? FormatPropertyValue(LogEventPropertyValue value)
    {
        return value switch
        {
            ScalarValue scalar => scalar.Value,
            SequenceValue sequence => sequence.Elements.Select(FormatPropertyValue).ToArray(),
            StructureValue structure => structure.Properties.ToDictionary(
                p => ToCamelCase(p.Name),
                p => FormatPropertyValue(p.Value)
            ),
            DictionaryValue dictionary => dictionary.Elements.ToDictionary(
                kvp => ToCamelCase(FormatPropertyValue(kvp.Key)?.ToString() ?? "null"),
                kvp => FormatPropertyValue(kvp.Value)
            ),
            _ => value.ToString()
        };
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

