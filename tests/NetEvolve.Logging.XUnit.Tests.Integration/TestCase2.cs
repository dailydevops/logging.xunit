namespace NetEvolve.Logging.XUnit.Tests.Integration;

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

internal sealed partial class TestCase2
{
    private readonly ILogger _logger;

    public TestCase2(ILogger logger) => _logger = logger;

    public TestCase2(ILogger<TestCase2> logger) => _logger = logger;

    public void Run()
    {
        LogBefore(1, null);
        try
        {
            throw new InvalidOperationException();
        }
        catch (Exception ex)
        {
            LogException(ex, "Unknown exception.");
        }
    }

    [LoggerMessage(0, LogLevel.Information, "Before {Number}: {Name}")]
    private partial void LogBefore(int number, string? name);

    [LoggerMessage(1, LogLevel.Error, "Exception: {Message}")]
    private partial void LogException(Exception ex, string message);
}
