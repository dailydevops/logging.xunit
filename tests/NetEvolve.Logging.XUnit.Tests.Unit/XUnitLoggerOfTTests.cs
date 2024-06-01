namespace NetEvolve.Logging.XUnit.Tests.Unit;

using System;
using Xunit;
using Xunit.Abstractions;

public partial class XUnitLoggerOfTTests
{
    [Fact]
    public void Constructor_WithMessageSinkNull_ThrowArgumentNullException()
    {
        IMessageSink messageSink = null!;

        _ = Assert.Throws<ArgumentNullException>(
            "messageSink",
            () => _ = new XUnitLogger<XUnitLoggerOfTTests>(messageSink, null!, null!, null!)
        );
    }

    [Fact]
    public void Constructor_WithTestOutputHelperNull_ThrowArgumentNullException()
    {
        ITestOutputHelper testOutputHelper = null!;

        _ = Assert.Throws<ArgumentNullException>(
            "testOutputHelper",
            () => _ = new XUnitLogger<XUnitLoggerOfTTests>(testOutputHelper, null!, null!, null!)
        );
    }

    [Fact]
    public void Constructor_WithWriteToActionNull_ThrowArgumentNullException()
    {
        Action<string> writeToAction = null!;

        _ = Assert.Throws<ArgumentNullException>(
            "writeToAction",
            () => _ = new XUnitLogger<XUnitLoggerOfTTests>(writeToAction, null!, null!, null!)
        );
    }
}
