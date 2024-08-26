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
            XUnitLoggerOptions.DisableAllFeatures,
            XUnitLoggerOptions.Minimal,
        };
}
