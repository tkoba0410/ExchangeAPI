using System.Runtime.CompilerServices;
using System.Text;
using ExchangeApi.Exchanges.Bitflyer.Native.Realtime.Models;
using ExchangeApi.Exchanges.Bitflyer.Native.Realtime.Public;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Realtime;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;

namespace ExchangeApi.Optional.Testing.Realtime;

public static class BitflyerRealtimeReplayRunner
{
    private static readonly Uri EndpointUri = new("wss://ws.lightstream.bitflyer.com/json-rpc");

    public static Task<RealtimeReplayResult<BitflyerRealtimeTickerMessage>> ReplayTickerAsync(
        string productCode,
        IEnumerable<RealtimeReplayFrame> frames,
        CancellationToken cancellationToken = default)
    {
        var channel = BitflyerRealtimeChannels.Ticker(productCode);
        return ReplayAsync(
            channel,
            frames,
            client => client.SubscribeTickerAsync(productCode, cancellationToken),
            cancellationToken);
    }

    public static Task<RealtimeReplayResult<BitflyerRealtimeExecutionMessage>> ReplayExecutionsAsync(
        string productCode,
        IEnumerable<RealtimeReplayFrame> frames,
        CancellationToken cancellationToken = default)
    {
        var channel = BitflyerRealtimeChannels.Executions(productCode);
        return ReplayAsync(
            channel,
            frames,
            client => client.SubscribeExecutionsAsync(productCode, cancellationToken),
            cancellationToken);
    }

    public static Task<RealtimeReplayResult<BitflyerRealtimeBoardSnapshotMessage>> ReplayBoardSnapshotAsync(
        string productCode,
        IEnumerable<RealtimeReplayFrame> frames,
        CancellationToken cancellationToken = default)
    {
        var channel = BitflyerRealtimeChannels.BoardSnapshot(productCode);
        return ReplayAsync(
            channel,
            frames,
            client => client.SubscribeBoardSnapshotsAsync(productCode, cancellationToken),
            cancellationToken);
    }

    public static Task<RealtimeReplayResult<BitflyerRealtimeBoardDeltaMessage>> ReplayBoardDeltaAsync(
        string productCode,
        IEnumerable<RealtimeReplayFrame> frames,
        CancellationToken cancellationToken = default)
    {
        var channel = BitflyerRealtimeChannels.Board(productCode);
        return ReplayAsync(
            channel,
            frames,
            client => client.SubscribeBoardDeltasAsync(productCode, cancellationToken),
            cancellationToken);
    }

    private static async Task<RealtimeReplayResult<T>> ReplayAsync<T>(
        string channel,
        IEnumerable<RealtimeReplayFrame> frames,
        Func<BitflyerPublicRealtimeClient, IAsyncEnumerable<T>> subscribe,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(channel);
        ArgumentNullException.ThrowIfNull(frames);
        ArgumentNullException.ThrowIfNull(subscribe);

        var replayFrames = frames.ToArray();
        var diagnostics = new List<RealtimeDiagnosticEvent>();
        foreach (var frame in replayFrames.Where(frame => frame.Channel == channel))
        {
            diagnostics.Add(Diagnostic(
                channel,
                RealtimeDiagnosticEventTypes.RawFrameReceived,
                RealtimeDiagnosticSeverities.Trace,
                "Raw realtime frame replayed.",
                frame.ReceivedAt,
                new Dictionary<string, string>
                {
                    ["payloadLength"] = Encoding.UTF8.GetByteCount(frame.RawText).ToString(),
                }));
        }

        var transport = new ReplayRealtimeTransport(replayFrames.Select(frame => frame.RawText));
        await using var protocol = new BitflyerRealtimeProtocolClient(transport, EndpointUri);
        await using var client = new BitflyerPublicRealtimeClient(protocol);
        var items = new List<T>();

        try
        {
            await foreach (var item in subscribe(client).WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                items.Add(item);
                diagnostics.Add(Diagnostic(
                    channel,
                    RealtimeDiagnosticEventTypes.MessageDecoded,
                    RealtimeDiagnosticSeverities.Trace,
                    "Realtime message decoded."));
            }

            return new RealtimeReplayResult<T>
            {
                Items = items,
                Diagnostics = diagnostics,
            };
        }
        catch (Exception exception) when (!cancellationToken.IsCancellationRequested)
        {
            var reason = exception.GetType().Name;
            diagnostics.Add(Diagnostic(
                channel,
                RealtimeDiagnosticEventTypes.MessageRejected,
                RealtimeDiagnosticSeverities.Warning,
                reason,
                errorKind: BitflyerRealtimeErrorKind.MessageDecodeFailed.ToString()));

            return new RealtimeReplayResult<T>
            {
                Items = items,
                Diagnostics = diagnostics,
                RejectionReason = reason,
                ErrorKind = BitflyerRealtimeErrorKind.MessageDecodeFailed.ToString(),
            };
        }
    }

    private static RealtimeDiagnosticEvent Diagnostic(
        string channel,
        string eventType,
        string severity,
        string reason,
        DateTimeOffset? observedAt = null,
        IReadOnlyDictionary<string, string>? attributes = null,
        string? errorKind = null)
    {
        return new RealtimeDiagnosticEvent
        {
            EventType = eventType,
            ObservedAt = observedAt ?? DateTimeOffset.UtcNow,
            Venue = "bitFlyer",
            Channel = channel,
            Severity = severity,
            Reason = reason,
            ErrorKind = errorKind,
            Attributes = attributes,
        };
    }

    private sealed class ReplayRealtimeTransport : IBitflyerRealtimeTransport
    {
        private readonly IReadOnlyList<string> _frames;

        public ReplayRealtimeTransport(IEnumerable<string> frames)
        {
            _frames = frames.ToArray();
        }

        public ValueTask ConnectAsync(Uri endpointUri, CancellationToken cancellationToken = default)
        {
            return ValueTask.CompletedTask;
        }

        public ValueTask SendTextAsync(string text, CancellationToken cancellationToken = default)
        {
            return ValueTask.CompletedTask;
        }

        public async IAsyncEnumerable<string> ReadTextAsync(
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            foreach (var frame in _frames)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await Task.Yield();
                yield return frame;
            }
        }

        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }
    }
}
