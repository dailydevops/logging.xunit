namespace NetEvolve.Logging.XUnit.Tests.Integration;

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

internal sealed partial class TestCase
{
    private readonly ILogger<TestCase> _logger;

    public TestCase(ILogger<TestCase> logger) => _logger = logger;

    public void Run()
    {
        using var scopeNull = _logger.BeginScope((string)null!);
        using var scopeOne = _logger.BeginScope(
            new Dictionary<string, object?> { { "MethodName", nameof(Run) } }
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
    }
}
