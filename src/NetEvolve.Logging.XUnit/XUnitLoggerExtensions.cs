namespace NetEvolve.Logging.XUnit;

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Xunit.Abstractions;

public static class XUnitLoggerExtensions
{
    public static ILoggingBuilder AddXUnit(
        this ILoggingBuilder builder,
        ITestOutputHelper testOutputHelper
    )
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(testOutputHelper);

        builder.AddConfiguration();

        _ = builder.Services.AddSingleton(testOutputHelper);
        builder.Services.TryAddEnumerable(
            ServiceDescriptor.Singleton<ILoggerProvider, XUnitLoggerProvider>()
        );

        return builder;
    }
}
