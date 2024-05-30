﻿namespace NetEvolve.Logging.XUnit.Tests.Integration;

using System;
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
}
