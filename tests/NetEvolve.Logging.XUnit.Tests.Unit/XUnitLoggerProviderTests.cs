namespace NetEvolve.Logging.XUnit.Tests.Unit;

using System;
using Microsoft.Extensions.Logging;
using NetEvolve.Logging.Abstractions;
using Xunit;
using Xunit.Abstractions;

public class XUnitLoggerProviderTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public XUnitLoggerProviderTests(ITestOutputHelper testOutputHelper) => _testOutputHelper = testOutputHelper;

    [Fact]
    public void CreateLogger_WithTestOutputHelper()
    {
        // Arrange
        using var provider = new XUnitLoggerProvider(_testOutputHelper, TimeProvider.System);

        // Act
        var logger = provider.CreateLogger(nameof(XUnitLoggerProviderTests));

        // Assert
        Assert.NotNull(logger);
        _ = Assert.IsType<XUnitLogger>(logger);
    }

    [Fact]
    public void CreateLoggerGeneric_WithTestOutputHelper()
    {
        // Arrange
        using var provider = new XUnitLoggerProvider(_testOutputHelper, TimeProvider.System);

        // Act
        var logger = provider.CreateLogger<XUnitLoggerProviderTests>();

        // Assert
        Assert.NotNull(logger);
        _ = Assert.IsType<XUnitLogger<XUnitLoggerProviderTests>>(logger);
    }

    [Fact]
    public void SetScopeProvider_Null_ThrowArgumentNullException()
    {
        // Arrange
        using var provider = new XUnitLoggerProvider(_testOutputHelper, TimeProvider.System);

        // Act
        void Act() => provider.SetScopeProvider(null!);

        // Assert
        _ = Assert.Throws<ArgumentNullException>("scopeProvider", Act);
    }

    [Fact]
    public void SetScopeProvider_WithNullScopeProvider_NoExceptionThrown()
    {
        // Arrange
        using var provider = new XUnitLoggerProvider(_testOutputHelper, TimeProvider.System);

        _ = provider.CreateLogger(nameof(XUnitLoggerProviderTests));
        _ = provider.CreateLogger<XUnitLoggerProviderTests>();

        // Act
        var ex = Record.Exception(() => provider.SetScopeProvider(NullExternalScopeProvider.Instance));

        // Assert
        Assert.Null(ex);
    }

    [Fact]
    public void SetScopeProvider_WithScopeProvider_Expected()
    {
        // Arrange
        using var provider = new XUnitLoggerProvider(_testOutputHelper, TimeProvider.System);
        var scopeProvider = new LoggerExternalScopeProvider();

        _ = provider.CreateLogger(nameof(XUnitLoggerProviderTests));
        _ = provider.CreateLogger<XUnitLoggerProviderTests>();

        // Act
        provider.SetScopeProvider(scopeProvider);

        // Assert
        foreach (var logger in provider.Loggers)
        {
            Assert.Same(scopeProvider, logger.ScopeProvider);
        }
    }
}
