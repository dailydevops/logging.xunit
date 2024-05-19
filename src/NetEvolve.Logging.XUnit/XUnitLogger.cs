namespace NetEvolve.Logging.XUnit;

using System;
using System.Globalization;
using System.Text;
using Microsoft.Extensions.Logging;
using NetEvolve.Logging.Abstractions;
using Xunit.Abstractions;

public class XUnitLogger : ILogger, ISupportExternalScope
{
    private readonly string? _categoryName;
    private readonly XUnitLoggerOptions _options;
    private readonly Action<string>? _writeToLogger;

    private readonly List<LoggedMessage> _loggedMessages;

    private const int DefaultCapacity = 1024;

    [ThreadStatic]
    private static StringBuilder? _builder;

    public IExternalScopeProvider ScopeProvider { get; private set; }

    public IReadOnlyList<LoggedMessage> LoggedMessages => _loggedMessages.AsReadOnly();

    public static XUnitLogger CreateLogger(
        ITestOutputHelper testOutputHelper,
        IExternalScopeProvider? scopeProvider = null,
        string? categoryName = null,
        XUnitLoggerOptions? options = null
    ) => new XUnitLogger(testOutputHelper, scopeProvider, categoryName, options);

    public static XUnitLogger<T> CreateLogger<T>(
        ITestOutputHelper testOutputHelper,
        IExternalScopeProvider? scopeProvider = null,
        XUnitLoggerOptions? options = null
    ) => new XUnitLogger<T>(testOutputHelper, scopeProvider, options);

    internal XUnitLogger(
        ITestOutputHelper testOutputHelper,
        IExternalScopeProvider? scopeProvider,
        string? name,
        XUnitLoggerOptions? options
    )
    {
        ArgumentNullException.ThrowIfNull(testOutputHelper);
        ArgumentNullException.ThrowIfNull(name);

        _writeToLogger = testOutputHelper.WriteLine;
        ScopeProvider = scopeProvider ?? NullExternalScopeProvider.Instance;
        _categoryName = name;
        _options = options ?? XUnitLoggerOptions.Default;

        _loggedMessages = [];
    }

    /// <inheritdoc cref="ILogger.BeginScope{TState}(TState)"/>
    public IDisposable? BeginScope<TState>(TState state)
        where TState : notnull => ScopeProvider.Push(state);

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

        if (!IsEnabled(logLevel))
        {
            return;
        }

        var builder = _builder;
        _builder = null;
        builder ??= new StringBuilder(DefaultCapacity);

        var message = formatter(state, exception);
        var now = DateTimeOffset.Now;
        (builder, var scopes) = CreateMessage(logLevel, state, exception, builder, message, now);

        try
        {
            _loggedMessages.Add(
                new LoggedMessage(now, logLevel, eventId, message, exception, scopes)
            );
            _writeToLogger?.Invoke(builder.ToString());
        }
        catch
        {
            // Ignore exception.
            // Unfortunately, this can happen if the process is terminated before the end of the test.
        }

        _ = builder.Clear();
        if (builder.Capacity > DefaultCapacity)
        {
            builder.Capacity = DefaultCapacity;
        }
        _builder = builder;
    }

    private (StringBuilder, List<object?>) CreateMessage<TState>(
        LogLevel logLevel,
        TState state,
        Exception? exception,
        StringBuilder builder,
        string message,
        DateTimeOffset now
    )
    {
        var scopes = new List<object?>();
        if (!_options.DisableTimestamp)
        {
            _ = builder
                .Append(now.ToString(_options.TimestampFormat, CultureInfo.InvariantCulture))
                .Append(' ');
        }

        if (!_options.DisableLogLevel)
        {
            _ = builder.Append('[').Append(LogLevelToString(logLevel)).Append("] ");
        }

        if (!_options.DisableCategory)
        {
            _ = builder.Append('[').Append(_categoryName).Append("] ");
        }

        _ = builder.Append(message);

        if (exception is not null)
        {
            _ = builder.Append('\n').Append(exception);
        }

        if (
            !_options.DisableAdditionalInformation
            && state is IReadOnlyList<KeyValuePair<string, object?>> additionalInformation
        )
        {
            var level = 1;
            _ = builder.Append('\n').Append('\t').Append("Additional Information");
            foreach (var info in additionalInformation)
            {
                AddAdditionalInformation(builder, info, level);
            }
        }

        ScopeProvider.ForEachScope(
            (scope, state) =>
            {
                scopes.Add(scope);

                if (!_options.DisableScopes)
                {
                    _ = state.Append("\n=>\t").Append(scope);
                }
            },
            builder
        );

        return (builder, scopes);
    }

    private static void AddAdditionalInformation(
        StringBuilder builder,
        KeyValuePair<string, object?> info,
        int level
    )
    {
        _ = builder
            .Append('\n')
            .Append('\t', level)
            .Append(CultureInfo.InvariantCulture, $"`{info.Key}`:");

        if (info.Value is null)
        {
            _ = builder.Append(" `null`");
        }
        else if (info.Value is IConvertible convertible)
        {
            _ = builder.Append(CultureInfo.InvariantCulture, $" `{convertible.ToString()}`");
        }
        else if (info.Value is KeyValuePair<string, object?> kvp)
        {
            AddAdditionalInformation(builder, kvp, level + 1);
        }
        else if (info.Value is IEnumerable<KeyValuePair<string, object?>> enumerable)
        {
            foreach (var item in enumerable)
            {
                AddAdditionalInformation(builder, item, level + 1);
            }
        }
    }

    private static string LogLevelToString(LogLevel logLevel) =>
        logLevel switch
        {
            LogLevel.Trace => "TRCE",
            LogLevel.Debug => "DBUG",
            LogLevel.Information => "INFO",
            LogLevel.Warning => "WARN",
            LogLevel.Error => "FAIL",
            LogLevel.Critical => "CRIT",
            LogLevel.None => "NONE",
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel))
        };

    /// <inheritdoc/>
    public void SetScopeProvider(IExternalScopeProvider scopeProvider)
    {
        ArgumentNullException.ThrowIfNull(scopeProvider);

        ScopeProvider = scopeProvider;
    }
}
