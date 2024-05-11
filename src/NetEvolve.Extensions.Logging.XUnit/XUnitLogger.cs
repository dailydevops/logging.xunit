namespace NetEvolve.Extensions.Logging.XUnit;

using System;
using System.Text;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

public class XUnitLogger : ILogger
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly IExternalScopeProvider _scopeProvider;
    private readonly string? _categoryName;
    private readonly XUnitLoggerOptions? _options;

    public static ILogger CreateLogger(
        ITestOutputHelper testOutputHelper,
        IExternalScopeProvider? scopeProvider = null,
        string? categoryName = null,
        XUnitLoggerOptions? options = null
    ) => new XUnitLogger(testOutputHelper, scopeProvider, categoryName, options);

    public static ILogger<T> CreateLogger<T>(
        ITestOutputHelper testOutputHelper,
        IExternalScopeProvider? scopeProvider = null,
        string? categoryName = null,
        XUnitLoggerOptions? options = null
    ) => new XUnitLogger<T>(testOutputHelper, scopeProvider, categoryName, options);

    internal XUnitLogger(
        ITestOutputHelper testOutputHelper,
        IExternalScopeProvider? scopeProvider,
        string? categoryName,
        XUnitLoggerOptions? options
    )
    {
        ArgumentNullException.ThrowIfNull(testOutputHelper);

        _testOutputHelper = testOutputHelper;
        _scopeProvider = scopeProvider ?? new LoggerExternalScopeProvider();
        _categoryName = categoryName;
        _options = options ?? new XUnitLoggerOptions();
    }

    /// <inheritdoc cref="ILogger.BeginScope{TState}(TState)"/>
    public IDisposable? BeginScope<TState>(TState state)
        where TState : notnull => _scopeProvider.Push(state);

    /// <inheritdoc cref="ILogger.IsEnabled(LogLevel)"/>
    public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

    /// <inheritdoc cref="ILogger.Log{TState}(LogLevel, EventId, TState, Exception?, Func{TState, Exception?, string})"/>
    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter
    )
    {
        ArgumentNullException.ThrowIfNull(formatter);

        var sb = new StringBuilder(500);

        _ = sb.Append('[').Append(LogLevelToString(logLevel)).Append("] ");

        _ = sb.Append(formatter(state, exception));

        try
        {
            _testOutputHelper.WriteLine(sb.ToString());
        }
        catch
        {
            // Unfortunately, this can happen if the process is terminated before the end of the test.
        }
    }

    private static string LogLevelToString(LogLevel logLevel) => logLevel switch
    {
        LogLevel.Trace => "trce",
        LogLevel.Debug => "dbug",
        LogLevel.Information => "info",
        LogLevel.Warning => "warn",
        LogLevel.Error => "fail",
        LogLevel.Critical => "crit",
        LogLevel.None => "none",
        _ => throw new ArgumentOutOfRangeException(nameof(logLevel))
    };
}
