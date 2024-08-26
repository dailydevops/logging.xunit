namespace NetEvolve.Logging.XUnit;

/// <summary>
/// Options for the <see cref="XUnitLogger"/>.
/// </summary>
public class XUnitLoggerOptions : IXUnitLoggerOptions
{
    /// <summary>
    /// Default options, which disables the category name, additional information and scopes in the log output.
    /// </summary>
    public static XUnitLoggerOptions Default { get; } =
        new XUnitLoggerOptions { DisableAdditionalInformation = true, DisableScopes = true };

    /// <summary>
    /// Disables all features in the log output.
    /// </summary>
    public static XUnitLoggerOptions DisableAllFeatures { get; } =
        new XUnitLoggerOptions
        {
            DisableAdditionalInformation = true,
            DisableLogLevel = true,
            DisableScopes = true,
            DisableTimestamp = true,
        };

    /// <summary>
    /// Enables all features in the log output.
    /// </summary>
    public static XUnitLoggerOptions EnableAllFeatures { get; } =
        new XUnitLoggerOptions
        {
            DisableAdditionalInformation = false,
            DisableLogLevel = false,
            DisableScopes = false,
            DisableTimestamp = false,
        };

    /// <summary>
    /// Minimal options, which disables the category name, additional information, scopes and timestamp in the log output.
    /// </summary>
    public static XUnitLoggerOptions Minimal { get; } =
        new XUnitLoggerOptions
        {
            DisableAdditionalInformation = true,
            DisableScopes = true,
            DisableTimestamp = true,
        };

    /// <inheritdoc cref="IXUnitLoggerOptions.DisableAdditionalInformation"/>
    public bool DisableAdditionalInformation { get; set; }

    /// <inheritdoc cref="IXUnitLoggerOptions.DisableLogLevel"/>
    public bool DisableLogLevel { get; set; }

    /// <inheritdoc cref="IXUnitLoggerOptions.DisableScopes"/>
    public bool DisableScopes { get; set; }

    /// <inheritdoc cref="IXUnitLoggerOptions.DisableTimestamp"/>
    public bool DisableTimestamp { get; set; }

    private string? _timestampFormat;

    /// <inheritdoc cref="IXUnitLoggerOptions.TimestampFormat"/>
#if NET7_0_OR_GREATER
    [System.Diagnostics.CodeAnalysis.StringSyntax(
        System.Diagnostics.CodeAnalysis.StringSyntaxAttribute.DateTimeFormat
    )]
#endif
    public string TimestampFormat
    {
        get
        {
            if (string.IsNullOrWhiteSpace(_timestampFormat))
            {
                return "o";
            }
            return _timestampFormat;
        }
        set => _timestampFormat = value;
    }
}
