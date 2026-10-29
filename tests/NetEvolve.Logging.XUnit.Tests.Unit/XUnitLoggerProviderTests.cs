namespace NetEvolve.Logging.XUnit.Tests.Unit;

using System;
using Microsoft.Extensions.Logging;
using NetEvolve.Logging.Abstractions;
using Xunit;
using Xunit.Sdk;
using Xunit.v3;

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

    [Fact]
    public void Ctor_MessageSink_Null_ThrowArgumentNullException() =>
        _ = Assert.Throws<ArgumentNullException>(
            "messageSink",
            () => new XUnitLoggerProvider((IMessageSink)null!, TimeProvider.System)
        );

    [Fact]
    public void Ctor_TestOutputHelper_Null_ThrowArgumentNullException() =>
        _ = Assert.Throws<ArgumentNullException>(
            "testOutputHelper",
            () => new XUnitLoggerProvider((ITestOutputHelper)null!, TimeProvider.System)
        );

    [Fact]
    public void Ctor_MessageSink_TimeProviderNull_ThrowArgumentNullException() =>
        _ = Assert.Throws<ArgumentNullException>(
            "timeProvider",
            () => new XUnitLoggerProvider(new TestMessageSink(), null!)
        );

    [Fact]
    public void Ctor_TestOutputHelper_TimeProviderNull_ThrowArgumentNullException() =>
        _ = Assert.Throws<ArgumentNullException>(
            "timeProvider",
            () => new XUnitLoggerProvider(_testOutputHelper, null!)
        );

    [Fact]
    public void CreateLogger_CategoryNull_ThrowArgumentNullException()
    {
        using var provider = new XUnitLoggerProvider(_testOutputHelper, TimeProvider.System);

        _ = Assert.Throws<ArgumentNullException>("categoryName", () => provider.CreateLogger(null!));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void CreateLogger_CategoryEmptyOrWhitespace_ThrowArgumentException(string value)
    {
        using var provider = new XUnitLoggerProvider(_testOutputHelper, TimeProvider.System);

        _ = Assert.Throws<ArgumentException>("categoryName", () => provider.CreateLogger(value));
    }

    [Fact]
    public void CreateLogger_SameCategory_ReturnsSameInstance()
    {
        using var provider = new XUnitLoggerProvider(_testOutputHelper, TimeProvider.System);

        var first = provider.CreateLogger("A");
        var second = provider.CreateLogger("A");
        var other = provider.CreateLogger("B");

        Assert.Same(first, second);
        Assert.NotSame(first, other);
        Assert.Equal(2, provider.Loggers.Count);
    }

    [Fact]
    public void CreateLoggerGeneric_SameType_ReturnsSameInstance()
    {
        using var provider = new XUnitLoggerProvider(_testOutputHelper, TimeProvider.System);

        var first = provider.CreateLogger<XUnitLoggerProviderTests>();
        var second = provider.CreateLogger<XUnitLoggerProviderTests>();
        var other = provider.CreateLogger<XUnitLoggerProvider>();

        Assert.Same(first, second);
        Assert.NotSame(first, other);
        Assert.Equal(2, provider.Loggers.Count);
    }

    [Fact]
    public void Ctor_OptionsNull_UsesDefaultOptions()
    {
        using var provider = new XUnitLoggerProvider(_testOutputHelper, TimeProvider.System, null, null);

        Assert.True(provider.DisableAdditionalInformation);
        Assert.True(provider.DisableScopes);
    }

    [Fact]
    public void Ctor_MessageSink_OptionsNull_UsesDefaultOptions()
    {
        using var provider = new XUnitLoggerProvider(new TestMessageSink(), TimeProvider.System, null, null);

        Assert.True(provider.DisableAdditionalInformation);
        Assert.True(provider.DisableScopes);
    }

    [Fact]
    public void Ctor_CustomOptions_AreUsed()
    {
        using var provider = new XUnitLoggerProvider(
            _testOutputHelper,
            TimeProvider.System,
            null,
            XUnitLoggerOptions.EnableAllFeatures
        );
        using var sinkProvider = new XUnitLoggerProvider(
            new TestMessageSink(),
            TimeProvider.System,
            null,
            XUnitLoggerOptions.EnableAllFeatures
        );

        Assert.False(provider.DisableAdditionalInformation);
        Assert.False(provider.DisableScopes);
        Assert.False(sinkProvider.DisableAdditionalInformation);
        Assert.False(sinkProvider.DisableScopes);
    }

    [Fact]
    public void Ctor_ScopeProviderNull_CreatesLoggerExternalScopeProvider()
    {
        using var provider = new XUnitLoggerProvider(_testOutputHelper, TimeProvider.System);
        using var sinkProvider = new XUnitLoggerProvider(new TestMessageSink(), TimeProvider.System);

        _ = Assert.IsType<LoggerExternalScopeProvider>(((XUnitLogger)provider.CreateLogger("x")).ScopeProvider);
        _ = Assert.IsType<LoggerExternalScopeProvider>(((XUnitLogger)sinkProvider.CreateLogger("x")).ScopeProvider);
    }

    [Fact]
    public void Ctor_ScopeProvider_IsPassedToLoggers()
    {
        var scopeProvider = new LoggerExternalScopeProvider();
        using var provider = new XUnitLoggerProvider(_testOutputHelper, TimeProvider.System, scopeProvider);
        using var sinkProvider = new XUnitLoggerProvider(new TestMessageSink(), TimeProvider.System, scopeProvider);

        Assert.Same(scopeProvider, ((XUnitLogger)provider.CreateLogger("x")).ScopeProvider);
        Assert.Same(scopeProvider, ((XUnitLogger)sinkProvider.CreateLogger("x")).ScopeProvider);
    }

    private sealed class TestMessageSink : IMessageSink
    {
        public bool OnMessage(IMessageSinkMessage message) => true;
    }
}
