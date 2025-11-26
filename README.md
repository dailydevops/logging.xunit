# NetEvolve.Logging.XUnit

[![NuGet](https://img.shields.io/nuget/v/NetEvolve.Logging.XUnit.svg)](https://www.nuget.org/packages/NetEvolve.Logging.XUnit/)
[![License](https://img.shields.io/github/license/dailydevops/logging.xunit.svg)](LICENSE)

A powerful logging implementation for [xUnit.net](https://xunit.net/) that enables you to capture, inspect, and assert on log messages generated during test execution. Perfect for debugging tests, verifying logging behavior, and building testable applications with comprehensive logging coverage.

## Features

- ✅ **Full `Microsoft.Extensions.Logging` integration** - Works seamlessly with the standard .NET logging infrastructure
- ✅ **Message inspection and assertions** - Access all logged messages for verification in your tests
- ✅ **Multiple creation patterns** - Direct instantiation or dependency injection via `ILoggingBuilder`
- ✅ **Configurable output** - Control timestamps, log levels, scopes, and additional information
- ✅ **Scope support** - Captures and displays logging scopes for better context
- ✅ **`TimeProvider` support** - Testable time-based logging scenarios
- ✅ **xUnit v3 compatibility** - Works with both `ITestOutputHelper` and `IMessageSink`
- ✅ **Generic logger support** - Strongly-typed loggers with `XUnitLogger<T>`