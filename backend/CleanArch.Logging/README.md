The following is an opinionated logging format and structure.

# Extensions

The library includes several extension methods for the `ILogger` interface.

```csharp
ILogger.Debug(string message, object? context = null);
ILogger.Info(string message, object? context = null);
ILogger.Warn(string message, object? context = null);
ILogger.Error(string message, object? context = null);
````

These are intended to replace their built-in counterparts: `.LogDebug()`,
`.LogInformation()`, etc. They take a string message and an optional object that describes the context for the log. If no context is needed, the parameter can be left out.

```csharp
logger.Info("Database initialized"); // No context

logger.Info("User login successful", new { Username = user.Name }); // Context
```

**Notice
**: In both cases, the message text is static. The dynamic information for the message is only entered into the context.

```csharp
IDisposable ILogger.AddContext<TContext>(TContext context) where TContext : notnull
```

This method is intended to replace the `.BeginScope()` method. This method is used to add context to logs without writing a log message. While this might seem odd at first, it is quite powerful.

Let's say you receive a REST request and extract the userId from the auth token. You may want to include the userId in all logs that happen while processing that request. Rather than adding the userId to the context each time, and possibly having to pass the userId down all the method chain to do so, you can add it to the context once at the beginning of the request. All subsequent logs will include the userId in their context automatically.

This works because the method returns an `IDisposable`. Anything that is logged before that `IDisposable` is disposed, will include the context passed in. 

```csharp
using(logger.AddContext(new { UserId = user.Id }))
{
    logger.Info("Begin user request");
    await service.DoRequestedThingAsync();
    logger.Info("End user request");
}
```

Let's walk through what happens when this code runs:

```csharp
using(logger.AddContext(new { UserId = user.Id }))
```

This adds an anonymous object to the context. The object has a single field, `UserId` which is set to the `Id` of the user. The method returns an `IDisposable`, which is captured in the `using`. When the `using` block ends, the `IDisposable` is guaranteed to dispose, even if an exception is thrown within the block.

```csharp
logger.Info("Begin user request");
```

The beginning of the request is logged. Since this request is in the scope of the `using`, it will include the `UserId` in its context. The line for the log will include the following properties.

```json
{
  "message": "Begin user request",
  "context": {
    "UserId": "123"
  }
}
```

```csharp
await service.DoRequestedThingAsync();
```

This calls some other service to do the real work of the request. If any messages are logged within this request those messages _WILL_ also contain the `UserId` in the context, since they are within the `using` scope as well. Synchronous and asynchronous methods behave the same with respect to the context scope.

```csharp
logger.Info("End user request");
```

The final log message to say the request has completed. Again, this will include `UserId` in its context since it is still within the scope of the `using` from `.AddContext()`.

**Notice**: The first set of methods, `.Debug()`, etc., do not return an `IDisposable`. This is because the context passed into them is only used for that message, so no scope needs to be maintained.

# Context

Context is where any dynamic information relevant to the log message goes. Context can be added from any of the methods.

The examples use anonymous objects, `new { FieldName = value }`. This is a convenient way to create temporary objects with whatever fields are relevant without creating a class for them. This is not a requirement, you could pass in a class, a `Dictionary`, etc. and it would work. 

⚠️ Avoid passing an enumerable as the context, it makes parsing the resulting message more difficult. Instead, set it as a field in an anonymous object.

```csharp
List<int> myList = [1, 2, 3];
using (logger.AddContext(myList)) {} // ❌ Bad: enumerable as context
using (logger.AddContext(new { MyList = myList })) {} // ✅ Good: enumerable as field on context
```

Fields on the context can be simple (int, string, boolean, etc) or complex (class, record, Exception).

Complex objects will be serialized using `System.Text.Json` serialization. Include the proper attributes to control how a class serializes for the logs.

## Properly disposing of the context

Failing to dispose of the `IDisposable` will result in unexpected behaviors.

```csharp
logger.AddContext(new { UserId = user.Id }); // ❌ Bad: IDisposable not handled properly

using (logger.AddContext(new { UserId = user.Id })) // ✅ Good: using will automatically dispose IDisposable 
{
    // Other code
}

using var myContext = logger.AddContext(new { UserId = user.Id }); // ✅ Good: using will dispose myContext automatically when myContext is no longer in scope 
// other code
```

The first line does not properly handle the returned IDisposable and may result in unexpected behavior including the context bleed and memory leaks.

The second set of lines wrap the IDisposable in a `using` block. Use this when you want to be explicit about the scope of the context.

The third set of lines capture the result in a variable, `myContext`, and preface it with `using`. This results in `myContext` being disposed of when it is no longer in scope. Notice that this style does not need the curly braces. This style reduces the amount of nesting the code has, but introduces a variable.

Use whichever of the two good methods make sense for your situation.

# Anonymous Objects

# Why not Structured Logging?

Structured logging is the built-in way to add context to logs in .NET. A log to record a user login failure might look something like this.

```csharp
logger.LogInformation("User '{username}' login failed", user.Name);
```

Structured Logging solves a very real problem: including context with log messages. This is makes debugging easier because you can log the value that caused the problem, what user was executing the query, etc.

Adding context to logs is a great feature, but the API was poorly designed and leads to multiple difficulties when trying to use the log output.

1. Message text is dynamic. Because the context is referenced in the message, the message is not static text. This makes it difficult to search through logs to find. "User login failed" vs "User 'bob' login failed".
2. The 

# TODO:
1. Add setup
2. Quickstart
3. Example output structure
4. Finish "Why not Structured Logging?" section
5. Handle sensitive fields (don't log them)
6. Can I avoid dictionary-ifying at log time and instead let Serilog do it, then combine the scopes at that point?