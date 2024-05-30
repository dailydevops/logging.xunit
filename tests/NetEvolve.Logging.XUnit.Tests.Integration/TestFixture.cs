namespace NetEvolve.Logging.XUnit.Tests.Integration;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

#pragma warning disable CA1812
internal sealed partial class TestFixture : IAsyncLifetime
{
    private readonly ILoggerFactory _loggerFactory;

    private readonly ILogger _logger;
    private readonly IMessageSink _messageSink;

#pragma warning disable S1144 // Unused private types or members should be removed
    public TestFixture(IMessageSink messageSink)
    {
        _messageSink = messageSink;
        _loggerFactory = LoggerFactory.Create(builder => builder.AddXUnit(_messageSink));
        _logger = _loggerFactory.CreateLogger<XUnitLoggerWithFixtureTests>();
    }
#pragma warning restore S1144 // Unused private types or members should be removed

    public Task DisposeAsync() => Task.CompletedTask;

    public async Task InitializeAsync() => await RunAsync();

    private ValueTask RunAsync()
    {
        using var scopeNull = _logger.BeginScope((string)null!);
        using var scopeOne = _logger.BeginScope(
            new Dictionary<string, object?> { { "MethodName", nameof(RunAsync) } }
        );
#pragma warning disable CA1848 // Use the LoggerMessage delegates
        try
        {
            _logger.LogTrace("This is a Trace.");
            _logger.LogDebug("This is a Debug.");
            _logger.LogInformation("This is an Information.");
            _logger.LogWarning("This is a Warning.");
            _logger.LogError("This is an Error.");
            _logger.LogCritical("This is a Critical.");

            throw new NotImplementedException();
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "This is a Critical with exception.");
        }
#pragma warning restore CA1848 // Use the LoggerMessage delegates

        return ValueTask.CompletedTask;
    }
}
#pragma warning restore CA1812
