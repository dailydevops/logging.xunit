namespace NetEvolve.Logging.XUnit.Tests.Unit;

using System;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;
using Xunit.Sdk;

public partial class XUnitLoggerTests
{
    [Theory]
    [MemberData(nameof(LogLevelToStringData))]
    public void LogLevelToString_Theory_Expected(string expected, LogLevel logLevel)
    {
        // Arrange
        // Act
        var result = XUnitLogger.LogLevelToString(logLevel);

        // Assert
        Assert.Equal(expected, result);
    }

    public static TheoryData<string, LogLevel> LogLevelToStringData =>
        new TheoryData<string, LogLevel>
        {
            { "TRCE", LogLevel.Trace },
            { "DBUG", LogLevel.Debug },
            { "INFO", LogLevel.Information },
            { "WARN", LogLevel.Warning },
            { "FAIL", LogLevel.Error },
            { "CRIT", LogLevel.Critical },
            { "NONE", LogLevel.None },
        };

    [Fact]
    public void CreateLogger_WithMessageSinkNull_ThrowsArgumentNullException()
    {
        IMessageSink messageSink = null!;

        _ = Assert.Throws<ArgumentNullException>("messageSink", () => XUnitLogger.CreateLogger(messageSink));
    }

    [Fact]
    public void CreateLogger_WithMessageSinkNullAndTimeProviderNull_ThrowsArgumentNullException()
    {
        IMessageSink messageSink = null!;
        TimeProvider timeProvider = null!;

        _ = Assert.Throws<ArgumentNullException>(
            "messageSink",
            () => XUnitLogger.CreateLogger(messageSink, timeProvider)
        );
    }

    [Fact]
    public void CreateLogger_WithMessageSinkSubstituteAndTimeProviderNull_ThrowsArgumentNullException()
    {
        var messageSink = Substitute.For<IMessageSink>();
        TimeProvider timeProvider = null!;

        _ = Assert.Throws<ArgumentNullException>(
            "timeProvider",
            () => XUnitLogger.CreateLogger(messageSink, timeProvider)
        );
    }

    [Fact]
    public void CreateLoggerGeneric_WithMessageSinkNull_ThrowsArgumentNullException()
    {
        IMessageSink messageSink = null!;

        _ = Assert.Throws<ArgumentNullException>(
            "messageSink",
            () => XUnitLogger.CreateLogger<XUnitLoggerTests>(messageSink)
        );
    }

    [Fact]
    public void CreateLoggerGeneric_WithMessageSinkNullAndTimeProviderNull_ThrowsArgumentNullException()
    {
        IMessageSink messageSink = null!;
        TimeProvider timeProvider = null!;

        _ = Assert.Throws<ArgumentNullException>(
            "messageSink",
            () => XUnitLogger.CreateLogger<XUnitLoggerTests>(messageSink, timeProvider)
        );
    }

    [Fact]
    public void CreateLoggerGeneric_WithMessageSinkSubstituteAndTimeProviderNull_ThrowsArgumentNullException()
    {
        var messageSink = Substitute.For<IMessageSink>();
        TimeProvider timeProvider = null!;

        _ = Assert.Throws<ArgumentNullException>(
            "timeProvider",
            () => XUnitLogger.CreateLogger<XUnitLoggerTests>(messageSink, timeProvider)
        );
    }

    [Fact]
    public void CreateLogger_WithTestOutputHelperNull_ThrowsArgumentNullException()
    {
        ITestOutputHelper testOutputHelper = null!;

        _ = Assert.Throws<ArgumentNullException>("testOutputHelper", () => XUnitLogger.CreateLogger(testOutputHelper));
    }

    [Fact]
    public void CreateLogger_WithTestOutputHelperNullAndTimeProviderNull_ThrowsArgumentNullException()
    {
        ITestOutputHelper testOutputHelper = null!;
        TimeProvider timeProvider = null!;

        _ = Assert.Throws<ArgumentNullException>(
            "testOutputHelper",
            () => XUnitLogger.CreateLogger(testOutputHelper, timeProvider)
        );
    }

    [Fact]
    public void CreateLogger_WithTestOutputHelperSubstituteAndTimeProviderNull_ThrowsArgumentNullException()
    {
        var testOutputHelper = Substitute.For<ITestOutputHelper>();
        TimeProvider timeProvider = null!;

        _ = Assert.Throws<ArgumentNullException>(
            "timeProvider",
            () => XUnitLogger.CreateLogger(testOutputHelper, timeProvider)
        );
    }

    [Fact]
    public void CreateLoggerGeneric_WithTestOutputHelperNull_ThrowsArgumentNullException()
    {
        ITestOutputHelper testOutputHelper = null!;

        _ = Assert.Throws<ArgumentNullException>(
            "testOutputHelper",
            () => XUnitLogger.CreateLogger<XUnitLoggerTests>(testOutputHelper)
        );
    }

    [Fact]
    public void CreateLoggerGeneric_WithTestOutputHelperNullAndTimeProviderNull_ThrowsArgumentNullException()
    {
        ITestOutputHelper testOutputHelper = null!;
        TimeProvider timeProvider = null!;

        _ = Assert.Throws<ArgumentNullException>(
            "testOutputHelper",
            () => XUnitLogger.CreateLogger<XUnitLoggerTests>(testOutputHelper, timeProvider)
        );
    }

    [Fact]
    public void CreateLoggerGeneric_WithTestOutputHelperSubstituteAndTimeProviderNull_ThrowsArgumentNullException()
    {
        var testOutputHelper = Substitute.For<ITestOutputHelper>();
        TimeProvider timeProvider = null!;

        _ = Assert.Throws<ArgumentNullException>(
            "timeProvider",
            () => XUnitLogger.CreateLogger<XUnitLoggerTests>(testOutputHelper, timeProvider)
        );
    }
}
