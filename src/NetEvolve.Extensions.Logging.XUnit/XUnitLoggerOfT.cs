namespace NetEvolve.Extensions.Logging.XUnit;

using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

public sealed class XUnitLogger<T> : XUnitLogger, ILogger<T>
{
    internal XUnitLogger(
        ITestOutputHelper testOutputHelper,
        IExternalScopeProvider? scopeProvider,
        string? categoryName,
        XUnitLoggerOptions? options
    )
        : base(testOutputHelper, scopeProvider, categoryName, options) { }
}
