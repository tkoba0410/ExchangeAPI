using ExchangeApi.Exchanges.Bitflyer.Native.Realtime.Private;
using ExchangeApi.Exchanges.Bitflyer.Native.Realtime.Internal;
using ExchangeApi.Exchanges.Bitflyer.Native.Realtime.Public;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Realtime;
using ExchangeApi.Primitives.Credentials;

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

        var resolved = ResolveOptions(options);
        IBitflyerRealtimeTransport effectiveTransport = ApplyTimeout(transport, resolved.ConnectTimeout);

        var protocol = new BitflyerRealtimeProtocolClient(effectiveTransport, resolved.EndpointUri);
        return new BitflyerPublicRealtimeClient(protocol, () => CreateProtocol(new WebSocketBitflyerRealtimeTransport(), resolved), ToResilienceOptions(resolved));
    }

    public static IBitflyerPrivateRealtimeClient CreatePrivateClient(
        IApiCredentialProvider credentialProvider,
        BitflyerRealtimeClientOptions? options = null)
    {
        return CreatePrivateClient(credentialProvider, new WebSocketBitflyerRealtimeTransport(), options);
    }

    public static IBitflyerPrivateRealtimeClient CreatePrivateClient(
        IApiCredentialProvider credentialProvider,
        IBitflyerRealtimeTransport transport,
        BitflyerRealtimeClientOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(credentialProvider);
        ArgumentNullException.ThrowIfNull(transport);

        var resolved = ResolveOptions(options);
        IBitflyerRealtimeTransport effectiveTransport = ApplyTimeout(transport, resolved.ConnectTimeout);

        var protocol = new BitflyerRealtimeProtocolClient(effectiveTransport, resolved.EndpointUri);
        return new BitflyerPrivateRealtimeClient(
            protocol,
            credentialProvider,
            () => CreateProtocol(new WebSocketBitflyerRealtimeTransport(), resolved),
            ToResilienceOptions(resolved));
    }

    private static BitflyerRealtimeClientOptions ResolveOptions(BitflyerRealtimeClientOptions? options)
    {
        var resolved = options ?? new BitflyerRealtimeClientOptions();
        Validate(resolved);
        return resolved;
    }

    private static void Validate(BitflyerRealtimeClientOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(options.EndpointUri);
        ArgumentNullException.ThrowIfNull(options.Reconnect);

        if (options.ConnectTimeout is { } connectTimeout && connectTimeout <= TimeSpan.Zero)
        {
            throw new ArgumentException("ConnectTimeout must be greater than zero when specified.", nameof(options));
        }

        if (options.IdleTimeout is { } idleTimeout && idleTimeout <= TimeSpan.Zero)
        {
            throw new ArgumentException("IdleTimeout must be greater than zero when specified.", nameof(options));
        }

        if (options.Reconnect.MaxAttempts < 0)
        {
            throw new ArgumentException("Reconnect MaxAttempts must be greater than or equal to zero.", nameof(options));
        }

        if (options.Reconnect.InitialDelay < TimeSpan.Zero)
        {
            throw new ArgumentException("Reconnect InitialDelay must be greater than or equal to zero.", nameof(options));
        }

        if (options.Reconnect.MaxDelay < options.Reconnect.InitialDelay)
        {
            throw new ArgumentException("Reconnect MaxDelay must be greater than or equal to InitialDelay.", nameof(options));
        }
    }

    private static IBitflyerPrivateRealtimeProtocolClient CreateProtocol(
        IBitflyerRealtimeTransport transport,
        BitflyerRealtimeClientOptions options)
    {
        return new BitflyerRealtimeProtocolClient(ApplyTimeout(transport, options.ConnectTimeout), options.EndpointUri);
    }

    private static IBitflyerRealtimeTransport ApplyTimeout(
        IBitflyerRealtimeTransport transport,
        TimeSpan? connectTimeout)
    {
        return connectTimeout is { } timeout
            ? new TimeoutBitflyerRealtimeTransport(transport, timeout)
            : transport;
    }

    private static BitflyerRealtimeResilienceOptions ToResilienceOptions(BitflyerRealtimeClientOptions options)
    {
        return new BitflyerRealtimeResilienceOptions
        {
            MaxAttempts = options.Reconnect.MaxAttempts,
            InitialDelay = options.Reconnect.InitialDelay,
            MaxDelay = options.Reconnect.MaxDelay,
            IdleTimeout = options.IdleTimeout,
        };
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
