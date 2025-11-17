using System.Reflection;
using Serilog.Events;
using Serilog.Formatting;
using System.Text.Json;
using System.Text.RegularExpressions;

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
            
            // Process properties and flatten scopes
            // Properties are processed in the order they appear in logEvent.Properties
            // When scopes are nested, outer scopes appear first, inner scopes appear later
            // By processing in order and overwriting, inner scopes will overwrite outer ones
            foreach (var property in logEvent.Properties)
            {
                if (frameworkProperties.Contains(property.Key))
                    continue;

                if (property.Key == "Scope" && property.Value is SequenceValue sequence)
                {
                    // Scope is a SequenceValue where each element represents a scope level
                    // Outer scopes appear first, inner scopes appear later
                    // Process in order so inner scopes overwrite outer ones
                    foreach (var element in sequence.Elements)
                    {
                        if (element is StructureValue scopeStructure)
                        {
                            // Flatten each scope structure into the properties dictionary
                            var flattened = FlattenStructureValue(scopeStructure);
                            foreach (var kvp in flattened)
                            {
                                // Overwrite if key exists (inner scopes overwrite outer ones)
                                properties[kvp.Key] = kvp.Value;
                            }
                        }
                        else if (element is ScalarValue scalar && scalar.Value is string scopeString)
                        {
                            // When BeginScope is used with an object, Serilog may serialize it as a string
                            // Format: "{ PropertyName = Value, PropertyName2 = Value2 }"
                            // Parse the string representation to extract properties
                            var parsed = ParseScopeString(scopeString);
                            foreach (var kvp in parsed)
                            {
                                // Overwrite if key exists (inner scopes overwrite outer ones)
                                properties[kvp.Key] = kvp.Value;
                            }
                        }
                    }
                }
                else
                {
                    // Regular property from LogContext.PushProperty
                    // When destructureObjects: true is used, complex objects become StructureValue
                    // Simple values become ScalarValue
                    var camelKey = ToCamelCase(property.Key);
                    
                    // If it's a StructureValue (destructured object), convert it to a nested dictionary
                    if (property.Value is StructureValue structureValue)
                    {
                        // Convert StructureValue to nested dictionary structure preserving object hierarchy
                        properties[camelKey] = ConvertStructureValueToDictionary(structureValue);
                    }
                    else
                    {
                        // Simple value - add directly
                        properties[camelKey] = FormatPropertyValue(property.Value);
                    }
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
            _ => value.ToString()
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

    /// <summary>
    /// Parses a scope string representation into a dictionary of properties.
    /// Handles format: "{ PropertyName = Value, PropertyName2 = Value2 }"
    /// </summary>
    private static Dictionary<string, object?> ParseScopeString(string scopeString)
    {
        var result = new Dictionary<string, object?>();
        
        if (string.IsNullOrWhiteSpace(scopeString))
            return result;
        
        // Remove outer braces if present
        var trimmed = scopeString.Trim();
        if (trimmed.StartsWith('{') && trimmed.EndsWith('}'))
        {
            trimmed = trimmed[1..^1].Trim();
        }
        
        if (string.IsNullOrWhiteSpace(trimmed))
            return result;
        
        // Split by comma, but be careful with quoted strings and nested objects
        var properties = new List<string>();
        var current = new System.Text.StringBuilder();
        var depth = 0;
        var inQuotes = false;
        var escapeNext = false;
        
        foreach (var ch in trimmed)
        {
            if (escapeNext)
            {
                current.Append(ch);
                escapeNext = false;
                continue;
            }
            
            if (ch == '\\')
            {
                escapeNext = true;
                current.Append(ch);
                continue;
            }
            
            if (ch == '"' && !escapeNext)
            {
                inQuotes = !inQuotes;
                current.Append(ch);
                continue;
            }
            
            if (inQuotes)
            {
                current.Append(ch);
                continue;
            }
            
            if (ch == '{' || ch == '[')
            {
                depth++;
                current.Append(ch);
                continue;
            }
            
            if (ch == '}' || ch == ']')
            {
                depth--;
                current.Append(ch);
                continue;
            }
            
            if (ch == ',' && depth == 0)
            {
                properties.Add(current.ToString().Trim());
                current.Clear();
                continue;
            }
            
            current.Append(ch);
        }
        
        // Add the last property
        if (current.Length > 0)
        {
            properties.Add(current.ToString().Trim());
        }
        
        // Parse each property: "PropertyName = Value"
        foreach (var prop in properties)
        {
            if (string.IsNullOrWhiteSpace(prop))
                continue;
            
            var equalIndex = prop.IndexOf('=');
            if (equalIndex <= 0)
                continue;
            
            var key = prop[..equalIndex].Trim();
            var valueStr = prop[(equalIndex + 1)..].Trim();
            
            if (string.IsNullOrWhiteSpace(key))
                continue;
            
            var camelKey = ToCamelCase(key);
            var value = ParseValue(valueStr);
            // Sanitize parsed values to ensure they can be serialized
            result[camelKey] = SanitizeValue(value);
        }
        
        return result;
    }
    
    /// <summary>
    /// Parses a string value into its appropriate type.
    /// </summary>
    private static object? ParseValue(string valueStr)
    {
        if (string.IsNullOrWhiteSpace(valueStr))
            return null;
        
        // Handle null
        if (valueStr.Equals("null", StringComparison.OrdinalIgnoreCase))
            return null;
        
        // Handle quoted strings
        if (valueStr.StartsWith('"') && valueStr.EndsWith('"'))
        {
            return valueStr[1..^1].Replace("\\\"", "\"").Replace("\\\\", "\\");
        }
        
        // Handle booleans
        if (bool.TryParse(valueStr, out var boolValue))
            return boolValue;
        
        // Handle integers
        if (long.TryParse(valueStr, out var longValue))
            return longValue;
        
        // Handle decimals
        if (decimal.TryParse(valueStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var decimalValue))
            return decimalValue;
        
        // Return as string if nothing else matches
        return valueStr;
    }

    /// <summary>
    /// Converts a StructureValue to a nested dictionary structure preserving the object hierarchy.
    /// </summary>
    private static Dictionary<string, object?> ConvertStructureValueToDictionary(StructureValue structure)
    {
        var result = new Dictionary<string, object?>();
        
        foreach (var prop in structure.Properties)
        {
            var camelKey = ToCamelCase(prop.Name);
            
            // If the value is another structure, convert it recursively to preserve nesting
            if (prop.Value is StructureValue nestedStructure)
            {
                result[camelKey] = ConvertStructureValueToDictionary(nestedStructure);
            }
            else
            {
                result[camelKey] = FormatPropertyValue(prop.Value);
            }
        }
        
        return result;
    }

    /// <summary>
    /// Flattens a StructureValue into a dictionary with camelCase keys.
    /// Recursively handles nested structures, using dot notation for nested properties.
    /// Used for flattening scopes into flat properties.
    /// </summary>
    private static Dictionary<string, object?> FlattenStructureValue(StructureValue structure)
    {
        var result = new Dictionary<string, object?>();
        
        foreach (var prop in structure.Properties)
        {
            var camelKey = ToCamelCase(prop.Name);
            
            // If the value is another structure, flatten it recursively
            if (prop.Value is StructureValue nestedStructure)
            {
                var nested = FlattenStructureValue(nestedStructure);
                // Merge nested properties with dot notation for nested objects
                foreach (var nestedKvp in nested)
                {
                    // Sanitize nested values to ensure they can be serialized
                    result[$"{camelKey}.{nestedKvp.Key}"] = SanitizeValue(nestedKvp.Value);
                }
            }
            else
            {
                result[camelKey] = FormatPropertyValue(prop.Value);
            }
        }
        
        return result;
    }
}

