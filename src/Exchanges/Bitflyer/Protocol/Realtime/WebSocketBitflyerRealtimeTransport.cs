using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;

namespace ExchangeApi.Exchanges.Bitflyer.Protocol.Realtime;

public sealed class WebSocketBitflyerRealtimeTransport : IBitflyerRealtimeTransport
{
    private const int BufferSize = 8192;

    private readonly ClientWebSocket _webSocket;
    private bool _disposed;

    public WebSocketBitflyerRealtimeTransport()
        : this(new ClientWebSocket())
    {
    }

    internal WebSocketBitflyerRealtimeTransport(ClientWebSocket webSocket)
    {
        _webSocket = webSocket;
    }

    public async ValueTask ConnectAsync(Uri endpointUri, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (_webSocket.State == WebSocketState.Open)
        {
            return;
        }

        await _webSocket.ConnectAsync(endpointUri, cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask SendTextAsync(string text, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var bytes = Encoding.UTF8.GetBytes(text);
        await _webSocket.SendAsync(
            new ArraySegment<byte>(bytes),
            WebSocketMessageType.Text,
            endOfMessage: true,
            cancellationToken).ConfigureAwait(false);
    }

    public async IAsyncEnumerable<string> ReadTextAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var buffer = new byte[BufferSize];
        using var stream = new MemoryStream();

        while (!cancellationToken.IsCancellationRequested)
        {
            stream.SetLength(0);
            WebSocketReceiveResult result;

            do
            {
                result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken).ConfigureAwait(false);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    throw new BitflyerRealtimeConnectionException("Realtime connection was closed by the remote endpoint.");
                }

                if (result.MessageType != WebSocketMessageType.Text)
                {
                    throw new BitflyerRealtimeMessageException("Realtime connection received a non-text message.");
                }

                stream.Write(buffer, 0, result.Count);
            }
            while (!result.EndOfMessage);

            yield return Encoding.UTF8.GetString(stream.ToArray());
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        if (_webSocket.State is WebSocketState.Open or WebSocketState.CloseReceived)
        {
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(2));
            try
            {
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "disposed", timeout.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }
            catch (WebSocketException)
            {
            }
        }

        _webSocket.Dispose();
    }
}
