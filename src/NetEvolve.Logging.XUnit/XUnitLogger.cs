﻿namespace NetEvolve.Logging.XUnit;

using System;
using System.Globalization;
using System.Text;
using Microsoft.Extensions.Logging;
using NetEvolve.Arguments;
using NetEvolve.Logging.Abstractions;
using Xunit.Abstractions;
using Xunit.Sdk;

/// <summary>
/// Represents a logger that writes messages to xunit output.
/// </summary>
public class XUnitLogger : ILogger, ISupportExternalScope
{
    private readonly IXUnitLoggerOptions _options;
    private readonly TimeProvider _timeProvider;

    private readonly List<LoggedMessage> _loggedMessages;

    private readonly Action<string> _writeToLog;

    private const int DefaultCapacity = 1024;

    [ThreadStatic]
    private static StringBuilder? _builder;

    /// <summary>
    /// Gets the external scope provider.
    /// </summary>
    public IExternalScopeProvider ScopeProvider { get; private set; }

    /// <inheritdoc cref="IHasLoggedMessages.LoggedMessages"/>
    public IReadOnlyList<LoggedMessage> LoggedMessages => _loggedMessages.AsReadOnly();

    /// <summary>
    /// Creates a new instance of <see cref="XUnitLogger"/>.
    /// </summary>
    /// <param name="messageSink">The <see cref="IMessageSink" /> to write the log messages to.</param>
    /// <param name="scopeProvider">The <see cref="IExternalScopeProvider" /> to use to get the current scope.</param>
    /// <param name="options">The options to control the behavior of the logger.</param>
    /// <returns>A cached or new instance of <see cref="XUnitLogger"/>.</returns>
    public static XUnitLogger CreateLogger(
        IMessageSink messageSink,
        IExternalScopeProvider? scopeProvider = null,
        IXUnitLoggerOptions? options = null
    ) => CreateLogger(messageSink, TimeProvider.System, scopeProvider, options);

    /// <summary>
    /// Creates a new instance of <see cref="XUnitLogger"/>.
    /// </summary>
    /// <param name="messageSink">The <see cref="IMessageSink" /> to write the log messages to.</param>
    /// <param name="timeProvider">The <see cref="TimeProvider" /> to use to get the current time.</param>
    /// <param name="scopeProvider">The <see cref="IExternalScopeProvider" /> to use to get the current scope.</param>
    /// <param name="options">The options to control the behavior of the logger.</param>
    /// <returns>A cached or new instance of <see cref="XUnitLogger"/>.</returns>
    public static XUnitLogger CreateLogger(
        IMessageSink messageSink,
        TimeProvider timeProvider,
        IExternalScopeProvider? scopeProvider = null,
        IXUnitLoggerOptions? options = null
    )
    {
        Argument.ThrowIfNull(messageSink);
        Argument.ThrowIfNull(timeProvider);

        return new XUnitLogger(
            message => _ = messageSink.OnMessage(new DiagnosticMessage(message)),
            timeProvider,
            scopeProvider,
            options
        );
    }

    /// <summary>
    /// Creates a new instance of <see cref="XUnitLogger{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type who's fullname is used as the category name for messages produced by the logger.</typeparam>
    /// <param name="messageSink">The <see cref="IMessageSink" /> to write the log messages to.</param>
    /// <param name="scopeProvider">The <see cref="IExternalScopeProvider" /> to use to get the current scope.</param>
    /// <param name="options">The options to control the behavior of the logger.</param>
    /// <returns>A cached or new instance of <see cref="XUnitLogger"/>.</returns>
    public static XUnitLogger<T> CreateLogger<T>(
        IMessageSink messageSink,
        IExternalScopeProvider? scopeProvider = null,
        IXUnitLoggerOptions? options = null
    )
        where T : notnull => CreateLogger<T>(messageSink, TimeProvider.System, scopeProvider, options);

    /// <summary>
    /// Creates a new instance of <see cref="XUnitLogger{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type who's fullname is used as the category name for messages produced by the logger.</typeparam>
    /// <param name="messageSink">The <see cref="IMessageSink" /> to write the log messages to.</param>
    /// <param name="timeProvider">The <see cref="TimeProvider" /> to use to get the current time.</param>
    /// <param name="scopeProvider">The <see cref="IExternalScopeProvider" /> to use to get the current scope.</param>
    /// <param name="options">The options to control the behavior of the logger.</param>
    /// <returns>A cached or new instance of <see cref="XUnitLogger"/>.</returns>
    public static XUnitLogger<T> CreateLogger<T>(
        IMessageSink messageSink,
        TimeProvider timeProvider,
        IExternalScopeProvider? scopeProvider = null,
        IXUnitLoggerOptions? options = null
    )
        where T : notnull
    {
        Argument.ThrowIfNull(messageSink);
        Argument.ThrowIfNull(timeProvider);

        return new XUnitLogger<T>(messageSink, timeProvider, scopeProvider, options);
    }

