using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using ExchangeApi.Primitives.Credentials;

namespace ExchangeApi.Exchanges.Bitflyer.Protocol.Realtime;

public sealed class BitflyerRealtimeProtocolClient : IBitflyerPrivateRealtimeProtocolClient
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    private readonly IBitflyerRealtimeTransport _transport;
    private readonly Uri _endpointUri;
    private readonly Func<DateTimeOffset> _clock;
    private readonly Func<string> _nonceFactory;
    private bool _connected;

    public BitflyerRealtimeProtocolClient(
        IBitflyerRealtimeTransport transport,
        Uri endpointUri,
        Func<DateTimeOffset>? clock = null,
        Func<string>? nonceFactory = null)
    {
        _transport = transport ?? throw new ArgumentNullException(nameof(transport));
        _endpointUri = endpointUri ?? throw new ArgumentNullException(nameof(endpointUri));
        _clock = clock ?? (() => DateTimeOffset.UtcNow);
        _nonceFactory = nonceFactory ?? CreateNonce;
    }

    public async ValueTask ConnectAsync(CancellationToken cancellationToken = default)
    {
        if (_connected)
        {
            return;
        }

        await _transport.ConnectAsync(_endpointUri, cancellationToken).ConfigureAwait(false);
        _connected = true;
    }

    public async ValueTask SubscribeAsync(string channel, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(channel);

        await ConnectAsync(cancellationToken).ConfigureAwait(false);
        await SendRequestAsync("subscribe", channel, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask UnsubscribeAsync(string channel, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(channel);

        await ConnectAsync(cancellationToken).ConfigureAwait(false);
        await SendRequestAsync("unsubscribe", channel, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask AuthenticateAsync(
        IApiCredentialSession credentialSession,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(credentialSession);

        await ConnectAsync(cancellationToken).ConfigureAwait(false);

        var timestamp = _clock().ToUnixTimeMilliseconds();
        var nonce = _nonceFactory();
        var signature = credentialSession.Sign($"{timestamp}{nonce}");
        var request = new BitflyerRealtimeJsonRpcRequest
        {
            Id = 1,
            Method = "auth",
            Params = new BitflyerRealtimeAuthenticationRequestParams
            {
                ApiKey = credentialSession.ApiKey,
                Timestamp = timestamp,
                Nonce = nonce,
                Signature = signature,
            },
        };

        var text = JsonSerializer.Serialize(request, SerializerOptions);
        await _transport.SendTextAsync(text, cancellationToken).ConfigureAwait(false);
        await ReadAuthenticationResponseAsync(cancellationToken).ConfigureAwait(false);
    }

    public async IAsyncEnumerable<BitflyerRealtimeChannelMessage> ReadMessagesAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await ConnectAsync(cancellationToken).ConfigureAwait(false);

        await foreach (var text in _transport.ReadTextAsync(cancellationToken).ConfigureAwait(false))
        {
            var message = TryParseChannelMessage(text, _clock());
            if (message is not null)
            {
                yield return message;
            }
        }
    }

    public ValueTask DisposeAsync()
    {
        return _transport.DisposeAsync();
    }

    private async ValueTask SendRequestAsync(string method, string channel, CancellationToken cancellationToken)
    {
        var request = new BitflyerRealtimeJsonRpcRequest
        {
            Method = method,
            Params = new BitflyerRealtimeJsonRpcRequestParams
            {
                Channel = channel,
            },
        };

        var text = JsonSerializer.Serialize(request, SerializerOptions);
        await _transport.SendTextAsync(text, cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask ReadAuthenticationResponseAsync(CancellationToken cancellationToken)
    {
        await foreach (var text in _transport.ReadTextAsync(cancellationToken).ConfigureAwait(false))
        {
            try
            {
                using var document = JsonDocument.Parse(text);
                var root = document.RootElement;
                if (root.ValueKind != JsonValueKind.Object)
                {
                    throw new BitflyerRealtimeAuthenticationException("Realtime authentication response must be a JSON object.");
                }

                if (root.TryGetProperty("error", out var error) && error.ValueKind != JsonValueKind.Null)
                {
                    throw new BitflyerRealtimeAuthenticationException("Realtime authentication failed.");
                }

                if (root.TryGetProperty("result", out var result)
                    && result.ValueKind is JsonValueKind.True or JsonValueKind.False)
                {
                    if (result.GetBoolean())
                    {
                        return;
                    }

                    throw new BitflyerRealtimeAuthenticationException("Realtime authentication was rejected.");
                }
            }
            catch (JsonException exception)
            {
                throw new BitflyerRealtimeAuthenticationException(
                    $"Realtime authentication response is not valid JSON: {exception.Message}");
            }
        }

        throw new BitflyerRealtimeAuthenticationException("Realtime authentication response was not received.");
    }

    private static BitflyerRealtimeChannelMessage? TryParseChannelMessage(string text, DateTimeOffset receivedAt)
    {
        try
        {
            using var document = JsonDocument.Parse(text);
            var root = document.RootElement;
            if (root.ValueKind != JsonValueKind.Object)
            {
                throw new BitflyerRealtimeMessageException("Realtime message must be a JSON object.");
            }

            if (!root.TryGetProperty("method", out var methodProperty)
                || methodProperty.ValueKind != JsonValueKind.String
                || methodProperty.GetString() != "channelMessage")
            {
                return null;
            }

            if (!root.TryGetProperty("params", out var parameters) || parameters.ValueKind != JsonValueKind.Object)
            {
                throw new BitflyerRealtimeMessageException("Realtime channel message is missing params.");
            }

            if (!parameters.TryGetProperty("channel", out var channelProperty)
                || channelProperty.ValueKind != JsonValueKind.String
                || string.IsNullOrWhiteSpace(channelProperty.GetString()))
            {
                throw new BitflyerRealtimeMessageException("Realtime channel message is missing channel.");
            }

            if (!parameters.TryGetProperty("message", out var messageProperty))
            {
                throw new BitflyerRealtimeMessageException("Realtime channel message is missing payload.");
            }

            return new BitflyerRealtimeChannelMessage
            {
                Channel = channelProperty.GetString()!,
                Message = messageProperty.Clone(),
                ReceivedAt = receivedAt,
                RawText = text,
                RawTextLength = Encoding.UTF8.GetByteCount(text),
            };
        }
        catch (JsonException exception)
        {
            throw new BitflyerRealtimeMessageException("Realtime message is not valid JSON.", exception);
        }
    }

    private static string CreateNonce()
    {
        return Convert.ToHexString(RandomNumberGenerator.GetBytes(16)).ToLowerInvariant();
    }
}
