namespace NetEvolve.Logging.XUnit.Tests.Unit;

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

public class XUnitLoggerExtensionsTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public XUnitLoggerExtensionsTests(ITestOutputHelper testOutputHelper) => _testOutputHelper = testOutputHelper;

    [Fact]
    public void AddXUnit_WithNullBuilder_ThrowArgumentNullException()
    {
        // Arrange
        ILoggingBuilder builder = null!;

        // Act
        void Act() => builder.AddXUnit(_testOutputHelper);

        // Assert
        _ = Assert.Throws<ArgumentNullException>("builder", Act);
    }

    [Fact]
    public void AddXUnit_WithNullTestOutputHelper_ThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        ITestOutputHelper output = null!;

        // Act & Assert
        _ = Assert.Throws<ArgumentNullException>(
            "testOutputHelper",
            () =>
            {
                _ = services.AddLogging(builder =>
                {
                    _ = builder.AddXUnit(output);
                });
            }
        );
    }
}
