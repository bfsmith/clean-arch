I want to create a custom logging provider for the backend project.

It will consist of two parts:

# Extensions to the ILogger interface
The first part is a set of extensions to the ILogger interface to add additional logging methods. These will be the example usages:

```csharp
logger.Debug("User logged in", new { name = "John" });

logger.Info("This is an info message");
logger.Info("User created", new { name = "John", age = 30 });

logger.Warn("User not found", new { name = "John" });

logger.Error("Invalid username", new { AttemptedName = "admin"});
logger.Error("DB connection failed", new { Exception = ex });

logger.AddContext(new { UserId = 123 });
```

The general pattern is logger.Level("message", new { properties }); The properties are optional and can be used to add additional context to the log message.

AddContext is a different type of method. It adds a Scope to the logging context. It leverages the ILogger.BeginScope method to add the context to the logging context.

# Custom logging provider
The second part is a custom logging provider that will be used to log the messages. It can be written from scratch or use an existing library like Serilog.

The output of all logs will be in JSON format. The format will be:

```json
{
    "timestamp": "2021-01-01T00:00:00.0000000Z",
    "level": "Information",
    "message": "This is an info message",
    "properties": {
        "UserId": 123
    },
    ...additional info like correlation id, request id, etc.
}
```

The timestamp is the UTC timestamp of the log message. The level is the log level. The message is the log message. The properties are the properties of the log message.

The additional info like correlation id, request id, etc. comes from the .NET framework.

All of this will be implemented in the `CleanArch.Logging` project.

# Plan of action

