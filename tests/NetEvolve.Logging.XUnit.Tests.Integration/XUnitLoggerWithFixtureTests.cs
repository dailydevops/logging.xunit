namespace NetEvolve.Logging.XUnit.Tests.Integration;

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Time.Testing;
using VerifyXunit;
using Xunit;

public partial class XUnitLoggerWithFixtureTests : IClassFixture<TestFixture>
{
    private readonly TimeProvider _fakeTimeProvider = new FakeTimeProvider(
        new DateTimeOffset(2000, 1, 1, 13, 37, 00, TimeSpan.FromHours(2))
    );
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly TestFixture _fixture;

    private readonly string _timestampFormat = "d";

    public XUnitLoggerWithFixtureTests(ITestOutputHelper testOutputHelper, TestFixture fixture)
    {
        _testOutputHelper = testOutputHelper;
        _fixture = fixture;
    }

    [Theory]
    [MemberData(nameof(LoggedMessageOrToStringData))]
    public async Task LoggedMessages_TestOutputHelper_Theory_Expected(
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
            TimestampFormat = _timestampFormat,
        };
        var logger = XUnitLogger.CreateLogger<TestCase1>(
            _testOutputHelper,
            _fakeTimeProvider,
            new LoggerExternalScopeProvider(),
            options
        );
        var @case = new TestCase1(logger);

        // Act
        @case.Run();

        // Assert
        _ = await Verifier
            .Verify(logger.LoggedMessages)
            .HashParameters()
            .UseParameters(disableAdditionalInformation, disableLogLevel, disableScopes, disableTimestamp);
    }

    [Theory]
    [MemberData(nameof(LoggedMessageOrToStringData))]
    public async Task ToString_TestOutputHelper_Theory_Expected(
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
            TimestampFormat = _timestampFormat,
        };
        var logger = XUnitLogger.CreateLogger<TestCase2>(
            _testOutputHelper,
            _fakeTimeProvider,
            new LoggerExternalScopeProvider(),
            options
        );
        var @case = new TestCase2(logger);

        // Act
        @case.Run();

        // Assert
        _ = await Verifier
            .Verify(logger.ToString())
            .HashParameters()
            .UseParameters(disableAdditionalInformation, disableLogLevel, disableScopes, disableTimestamp);
    }

    [Theory]
    [MemberData(nameof(LoggedMessageOrToStringData))]
    public async Task LoggedMessages_MessageSink_Theory_Expected(
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
            TimestampFormat = _timestampFormat,
        };
        var logger = XUnitLogger.CreateLogger(
            _fixture.MessageSink,
            _fakeTimeProvider,
            new LoggerExternalScopeProvider(),
            options
        );
        var @case = new TestCase1(logger);

        // Act
        for (var sc = 0; sc < 10; sc++)
        {
            @case.Run();
        }

        // Assert
        _ = await Verifier
            .Verify(logger.LoggedMessages)
            .HashParameters()
            .UseParameters(disableAdditionalInformation, disableLogLevel, disableScopes, disableTimestamp);
    }

    [Theory]
    [MemberData(nameof(LoggedMessageOrToStringData))]
    public async Task ToString_MessageSink_Theory_Expected(
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
            TimestampFormat = _timestampFormat,
        };
        var logger = XUnitLogger.CreateLogger(
            _fixture.MessageSink,
            _fakeTimeProvider,
            new LoggerExternalScopeProvider(),
            options
        );
        var @case = new TestCase2(logger);

        // Act
        for (var sc = 0; sc < 10; sc++)
        {
            @case.Run();
        }

        // Assert
        _ = await Verifier
            .Verify(logger.ToString())
            .HashParameters()
            .UseParameters(disableAdditionalInformation, disableLogLevel, disableScopes, disableTimestamp);
    }

    [Theory]
    [MemberData(nameof(LoggedMessageOrToStringData))]
    public async Task LoggedMessages_MessageSinkGeneric_Theory_Expected(
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
            TimestampFormat = _timestampFormat,
        };
        var logger = XUnitLogger.CreateLogger<TestCase1>(
            _fixture.MessageSink,
            _fakeTimeProvider,
            new LoggerExternalScopeProvider(),
            options
        );
        var @case = new TestCase1(logger);

        // Act
        for (var sc = 0; sc < 10; sc++)
        {
            @case.Run();
        }

        // Assert
        _ = await Verifier
            .Verify(logger.LoggedMessages)
            .HashParameters()
            .UseParameters(disableAdditionalInformation, disableLogLevel, disableScopes, disableTimestamp);
    }

    [Theory]
    [MemberData(nameof(LoggedMessageOrToStringData))]
    public async Task ToString_MessageSinkGeneric_Theory_Expected(
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
            TimestampFormat = _timestampFormat,
        };
        var logger = XUnitLogger.CreateLogger<TestCase2>(
            _fixture.MessageSink,
            _fakeTimeProvider,
            new LoggerExternalScopeProvider(),
            options
        );
        var @case = new TestCase2(logger);

        // Act
        for (var sc = 0; sc < 10; sc++)
        {
            @case.Run();
        }

        // Assert
        _ = await Verifier
            .Verify(logger.ToString())
            .HashParameters()
            .UseParameters(disableAdditionalInformation, disableLogLevel, disableScopes, disableTimestamp);
    }

    public static TheoryData<bool, bool, bool, bool> LoggedMessageOrToStringData =>
        new TheoryData<bool, bool, bool, bool>
        {
            { false, false, false, false },
            { true, false, false, false },
            { false, true, false, false },
            { false, false, true, false },
            { false, false, false, true },
            { true, true, true, true },
        };
}
