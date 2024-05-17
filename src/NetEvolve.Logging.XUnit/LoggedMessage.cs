namespace NetEvolve.Logging.XUnit;

using System;
using Microsoft.Extensions.Logging;

/// <summary>
/// Represents a logged message, including the timestamp, log level, event ID, message, exception, and scopes.
/// </summary>
/// <param name="Timestamp">Timestamp of the logged message.</param>
/// <param name="LogLevel">LogLevel of the logged message.</param>
/// <param name="EventId">EventId of the logged message.</param>
/// <param name="Message">Logged message.</param>
/// <param name="Exception">Logged exception. (optional)</param>
/// <param name="Scopes">Logged scopes.</param>
public record struct LoggedMessage(
    DateTimeOffset Timestamp,
    LogLevel LogLevel,
    EventId EventId,
    string Message,
    Exception? Exception,
    IReadOnlyCollection<object?> Scopes
);
