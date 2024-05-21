namespace NetEvolve.Logging.XUnit.Tests.Integration;

using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

public partial class XUnitLoggerExtensionsTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public XUnitLoggerExtensionsTests(ITestOutputHelper testOutputHelper) =>
        _testOutputHelper = testOutputHelper;

    [Theory]
    [MemberData(nameof(AddXUnitData))]
    public void AddXUnit_TestCase1_Expected(XUnitLoggerOptions? options)
    {
        // Arrange
        var services = new ServiceCollection()
            .AddLogging(builder => _ = builder.AddXUnit(_testOutputHelper, options))
            .AddSingleton<TestCase1>();
        using var serviceProvider = services.BuildServiceProvider();

        // Act
        var ex = Record.Exception(() =>
        {
            var testCase = serviceProvider.GetRequiredService<TestCase1>();

            testCase.Run();
        });

        // Assert
        Assert.Null(ex);
    }

    [Fact]
    public void AddXUnit_TestCase2_Expected()
    {
        // Arrange
        var services = new ServiceCollection()
            .AddLogging(builder => _ = builder.AddXUnit(_testOutputHelper))
            .AddSingleton<TestCase2>();
        using var serviceProvider = services.BuildServiceProvider();

        // Act
        var ex = Record.Exception(() =>
        {
            var testCase = serviceProvider.GetRequiredService<TestCase2>();

            testCase.Run();
        });

        // Assert
        Assert.Null(ex);
    }

    public static TheoryData<XUnitLoggerOptions?> AddXUnitData =>
        new TheoryData<XUnitLoggerOptions?>
        {
            null,
            XUnitLoggerOptions.Default,
            XUnitLoggerOptions.EnableAllFeatures,
            XUnitLoggerOptions.DisableAllFeatures
        };

#pragma warning disable CA1812 // Avoid uninstantiated internal classes
    private sealed partial class TestCase1
    {
        private readonly ILogger _logger;

#pragma warning disable S1144 // Unused private types or members should be removed
        public TestCase1(ILogger<TestCase1> logger) => _logger = logger;
#pragma warning restore S1144 // Unused private types or members should be removed

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

    private sealed partial class TestCase2
    {
        private readonly ILogger<TestCase2> _logger;

#pragma warning disable S1144 // Unused private types or members should be removed
        public TestCase2(ILogger<TestCase2> logger) => _logger = logger;
#pragma warning restore S1144 // Unused private types or members should be removed

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
#pragma warning restore CA1812 // Avoid uninstantiated internal classes
}
