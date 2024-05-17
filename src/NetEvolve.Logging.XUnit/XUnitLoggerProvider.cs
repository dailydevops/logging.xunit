namespace NetEvolve.Logging.XUnit;

using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using NetEvolve.Arguments;
using Xunit.Abstractions;

internal sealed class XUnitLoggerProvider : ILoggerProvider, ISupportExternalScope
{
    private readonly ITestOutputHelper? _testOutputHelper;
    private readonly IMessageSink? _messageSink;
    private readonly IExternalScopeProvider _scopeProvider; // = NullExternalScopeProvider.Instance;
    private readonly XUnitLoggerOptions _options;
    private readonly ConcurrentDictionary<string, XUnitLogger> _loggers;

    public XUnitLoggerProvider(
        ITestOutputHelper testOutputHelper,
        IExternalScopeProvider? scopeProvider = null,
        XUnitLoggerOptions? options = null
    )
        : this(scopeProvider, options)
    {
        ArgumentNullException.ThrowIfNull(testOutputHelper);

        _testOutputHelper = testOutputHelper;
    }

    public XUnitLoggerProvider(
        IMessageSink messageSink,
        IExternalScopeProvider? scopeProvider = null,
        XUnitLoggerOptions? options = null
    )
        : this(scopeProvider, options)
    {
        ArgumentNullException.ThrowIfNull(messageSink);

        _messageSink = messageSink;
    }

    private XUnitLoggerProvider(
        IExternalScopeProvider? scopeProvider = null,
        XUnitLoggerOptions? options = null
    )
    {
        _scopeProvider = scopeProvider ?? new LoggerExternalScopeProvider();
        _options = options ?? new XUnitLoggerOptions();

        _loggers = new ConcurrentDictionary<string, XUnitLogger>(StringComparer.Ordinal);
    }

    /// <inheritdoc cref="ILoggerProvider.CreateLogger(string)"/>
    public ILogger CreateLogger(string categoryName)
    {
        Argument.ThrowIfNullOrWhiteSpace(categoryName);

        return _loggers.GetOrAdd(categoryName, CreateLoggerInternal);
    }

    /// <inheritdoc cref="ILoggerProvider.CreateLogger(string)"/>
    public ILogger CreateLogger<T>()
        where T : notnull => _loggers.GetOrAdd(typeof(T).FullName!, CreateLoggerInternal);

    private XUnitLogger CreateLoggerInternal(string name)
    {
        if (_testOutputHelper is not null)
        {
            return new XUnitLogger(_testOutputHelper, _scopeProvider, name, _options);
        }
        else if (_messageSink is not null)
        {
            return new XUnitLogger(_messageSink, _scopeProvider, name, _options);
        }

        throw new InvalidOperationException("No output destination was provided.");
    }

    /// <inheritdoc cref="ISupportExternalScope.SetScopeProvider(IExternalScopeProvider)"/>
    public void SetScopeProvider(IExternalScopeProvider scopeProvider)
    {
        ArgumentNullException.ThrowIfNull(scopeProvider);

        if (_scopeProvider == scopeProvider)
        {
            return;
        }

        foreach (var logger in _loggers.Values)
        {
            logger.ScopeProvider = scopeProvider;
        }
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose() { }
}
