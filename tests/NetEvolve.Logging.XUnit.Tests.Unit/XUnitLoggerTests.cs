namespace NetEvolve.Logging.XUnit.Tests.Unit;

using Microsoft.Extensions.Logging;
using Xunit;

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
            { "NONE", LogLevel.None }
        };
}
