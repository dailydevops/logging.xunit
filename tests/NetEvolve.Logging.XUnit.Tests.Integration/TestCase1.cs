namespace NetEvolve.Logging.XUnit.Tests.Integration;

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

internal sealed partial class TestCase1
{
    private readonly ILogger _logger;

    public TestCase1(ILogger logger) => _logger = logger;

    public TestCase1(ILogger<TestCase1> logger) => _logger = logger;

    public void Run()
    {
        using var scope1 = _logger.BeginScope("Scope");
#pragma warning disable CA1848 // Use the LoggerMessage delegates
        using var scope2 = _logger.BeginScope("Execution {Now}", DateTimeOffset.Now);
        using var scope3 = _logger.BeginScope(
            new Dictionary<string, object> { { "ExectionTime", DateTimeOffset.Now } }
        );
#pragma warning restore CA1848 // Use the LoggerMessage delegates
        LogTrace();
        LogDebug();
        LogInformation();
        LogWarning();
        LogError();
        LogCritical();
    }

    [LoggerMessage(0, LogLevel.Trace, "Trace")]
    private partial void LogTrace();

    [LoggerMessage(1, LogLevel.Debug, "Debug")]
    private partial void LogDebug();

    [LoggerMessage(2, LogLevel.Information, "Information")]
    private partial void LogInformation();

    [LoggerMessage(3, LogLevel.Warning, "Warning")]
    private partial void LogWarning();

    [LoggerMessage(4, LogLevel.Error, "Error")]
    private partial void LogError();

    [LoggerMessage(5, LogLevel.Critical, "Critical")]
    private partial void LogCritical();
}
