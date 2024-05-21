namespace NetEvolve.Logging.XUnit;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using NetEvolve.Arguments;
using Xunit.Abstractions;

/// <summary>
/// Extensions for <see cref="ILoggingBuilder"/> to add a xunit logger.
/// </summary>
public static class XUnitLoggerExtensions
{
    /// <summary>
    /// Adds a xunit logger named `xunit` to the factory.
    /// </summary>
    public static ILoggingBuilder AddXUnit(
        this ILoggingBuilder builder,
        ITestOutputHelper testOutputHelper,
        XUnitLoggerOptions? options = null
    )
    {
        Argument.ThrowIfNull(builder);
        Argument.ThrowIfNull(testOutputHelper);

        var services = builder.Services.AddSingleton(testOutputHelper);
        services.TryAddSingleton(_ => TimeProvider.System);
        services.TryAddScoped<IExternalScopeProvider, LoggerExternalScopeProvider>();
        services.TryAddEnumerable(
            ServiceDescriptor.Singleton<ILoggerProvider, XUnitLoggerProvider>(
                sp => new XUnitLoggerProvider(
                    sp.GetRequiredService<ITestOutputHelper>(),
                    sp.GetRequiredService<TimeProvider>(),
                    sp.GetRequiredService<IExternalScopeProvider>(),
                    options ?? XUnitLoggerOptions.Default
                )
            )
        );

        return builder;
    }
}
