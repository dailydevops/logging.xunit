namespace NetEvolve.Logging.XUnit.Tests.Integration;

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Time.Testing;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public partial class XUnitLoggerWithFixtureTests : IClassFixture<TestFixture>
{
    private readonly TimeProvider _fakeTimeProvider = new FakeTimeProvider(
        new DateTimeOffset(2000, 1, 1, 13, 37, 00, TimeSpan.FromHours(2))
    );
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly TestFixture _fixture;

    public XUnitLoggerWithFixtureTests(ITestOutputHelper testOutputHelper, TestFixture fixture)
    {
        _testOutputHelper = testOutputHelper;
        _fixture = fixture;
    }

    [Theory]
    [MemberData(nameof(LoggedMessageOrToStringData))]
    public async Task LoggedMessages_Theory_Expected(
        bool disableAdditionalInformation,
        bool disableLogLevel,
        bool disableScopes,
        bool disableTimestamp
    )
    {
        var options = new XUnitLoggerOptions
        {
            DisableAdditionalInformation = disableAdditionalInformation,
            DisableLogLevel = disableLogLevel,
            DisableScopes = disableScopes,
            DisableTimestamp = disableTimestamp,
            TimestampFormat = _fixture.TimestampFormat
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
                disableTimestamp
            );
    }

    [Theory]
    [MemberData(nameof(LoggedMessageOrToStringData))]
    public async Task ToString_Theory_Expected(
        bool disableAdditionalInformation,
        bool disableLogLevel,
        bool disableScopes,
        bool disableTimestamp
    )
    {
        var options = new XUnitLoggerOptions
        {
            DisableAdditionalInformation = disableAdditionalInformation,
            DisableLogLevel = disableLogLevel,
            DisableScopes = disableScopes,
            DisableTimestamp = disableTimestamp,
            TimestampFormat = _fixture.TimestampFormat
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
                disableTimestamp
            );
    }

    public static TheoryData<bool, bool, bool, bool> LoggedMessageOrToStringData =>
        new TheoryData<bool, bool, bool, bool>
        {
            { false, false, false, false },
            { true, false, false, false },
            { false, true, false, false },
            { false, false, true, false },
            { false, false, false, true },
            { false, false, false, false }
        };
}