    /// <summary>
    /// Creates a new instance of <see cref="XUnitLogger"/>.
    /// </summary>
    /// <param name="testOutputHelper">The <see cref="ITestOutputHelper" /> to write the log messages to.</param>
    /// <param name="scopeProvider">The <see cref="IExternalScopeProvider" /> to use to get the current scope.</param>
    /// <param name="options">The options to control the behavior of the logger.</param>
    /// <returns>A cached or new instance of <see cref="XUnitLogger"/>.</returns>
    public static XUnitLogger CreateLogger(
        ITestOutputHelper testOutputHelper,
        IExternalScopeProvider? scopeProvider = null,
        IXUnitLoggerOptions? options = null
    ) => CreateLogger(testOutputHelper, TimeProvider.System, scopeProvider, options);

    /// <summary>
    /// Creates a new instance of <see cref="XUnitLogger"/>.
    /// </summary>
    /// <param name="testOutputHelper">The <see cref="ITestOutputHelper" /> to write the log messages to.</param>
    /// <param name="timeProvider">The <see cref="TimeProvider" /> to use to get the current time.</param>
    /// <param name="scopeProvider">The <see cref="IExternalScopeProvider" /> to use to get the current scope.</param>
    /// <param name="options">The options to control the behavior of the logger.</param>
    /// <returns>A cached or new instance of <see cref="XUnitLogger"/>.</returns>
    public static XUnitLogger CreateLogger(
        ITestOutputHelper testOutputHelper,
        TimeProvider timeProvider,
        IExternalScopeProvider? scopeProvider = null,
        IXUnitLoggerOptions? options = null
    )
    {
        Argument.ThrowIfNull(testOutputHelper);
        Argument.ThrowIfNull(timeProvider);

        return new XUnitLogger(testOutputHelper.WriteLine, timeProvider, scopeProvider, options);
    }

    /// <summary>
    /// Creates a new instance of <see cref="XUnitLogger{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type who's fullname is used as the category name for messages produced by the logger.</typeparam>
    /// <param name="testOutputHelper">The <see cref="ITestOutputHelper" /> to write the log messages to.</param>
    /// <param name="scopeProvider">The <see cref="IExternalScopeProvider" /> to use to get the current scope.</param>
    /// <param name="options">The options to control the behavior of the logger.</param>
    /// <returns>A cached or new instance of <see cref="XUnitLogger"/>.</returns>
    public static XUnitLogger<T> CreateLogger<T>(
        ITestOutputHelper testOutputHelper,
        IExternalScopeProvider? scopeProvider = null,
        IXUnitLoggerOptions? options = null
    )
        where T : notnull => CreateLogger<T>(testOutputHelper, TimeProvider.System, scopeProvider, options);

    /// <summary>
    /// Creates a new instance of <see cref="XUnitLogger{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type who's fullname is used as the category name for messages produced by the logger.</typeparam>
    /// <param name="testOutputHelper">The <see cref="ITestOutputHelper" /> to write the log messages to.</param>
    /// <param name="timeProvider">The <see cref="TimeProvider" /> to use to get the current time.</param>
    /// <param name="scopeProvider">The <see cref="IExternalScopeProvider" /> to use to get the current scope.</param>
    /// <param name="options">The options to control the behavior of the logger.</param>
    /// <returns>A cached or new instance of <see cref="XUnitLogger"/>.</returns>
    public static XUnitLogger<T> CreateLogger<T>(
        ITestOutputHelper testOutputHelper,
        TimeProvider timeProvider,
        IExternalScopeProvider? scopeProvider = null,
        IXUnitLoggerOptions? options = null
    )
        where T : notnull
    {
        Argument.ThrowIfNull(testOutputHelper);
        Argument.ThrowIfNull(timeProvider);

        return new XUnitLogger<T>(testOutputHelper, timeProvider, scopeProvider, options);
    }

