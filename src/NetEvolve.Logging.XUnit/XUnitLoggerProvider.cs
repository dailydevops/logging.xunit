namespace NetEvolve.Logging.XUnit;

using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using Microsoft.Extensions.Logging;
using NetEvolve.Arguments;
using NetEvolve.Logging.Abstractions;
using Xunit.Abstractions;

internal sealed class XUnitLoggerProvider : ILoggerProvider, ISupportExternalScope
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly IExternalScopeProvider _scopeProvider = NullExternalScopeProvider.Instance;
    private readonly XUnitLoggerOptions _options;
    private readonly ConcurrentDictionary<string, XUnitLogger> _loggers;

    internal ImmutableList<XUnitLogger> Loggers => _loggers.Values.ToImmutableList();

    public XUnitLoggerProvider(
        ITestOutputHelper testOutputHelper,
        IExternalScopeProvider? scopeProvider = null,
        XUnitLoggerOptions? options = null
    )
    {
        ArgumentNullException.ThrowIfNull(testOutputHelper);

        _scopeProvider = scopeProvider ?? new LoggerExternalScopeProvider();
        _options = options ?? new XUnitLoggerOptions();

        _loggers = new ConcurrentDictionary<string, XUnitLogger>(StringComparer.Ordinal);
        _testOutputHelper = testOutputHelper;
    }

    /// <inheritdoc cref="ILoggerProvider.CreateLogger(string)"/>
    public ILogger CreateLogger(string categoryName)
    {
        Argument.ThrowIfNullOrWhiteSpace(categoryName);

        return _loggers.GetOrAdd(
            categoryName,
            name => new XUnitLogger(_testOutputHelper, _scopeProvider, name, _options)
        );
    }

    /// <inheritdoc cref="ILoggerProvider.CreateLogger(string)"/>
    public ILogger CreateLogger<T>()
        where T : notnull =>
        _loggers.GetOrAdd(
            typeof(T).FullName!,
            _ => new XUnitLogger<T>(_testOutputHelper, _scopeProvider, _options)
        );

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
            logger.SetScopeProvider(scopeProvider);
        }
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose() { }
}
