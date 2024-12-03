namespace NetEvolve.Logging.XUnit;

using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using Microsoft.Extensions.Logging;
using NetEvolve.Arguments;
using NetEvolve.Logging.Abstractions;
using Xunit.Abstractions;
using Xunit.Sdk;

[ProviderAlias("XUnit")]
internal sealed class XUnitLoggerProvider
    : ILoggerProvider,
        ISupportExternalScope,
        IXUnitLoggerOptions
{
    private readonly Action<string?> _writeToAction;

    private readonly IExternalScopeProvider _scopeProvider = NullExternalScopeProvider.Instance;
    private readonly XUnitLoggerOptions _options;
    private readonly ConcurrentDictionary<string, XUnitLogger> _loggers;
    private readonly TimeProvider _timeProvider;

    internal ImmutableList<XUnitLogger> Loggers => ImmutableList.CreateRange(_loggers.Values);

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
        IMessageSink messageSink,
        TimeProvider timeProvider,
        IExternalScopeProvider? scopeProvider = null,
        XUnitLoggerOptions? options = null
    )
    {
        Argument.ThrowIfNull(messageSink);
        Argument.ThrowIfNull(timeProvider);

        _scopeProvider = scopeProvider ?? new LoggerExternalScopeProvider();
        _options = options ?? XUnitLoggerOptions.Default;

        _timeProvider = timeProvider;

        _loggers = new ConcurrentDictionary<string, XUnitLogger>(StringComparer.Ordinal);
        _writeToAction = message => messageSink.OnMessage(new DiagnosticMessage(message));
    }

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
        _writeToAction = testOutputHelper.WriteLine;
    }

    /// <inheritdoc cref="ILoggerProvider.CreateLogger(string)"/>
    public ILogger CreateLogger(string categoryName)
    {
        Argument.ThrowIfNullOrWhiteSpace(categoryName);

        return _loggers.GetOrAdd(
            $"{categoryName}_Default",
            name => new XUnitLogger(_writeToAction, _timeProvider, _scopeProvider, this)
        );
    }

    /// <inheritdoc cref="ILoggerProvider.CreateLogger(string)"/>
    public ILogger CreateLogger<T>()
        where T : notnull =>
        _loggers.GetOrAdd(
            $"{typeof(T).FullName}_Default",
            _ => new XUnitLogger<T>(_writeToAction, _timeProvider, _scopeProvider, this)
        );

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
