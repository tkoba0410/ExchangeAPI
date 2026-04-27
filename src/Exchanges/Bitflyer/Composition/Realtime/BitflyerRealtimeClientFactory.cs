using ExchangeApi.Exchanges.Bitflyer.Native.Realtime.Public;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Realtime;

namespace ExchangeApi.Exchanges.Bitflyer.Composition.Realtime;

public static class BitflyerRealtimeClientFactory
{
    public static IBitflyerPublicRealtimeClient CreatePublicClient(BitflyerRealtimeClientOptions? options = null)
    {
        return CreatePublicClient(new WebSocketBitflyerRealtimeTransport(), options);
    }

    public static IBitflyerPublicRealtimeClient CreatePublicClient(
        IBitflyerRealtimeTransport transport,
        BitflyerRealtimeClientOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(transport);

        var resolved = options ?? new BitflyerRealtimeClientOptions();
        IBitflyerRealtimeTransport effectiveTransport = resolved.ConnectTimeout is { } timeout
            ? new TimeoutBitflyerRealtimeTransport(transport, timeout)
            : transport;

        var protocol = new BitflyerRealtimeProtocolClient(effectiveTransport, resolved.EndpointUri);
        return new BitflyerPublicRealtimeClient(protocol);
    }

    private sealed class TimeoutBitflyerRealtimeTransport : IBitflyerRealtimeTransport
    {
        private readonly IBitflyerRealtimeTransport _inner;
        private readonly TimeSpan _connectTimeout;

        internal TimeoutBitflyerRealtimeTransport(IBitflyerRealtimeTransport inner, TimeSpan connectTimeout)
        {
            _inner = inner;
            _connectTimeout = connectTimeout;
        }

        public async ValueTask ConnectAsync(Uri endpointUri, CancellationToken cancellationToken = default)
        {
            using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeout.CancelAfter(_connectTimeout);

            try
            {
                await _inner.ConnectAsync(endpointUri, timeout.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException exception) when (!cancellationToken.IsCancellationRequested)
            {
                throw new BitflyerRealtimeConnectionException($"Realtime connection timed out after {_connectTimeout:c}.", exception);
            }
        }

        public ValueTask SendTextAsync(string text, CancellationToken cancellationToken = default)
        {
            return _inner.SendTextAsync(text, cancellationToken);
        }

        public IAsyncEnumerable<string> ReadTextAsync(CancellationToken cancellationToken = default)
        {
            return _inner.ReadTextAsync(cancellationToken);
        }

        public ValueTask DisposeAsync()
        {
            return _inner.DisposeAsync();
        }
    }
}
