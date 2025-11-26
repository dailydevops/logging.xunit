# NetEvolve.Logging.XUnit

[![NuGet](https://img.shields.io/nuget/v/NetEvolve.Logging.XUnit.svg)](https://www.nuget.org/packages/NetEvolve.Logging.XUnit/)
[![License](https://img.shields.io/github/license/dailydevops/logging.xunit.svg)](LICENSE)

A powerful logging implementation for [xUnit.net](https://xunit.net/) that enables you to capture, inspect, and assert on log messages generated during test execution. Perfect for debugging tests, verifying logging behavior, and building testable applications with comprehensive logging coverage.

## Features

- ✅ **Full `Microsoft.Extensions.Logging` integration** - Works seamlessly with the standard .NET logging infrastructure
- ✅ **Message inspection and assertions** - Access all logged messages for verification in your tests
- ✅ **Multiple creation patterns** - Direct instantiation or dependency injection via `ILoggingBuilder`
- ✅ **Configurable output** - Control timestamps, log levels, scopes, and additional information
- ✅ **Scope support** - Captures and displays logging scopes for better context
- ✅ **`TimeProvider` support** - Testable time-based logging scenarios
- ✅ **xUnit v3 compatibility** - Works with both `ITestOutputHelper` and `IMessageSink`
- ✅ **Generic logger support** - Strongly-typed loggers with `XUnitLogger<T>`

## Installation

```bash
dotnet add package NetEvolve.Logging.XUnit
```

Or via the NuGet Package Manager:

```powershell
Install-Package NetEvolve.Logging.XUnit
```

## Quick Start

### Basic Usage with `ITestOutputHelper`

```csharp
using Microsoft.Extensions.Logging;
using NetEvolve.Logging.XUnit;
using Xunit;
using Xunit.Abstractions;

public class ExampleTests
{
    private readonly ITestOutputHelper _output;

    public ExampleTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void BasicLoggingExample()
    {
        // Create a logger
        var logger = XUnitLogger.CreateLogger<ExampleTests>(_output);

        // Use the logger
        logger.LogInformation("Starting test execution");
        logger.LogDebug("Debug information: {Value}", 42);

        // Perform test logic
        var result = 2 + 2;

        logger.LogInformation("Calculation result: {Result}", result);

        // Assert on log messages
        Assert.NotEmpty(logger.LoggedMessages);
        Assert.Contains(logger.LoggedMessages, m => m.Message.Contains("Starting test"));
    }
}
```

## Usage Patterns

### 1. Direct Logger Creation

Create loggers directly using the static factory methods:

```csharp
// Generic typed logger
var logger = XUnitLogger.CreateLogger<MyClass>(_output);

// Non-generic logger
var logger = XUnitLogger.CreateLogger(_output);

// With custom options
var options = new XUnitLoggerOptions
{
    TimestampFormat = "HH:mm:ss.fff",
    DisableScopes = false
};
var logger = XUnitLogger.CreateLogger<MyClass>(_output, options: options);

// With custom TimeProvider (for testing)
var timeProvider = new FakeTimeProvider();
var logger = XUnitLogger.CreateLogger<MyClass>(_output, timeProvider, options: options);
```

### 2. Dependency Injection Integration

Use the `AddXUnit` extension method with `ILoggingBuilder`:

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetEvolve.Logging.XUnit;
using Xunit;
using Xunit.Abstractions;

public class ServiceTests
{
    private readonly ITestOutputHelper _output;

    public ServiceTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void TestWithDependencyInjection()
    {
        // Configure services with xUnit logging
        var services = new ServiceCollection();
        services.AddLogging(builder =>
        {
            builder.AddXUnit(_output, new XUnitLoggerOptions
            {
                TimestampFormat = "HH:mm:ss.fff"
            });
        });

        // Add your services
        services.AddTransient<MyService>();

        // Build and use
        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetRequiredService<MyService>();

        // Your service will now log to xUnit output
        service.DoSomething();
    }
}
```

### 3. Using with `IMessageSink` (xUnit v3)

For xUnit v3 scenarios where `ITestOutputHelper` isn't available:

```csharp
using NetEvolve.Logging.XUnit;
using Xunit.Sdk;

public class FrameworkTests
{
    private readonly IMessageSink _messageSink;

    public FrameworkTests(IMessageSink messageSink)
    {
        _messageSink = messageSink;
    }

    [Fact]
    public void TestWithMessageSink()
    {
        var logger = XUnitLogger.CreateLogger(_messageSink);
        
        logger.LogInformation("This will be written to the message sink");
    }
}
```

## Configuration Options

Customize logger behavior using `XUnitLoggerOptions`:

```csharp
var options = new XUnitLoggerOptions
{
    // Timestamp format (default: "o" - ISO 8601)
    TimestampFormat = "yyyy-MM-dd HH:mm:ss.fff",
    
    // Disable timestamp output (default: false)
    DisableTimestamp = false,
    
    // Disable log level output (default: false)
    DisableLogLevel = false,
    
    // Disable scope information (default: false)
    DisableScopes = false,
    
    // Disable additional structured data (default: false)
    DisableAdditionalInformation = false
};

var logger = XUnitLogger.CreateLogger<MyClass>(_output, options: options);
```

### Output Format Examples

**Default output:**
```
2025-11-26T10:30:45.123+00:00 [INFO] User logged in successfully
    UserId: 123
    Username: john.doe
```

**Minimal output (all optional features disabled):**
```
User logged in successfully
```

**Custom timestamp format:**
```
10:30:45.123 [INFO] User logged in successfully
```

## Accessing Logged Messages

The logger captures all messages for inspection and assertions:

```csharp
[Fact]
public void VerifyLoggingBehavior()
{
    var logger = XUnitLogger.CreateLogger<MyClass>(_output);
    
    // Perform operations that generate logs
    logger.LogInformation("Operation started");
    logger.LogWarning("Potential issue detected");
    logger.LogError("Operation failed");
    
    // Access all logged messages
    var messages = logger.LoggedMessages;
    
    // Assertions
    Assert.Equal(3, messages.Count);
    Assert.Contains(messages, m => m.LogLevel == LogLevel.Information);
    Assert.Contains(messages, m => m.LogLevel == LogLevel.Warning);
    Assert.Contains(messages, m => m.LogLevel == LogLevel.Error);
    Assert.Contains(messages, m => m.Message.Contains("Operation started"));
    
    // Check timestamps
    Assert.All(messages, m => Assert.True(m.Timestamp != default));
    
    // Check for exceptions
    var errorMessage = messages.First(m => m.LogLevel == LogLevel.Error);
    Assert.Null(errorMessage.Exception); // or Assert.NotNull if exception was logged
}
```

## Advanced Scenarios

### Working with Logging Scopes

```csharp
[Fact]
public void TestWithScopes()
{
    var logger = XUnitLogger.CreateLogger<MyClass>(_output);
    
    using (logger.BeginScope("Operation {OperationId}", "12345"))
    {
        logger.LogInformation("Processing started");
        
        using (logger.BeginScope("Step {StepNumber}", 1))
        {
            logger.LogDebug("Step 1 execution");
        }
        
        logger.LogInformation("Processing completed");
    }
    
    // Verify scope information was captured
    var messages = logger.LoggedMessages;
    Assert.All(messages, m => Assert.NotEmpty(m.Scopes));
}
```

### Testing Time-Dependent Logging

```csharp
[Fact]
public void TestWithFakeTimeProvider()
{
    var fakeTime = new FakeTimeProvider();
    fakeTime.SetUtcNow(new DateTimeOffset(2025, 1, 1, 12, 0, 0, TimeSpan.Zero));
    
    var logger = XUnitLogger.CreateLogger<MyClass>(_output, fakeTime);
    
    logger.LogInformation("First message");
    
    // Advance time
    fakeTime.Advance(TimeSpan.FromMinutes(5));
    
    logger.LogInformation("Second message");
    
    // Verify timestamps
    var messages = logger.LoggedMessages;
    Assert.Equal(5, (messages[1].Timestamp - messages[0].Timestamp).TotalMinutes);
}
```

### Integration Testing Example

```csharp
public class IntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly ITestOutputHelper _output;

    public IntegrationTests(WebApplicationFactory<Program> factory, ITestOutputHelper output)
    {
        _factory = factory;
        _output = output;
    }

    [Fact]
    public async Task ApiEndpoint_LogsRequestDetails()
    {
        // Create a custom web application factory with xUnit logging
        var factory = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddXUnit(_output);
                logging.SetMinimumLevel(LogLevel.Debug);
            });
        });

        var client = factory.CreateClient();
        
        // Make request
        var response = await client.GetAsync("/api/users");
        
        // All logging from the API will appear in test output
        Assert.True(response.IsSuccessStatusCode);
    }
}
```

## Best Practices

1. **Use generic loggers** - `CreateLogger<T>()` provides better categorization
2. **Assert on log messages** - Verify your application logs appropriate information
3. **Configure log levels** - Use appropriate log levels in tests (Debug for detailed info)
4. **Use scopes** - Provide context for complex operations
5. **Inspect `LoggedMessages`** - Validate logging behavior as part of your tests
6. **Disable optional features** - Reduce output noise when needed using `XUnitLoggerOptions`

## Troubleshooting

### Logs not appearing in test output

Ensure you're passing the correct `ITestOutputHelper` instance:

```csharp
public class MyTests
{
    private readonly ITestOutputHelper _output;

    // Constructor injection
    public MyTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void TestMethod()
    {
        // Use the injected instance
        var logger = XUnitLogger.CreateLogger<MyTests>(_output);
        logger.LogInformation("This will appear in test output");
    }
}
```

### Exception: "There is no currently active test"

This occurs when trying to write to `ITestOutputHelper` outside of test execution. Ensure logging only happens during test execution, not in class constructors or after test completion.

## Target Frameworks

- .NET 6.0 and later
- Full support for `Microsoft.Extensions.Logging` abstractions
- Compatible with xUnit.net v3

## License

This project is licensed under the MIT License - see the [LICENSE](../../LICENSE) file for details.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## Links

- [NuGet Package](https://www.nuget.org/packages/NetEvolve.Logging.XUnit/)
- [GitHub Repository](https://github.com/dailydevops/logging.xunit)
- [xUnit.net Documentation](https://xunit.net/)
- [Microsoft.Extensions.Logging Documentation](https://learn.microsoft.com/en-us/dotnet/core/extensions/logging)