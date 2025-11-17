using System.Text;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Extensions.Logging;
using CleanArch.Logging;

namespace CleanArch.Logging.Tests;

/// <summary>
/// Helper class for creating loggers that capture output for testing
/// </summary>
public static class TestLoggerHelper
{
    /// <summary>
    /// Creates a logger that writes to a StringWriter for testing
    /// </summary>
    public static (Microsoft.Extensions.Logging.ILogger Logger, StringWriter Output) CreateCapturingLogger(LogEventLevel minimumLevel = LogEventLevel.Debug)
    {
        var output = new StringWriter();
        var formatter = new CustomJsonFormatter();
        
        var serilogLogger = new LoggerConfiguration()
            .MinimumLevel.Is(minimumLevel)
            .WriteTo.Sink(new TextWriterSink(output, formatter))
            .CreateLogger();

        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddSerilog(serilogLogger, dispose: true);
        });

        return (loggerFactory.CreateLogger("Test"), output);
    }

    /// <summary>
    /// Simple sink that writes to a TextWriter using a formatter
    /// </summary>
    private class TextWriterSink : ILogEventSink
    {
        private readonly TextWriter _writer;
        private readonly Serilog.Formatting.ITextFormatter _formatter;

        public TextWriterSink(TextWriter writer, Serilog.Formatting.ITextFormatter formatter)
        {
            _writer = writer;
            _formatter = formatter;
        }

        public void Emit(LogEvent logEvent)
        {
            _formatter.Format(logEvent, _writer);
        }
    }

    /// <summary>
    /// Parses JSON log output and returns the log entries
    /// </summary>
    public static List<Dictionary<string, object?>> ParseJsonLogs(string jsonOutput)
    {
        var logs = new List<Dictionary<string, object?>>();
        var lines = jsonOutput.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            try
            {
                var logEntry = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object?>>(line);
                if (logEntry != null)
                {
                    logs.Add(logEntry);
                }
            }
            catch
            {
                // Skip invalid JSON lines
            }
        }

        return logs;
    }
}

