namespace NetEvolve.Logging.XUnit.Tests.Integration;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Time.Testing;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public partial class XUnitLoggerTests
{
    private readonly TimeProvider _fakeTimeProvider = new FakeTimeProvider(
        new DateTimeOffset(2000, 1, 1, 13, 37, 00, TimeSpan.FromHours(2))
    );
    private readonly ITestOutputHelper _testOutputHelper;

    public XUnitLoggerTests(ITestOutputHelper testOutputHelper) =>
        _testOutputHelper = testOutputHelper;

    [Theory]
    [MemberData(nameof(LoggedMessageOrToStringData))]
    public async Task LoggedMessages_Theory_Expected(
        bool disableAdditionalInformation,
        bool disableLogLevel,
        bool disableScopes,
        bool disableTimestamp,
        string? formatTimestamp
    )
    {
        var options = new XUnitLoggerOptions
        {
            DisableAdditionalInformation = disableAdditionalInformation,
            DisableLogLevel = disableLogLevel,
            DisableScopes = disableScopes,
            DisableTimestamp = disableTimestamp,
            TimestampFormat = formatTimestamp!
        };
        var logger = XUnitLogger.CreateLogger<TestCase>(
            _testOutputHelper,
            _fakeTimeProvider,
            new LoggerExternalScopeProvider(),
            options
        );
        var @case = new TestCase(logger);

        // Act
        @case.Run();

        // Assert
        _ = await Verifier
            .Verify(logger.LoggedMessages)
            .UseDirectory("_snapshots")
            .UseHashedParameters(
                disableAdditionalInformation,
                disableLogLevel,
                disableScopes,
                disableTimestamp,
                formatTimestamp
            );
    }

    [Theory]
    [MemberData(nameof(LoggedMessageOrToStringData))]
    public async Task ToString_Theory_Expected(
        bool disableAdditionalInformation,
        bool disableLogLevel,
        bool disableScopes,
        bool disableTimestamp,
        string? formatTimestamp
    )
    {
        var options = new XUnitLoggerOptions
        {
            DisableAdditionalInformation = disableAdditionalInformation,
            DisableLogLevel = disableLogLevel,
            DisableScopes = disableScopes,
            DisableTimestamp = disableTimestamp,
            TimestampFormat = formatTimestamp!
        };
        var logger = XUnitLogger.CreateLogger<TestCase>(
            _testOutputHelper,
            _fakeTimeProvider,
            new LoggerExternalScopeProvider(),
            options
        );
        var @case = new TestCase(logger);

        // Act
        @case.Run();

        // Assert
        _ = await Verifier
            .Verify(logger.ToString())
            .UseDirectory("_snapshots")
            .UseHashedParameters(
                disableAdditionalInformation,
                disableLogLevel,
                disableScopes,
                disableTimestamp,
                formatTimestamp
            );
    }

    public static TheoryData<bool, bool, bool, bool, string?> LoggedMessageOrToStringData =>
        new TheoryData<bool, bool, bool, bool, string?>
        {
            { false, false, false, false, null },
            { true, false, false, false, null },
            { false, true, false, false, null },
            { false, false, true, false, null },
            { false, false, false, true, null },
            { false, false, false, false, "yyyy-MM-dd HH:mm:ss" }
        };

    private sealed partial class TestCase
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
}
