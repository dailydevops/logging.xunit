namespace NetEvolve.Logging.XUnit.Tests.Integration;

using Microsoft.Extensions.DependencyInjection;
using Xunit;

public partial class XUnitLoggerExtensionsWithFixtureTests : IClassFixture<TestFixture>
{
    private readonly TestFixture _fixture;

    public XUnitLoggerExtensionsWithFixtureTests(TestFixture fixture) => _fixture = fixture;

    [Theory]
    [MemberData(nameof(AddXUnitData))]
    public void AddXUnit_TestCase1_Expected(XUnitLoggerOptions? options)
    {
        // Arrange
        var services = new ServiceCollection()
            .AddLogging(builder => _ = builder.AddXUnit(_fixture.MessageSink, options))
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
            .AddLogging(builder => _ = builder.AddXUnit(_fixture.MessageSink))
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
        [
            null!,
            XUnitLoggerOptions.Default,
            XUnitLoggerOptions.EnableAllFeatures,
            XUnitLoggerOptions.DisableAllFeatures,
            XUnitLoggerOptions.Minimal,
        ];
}
