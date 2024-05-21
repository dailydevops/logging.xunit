namespace NetEvolve.Logging.XUnit;

using System;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

/// <inheritdoc cref="XUnitLogger" />
public sealed class XUnitLogger<T> : XUnitLogger, ILogger<T>
    where T : notnull
{
    internal XUnitLogger(
        ITestOutputHelper testOutputHelper,
        TimeProvider timeProvider,
        IExternalScopeProvider? scopeProvider,
        IXUnitLoggerOptions? options
    )
        : base(testOutputHelper, timeProvider, scopeProvider, options) { }
}
