namespace NetEvolve.Logging.XUnit;

using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using Microsoft.Extensions.Logging;
using NetEvolve.Arguments;
using NetEvolve.Logging.Abstractions;
using Xunit.Abstractions;

[ProviderAlias("XUnit")]
internal sealed class XUnitLoggerProvider
    : ILoggerProvider,
        ISupportExternalScope,
        IXUnitLoggerOptions
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly IExternalScopeProvider _scopeProvider = NullExternalScopeProvider.Instance;
    private readonly XUnitLoggerOptions _options;
    private readonly ConcurrentDictionary<string, XUnitLogger> _loggers;
    private readonly TimeProvider _timeProvider;

    internal ImmutableList<XUnitLogger> Loggers => _loggers.Values.ToImmutableList();

    /// <inheritdoc cref="IXUnitLoggerOptions.DisableAdditionalInformation"/>
    public bool DisableAdditionalInformation => _options.DisableAdditionalInformation;

    /// <inheritdoc cref="IXUnitLoggerOptions.DisableLogLevel"/>
    public bool DisableLogLevel => _options.DisableLogLevel;

    /// <inheritdoc cref="IXUnitLoggerOptions.DisableScopes"/>
    public bool DisableScopes => _options.DisableScopes;

    /// <inheritdoc cref="IXUnitLoggerOptions.DisableTimestamp"/>
    public bool DisableTimestamp => _options.DisableTimestamp;

    /// <inheritdoc cref="IXUnitLoggerOptions.TimestampFormat"/>
    public string TimestampFormat => _options.TimestampFormat;

    public XUnitLoggerProvider(
        ITestOutputHelper testOutputHelper,
        TimeProvider timeProvider,
        IExternalScopeProvider? scopeProvider = null,
        XUnitLoggerOptions? options = null
    )
    {
        Argument.ThrowIfNull(testOutputHelper);
        Argument.ThrowIfNull(timeProvider);

        _scopeProvider = scopeProvider ?? new LoggerExternalScopeProvider();
        _options = options ?? XUnitLoggerOptions.Default;

        _timeProvider = timeProvider;

        _loggers = new ConcurrentDictionary<string, XUnitLogger>(StringComparer.Ordinal);
        _testOutputHelper = testOutputHelper;
    }

    /// <inheritdoc cref="ILoggerProvider.CreateLogger(string)"/>
    public ILogger CreateLogger(string categoryName)
    {
        Argument.ThrowIfNullOrWhiteSpace(categoryName);

        return _loggers.GetOrAdd(
            $"{categoryName}_Default",
            name => XUnitLogger.CreateLogger(_testOutputHelper, _timeProvider, _scopeProvider, this)
        );
    }

    /// <inheritdoc cref="ILoggerProvider.CreateLogger(string)"/>
    public ILogger CreateLogger<T>()
        where T : notnull =>
        _loggers.GetOrAdd(
            $"{typeof(T).FullName}_Default",
            _ => XUnitLogger.CreateLogger<T>(_testOutputHelper, _timeProvider, _scopeProvider, this)
        );

    /// <inheritdoc cref="ILoggerProvider.CreateLogger(string)"/>
    public ILogger CreateLogger(string categoryName, IMessageSink messageSink)
    {
        Argument.ThrowIfNullOrWhiteSpace(categoryName);
        Argument.ThrowIfNull(messageSink);

        return _loggers.GetOrAdd(
            $"{categoryName}_IMessageSink",
            name => XUnitLogger.CreateLogger(messageSink, _timeProvider, _scopeProvider, this)
        );
    }

    /// <inheritdoc cref="ILoggerProvider.CreateLogger(string)"/>
    public ILogger CreateLogger<T>(IMessageSink messageSink)
        where T : notnull
    {
        Argument.ThrowIfNull(messageSink);

        return _loggers.GetOrAdd(
            $"{typeof(T).FullName}_IMessageSink",
            _ => XUnitLogger.CreateLogger<T>(messageSink, _timeProvider, _scopeProvider, this)
        );
    }

    /// <inheritdoc cref="ILoggerProvider.CreateLogger(string)"/>
    public ILogger CreateLogger(string categoryName, ITestOutputHelper testOutputHelper)
    {
        Argument.ThrowIfNullOrWhiteSpace(categoryName);
        Argument.ThrowIfNull(testOutputHelper);

        return _loggers.GetOrAdd(
            $"{categoryName}_ITestOutputHelper",
            name => XUnitLogger.CreateLogger(testOutputHelper, _timeProvider, _scopeProvider, this)
        );
    }

    /// <inheritdoc cref="ILoggerProvider.CreateLogger(string)"/>
    public ILogger CreateLogger<T>(ITestOutputHelper testOutputHelper)
        where T : notnull
    {
        Argument.ThrowIfNull(testOutputHelper);

        return _loggers.GetOrAdd(
            $"{typeof(T).FullName}_ITestOutputHelper",
            _ => XUnitLogger.CreateLogger<T>(testOutputHelper, _timeProvider, _scopeProvider, this)
        );
    }

    /// <inheritdoc cref="ISupportExternalScope.SetScopeProvider(IExternalScopeProvider)"/>
    public void SetScopeProvider(IExternalScopeProvider scopeProvider)
    {
        Argument.ThrowIfNull(scopeProvider);

        if (_scopeProvider == scopeProvider)
        {
            return;
        }

        foreach (var logger in _loggers.Values)
        {
            logger.SetScopeProvider(scopeProvider);
        }
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose() => _loggers.Clear();
}
