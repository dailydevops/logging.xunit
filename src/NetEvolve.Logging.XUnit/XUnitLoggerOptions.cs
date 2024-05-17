namespace NetEvolve.Logging.XUnit;

/// <summary>
/// Options for the <see cref="XUnitLogger"/>.
/// </summary>
public class XUnitLoggerOptions
{
    public static XUnitLoggerOptions Default { get; } =
        new XUnitLoggerOptions { DisableAdditionalInformation = true, DisableScopes = true };

    public static XUnitLoggerOptions DisableAllFeatures { get; } =
        new XUnitLoggerOptions
        {
            DisableAdditionalInformation = true,
            DisableCategory = true,
            DisableLogLevel = true,
            DisableScopes = true,
            DisableTimestamp = true
        };

    public static XUnitLoggerOptions EnableAllFeatures { get; } =
        new XUnitLoggerOptions
        {
            DisableAdditionalInformation = false,
            DisableCategory = false,
            DisableLogLevel = false,
            DisableScopes = false,
            DisableTimestamp = false
        };

    /// <summary>
    /// Disables the output of the additional information in the log output. Default <see langword="false"/>.
    /// </summary>
    public bool DisableAdditionalInformation { get; set; }

    /// <summary>
    /// Disable the category name in the log output. Default <see langword="false"/>.
    /// </summary>
    public bool DisableCategory { get; set; }

    /// <summary>
    /// Disable the log level in the log output. Default <see langword="false"/>.
    /// </summary>
    public bool DisableLogLevel { get; set; }

    /// <summary>
    /// Disable the scopes in the log output. Default <see langword="false"/>.
    /// </summary>
    public bool DisableScopes { get; set; }

    /// <summary>
    /// Disables the timestamps in the log output. Default <see langword="false"/>.
    /// </summary>
    public bool DisableTimestamp { get; set; }

    private string? _timestampFormat;

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
