namespace NetEvolve.Logging.XUnit;

using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

public sealed class XUnitLogger<T> : XUnitLogger, ILogger<T>
{
    internal XUnitLogger(
        ITestOutputHelper testOutputHelper,
        IExternalScopeProvider? scopeProvider,
        XUnitLoggerOptions? options
    )
        : base(testOutputHelper, scopeProvider, typeof(T).FullName, options) { }

    internal XUnitLogger(
        IMessageSink messageSink,
        IExternalScopeProvider? scopeProvider,
        XUnitLoggerOptions? options
    )
        : base(messageSink, scopeProvider, typeof(T).FullName, options) { }
}
