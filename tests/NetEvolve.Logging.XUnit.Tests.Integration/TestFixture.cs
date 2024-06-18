namespace NetEvolve.Logging.XUnit.Tests.Integration;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

public sealed partial class TestFixture : IAsyncLifetime
{
    public IMessageSink MessageSink { get; }

    public TestFixture(IMessageSink messageSink) => MessageSink = messageSink;

    public Task DisposeAsync() => Task.CompletedTask;

    public Task InitializeAsync() => Task.CompletedTask;
}
