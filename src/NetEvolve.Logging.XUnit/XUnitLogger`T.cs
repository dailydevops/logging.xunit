namespace NetEvolve.Logging.XUnit;

using System;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;
using Xunit.Sdk;

/// <inheritdoc cref="XUnitLogger" />
public sealed class XUnitLogger<T> : XUnitLogger, ILogger<T>
    where T : notnull
{
    internal XUnitLogger(
        IMessageSink messageSink,
        TimeProvider timeProvider,
        IExternalScopeProvider? scopeProvider,
        IXUnitLoggerOptions? options
    )
        : base(
            messageSink is not null
                ? message => messageSink.OnMessage(new DiagnosticMessage(message))
                : throw new ArgumentNullException(nameof(messageSink)),
            timeProvider,
            scopeProvider,
            options
        ) { }

    internal XUnitLogger(
        ITestOutputHelper testOutputHelper,
        TimeProvider timeProvider,
        IExternalScopeProvider? scopeProvider,
        IXUnitLoggerOptions? options
    )
        : base(
            testOutputHelper is not null
                ? testOutputHelper.WriteLine
                : throw new ArgumentNullException(nameof(testOutputHelper)),
            timeProvider,
            scopeProvider,
            options
        ) { }

    internal XUnitLogger(
        Action<string> writeToAction,
        TimeProvider timeProvider,
        IExternalScopeProvider? scopeProvider,
        IXUnitLoggerOptions? options
    )
        : base(writeToAction, timeProvider, scopeProvider, options) { }
}
