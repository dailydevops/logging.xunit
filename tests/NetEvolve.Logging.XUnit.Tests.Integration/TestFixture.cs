namespace NetEvolve.Logging.XUnit.Tests.Integration;

using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;

public sealed partial class TestFixture : IAsyncLifetime
{
    public IMessageSink MessageSink { get; }

    public TestFixture(IMessageSink messageSink) => MessageSink = messageSink;

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    public ValueTask InitializeAsync() => ValueTask.CompletedTask;
}