    internal XUnitLogger(
        Action<string> writeToAction,
        TimeProvider timeProvider,
        IExternalScopeProvider? scopeProvider,
        IXUnitLoggerOptions? options
    )
    {
        Argument.ThrowIfNull(writeToAction);
        Argument.ThrowIfNull(timeProvider);

        ScopeProvider = scopeProvider ?? NullExternalScopeProvider.Instance;
        _timeProvider = timeProvider;
        _options = options ?? XUnitLoggerOptions.Default;

        _loggedMessages = [];

        _writeToLog = writeToAction;
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
        Argument.ThrowIfNull(formatter);

        if (!IsEnabled(logLevel))
        {
            return;
        }

        try
        {
            var message = formatter(state, exception);
            var now = _timeProvider.GetLocalNow();
            var (fullMessage, scopes) = CreateMessage(logLevel, state, exception, message, now);

            _loggedMessages.Add(new LoggedMessage(now, logLevel, eventId, message, exception, scopes));

            _writeToLog.Invoke(fullMessage);
        }
        catch
        {
            // Ignore exception.
            // Unfortunately, this can happen if the process is terminated before the end of the test.
        }
    }

    private (string, List<object?>) CreateMessage<TState>(
        LogLevel logLevel,
        TState state,
        Exception? exception,
        string message,
        DateTimeOffset now
    )
    {
        var scopes = new List<object?>();
        var builder = _builder;
        _builder = null;
        builder ??= new StringBuilder(DefaultCapacity);

        try
        {
            if (!_options.DisableTimestamp)
            {
                _ = builder.Append(now.ToString(_options.TimestampFormat, CultureInfo.InvariantCulture)).Append(' ');
            }

            if (!_options.DisableLogLevel)
            {
                _ = builder.Append('[').Append(LogLevelToString(logLevel)).Append("] ");
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
                _ = builder.Append('\n').Append('\t').Append("Additional Information");
                foreach (var info in additionalInformation)
                {
                    AddAdditionalInformation(builder, info);
                }
            }

            ScopeProvider.ForEachScope(IterateScopes, builder);

            return (builder.ToString(), scopes);
        }
        finally
        {
            _ = builder.Clear();
            if (builder.Capacity > DefaultCapacity)
            {
                builder.Capacity = DefaultCapacity;
            }
            _builder = builder;
        }

        void IterateScopes(object? scope, StringBuilder state)
        {
            if (scope is null)
            {
                return;
            }

            scopes.Add(scope);

            if (!_options.DisableScopes)
            {
                PrintScope(scope, state);
            }
        }
    }

    private static void PrintScope(object? scope, StringBuilder state)
    {
        if (scope is IEnumerable<KeyValuePair<string, object?>> scopeList)
        {
            foreach (var subScope in scopeList)
            {
                PrintScope(subScope, state);
            }

            return;
        }

        _ = state.Append('\n').Append(' ', 4).Append("=>").Append(' ');

        _ = scope is KeyValuePair<string, object> info
            ? state.Append(info.Key).Append(": ").Append(info.Value)
            : state.Append(scope);
    }

    private static void AddAdditionalInformation(StringBuilder builder, KeyValuePair<string, object?> info) =>
        _ = builder.Append('\n').Append(' ', 4).Append(info.Key).Append(": ").Append(info.Value);

#pragma warning disable IDE0072 // Add missing cases
    internal static string LogLevelToString(LogLevel logLevel) =>
        logLevel switch
        {
            LogLevel.Trace => "TRCE",
            LogLevel.Debug => "DBUG",
            LogLevel.Information => "INFO",
            LogLevel.Warning => "WARN",
            LogLevel.Error => "FAIL",
            LogLevel.Critical => "CRIT",
            _ => "NONE",
        };
#pragma warning restore IDE0072 // Add missing cases

    /// <inheritdoc/>
    public void SetScopeProvider(IExternalScopeProvider scopeProvider)
    {
        Argument.ThrowIfNull(scopeProvider);

        ScopeProvider = scopeProvider;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        var builder = _builder;
        _builder = null;
        builder ??= new StringBuilder(DefaultCapacity);

        try
        {
            foreach (var lmsg in LoggedMessages)
            {
                if (!_options.DisableTimestamp)
                {
                    _ = builder
                        .Append(lmsg.Timestamp.ToString(_options.TimestampFormat, CultureInfo.InvariantCulture))
                        .Append(' ');
                }

                if (!_options.DisableLogLevel)
                {
                    _ = builder.Append('[').Append(LogLevelToString(lmsg.LogLevel)).Append("] ");
                }

                _ = builder.Append(lmsg.Message);
                _ = builder.AppendLine();

                if (lmsg.Exception is not null)
                {
                    _ = builder.Append(lmsg.Exception).AppendLine();
                }
            }

            return builder.ToString().Trim();
        }
        finally
        {
            _ = builder.Clear();
            if (builder.Capacity > DefaultCapacity)
            {
                builder.Capacity = DefaultCapacity;
            }
            _builder = builder;
        }
    }
}
