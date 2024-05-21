# NetEvolve.Logging.XUnit

This library provides a logging implementation for [XUnit](https://xunit.net/). When using this library, you have the ability to access the logs generated while executing your tests. This can be useful for debugging purposes.

## Installation
```bash
dotnet add package NetEvolve.Logging.XUnit
```

## Usage

You can choose to use the `XUnitLogger` class directly or use the `AddXUnit` extension method on the `ILoggingBuilder` instance.

### Direct usage

```csharp
using Microsoft.Extensions.Logging;
using NetEvolve.Logging.XUnit;
using XUnit;

public class ExampleTests
{
    private readonly ITestOutputHelper _output;

    public ExampleTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void Test()
    {
        var logger = XUnitLogger.CreateLogger<ExampleTests>(_output);

        // Arrange
        ...
        // Act
        ...
        // Assert
        ...

        Assert.NotEmpty(logger.LoggedMessages);
    }
}
```

### Usage with `ILoggingBuilder.AddXUnit`

Or you can use the `AddXUnit` extension method on the `ILoggingBuilder` instance.

```csharp
using Microsoft.Extensions.Logging;
using NetEvolve.Logging.XUnit;

var services = new ServiceCollection();
services.AddLogging(builder =>
{
    // Add the XUnit logging implementation
    builder.AddXUnit();

    // Or alternatively with options
    builder.AddXUnit(options =>
    {
        options.TimestampFormat = "HH:mm:ss.fff";
    });
});
```