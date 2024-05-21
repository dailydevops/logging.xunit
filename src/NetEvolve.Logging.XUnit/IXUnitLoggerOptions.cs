namespace NetEvolve.Logging.XUnit;

/// <summary>
/// Accessor for the options of the <see cref="XUnitLogger"/>.
/// </summary>
public interface IXUnitLoggerOptions
{
    /// <summary>
    /// Disables the output of the additional information in the log output. Default <see langword="false"/>.
    /// </summary>
    bool DisableAdditionalInformation { get; }

    /// <summary>
    /// Disable the log level in the log output. Default <see langword="false"/>.
    /// </summary>
    bool DisableLogLevel { get; }

    /// <summary>
    /// Disable the scopes in the log output. Default <see langword="false"/>.
    /// </summary>
    bool DisableScopes { get; }

    /// <summary>
    /// Disables the timestamps in the log output. Default <see langword="false"/>.
    /// </summary>
    bool DisableTimestamp { get; }

    /// <summary>
    /// The format of the timestamp in the log output. Default <c>"o"</c>.
    /// </summary>
    string TimestampFormat { get; }
}
