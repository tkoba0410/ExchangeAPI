using System.Text;
using ExchangeApi.Exchanges.Binance.Protocol.Internal.Shared;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Exchanges.Binance.Protocol.Internal.Runtime;

public sealed class BinanceProtocolTransport : IProtocolTransport
{
    private readonly HttpClient _httpClient;
    private readonly IProtocolDebugLogger _debugLogger;

    public BinanceProtocolTransport(HttpClient httpClient, IProtocolDebugLogger debugLogger)
    {
        _httpClient = httpClient;
        _debugLogger = debugLogger;
    }

    public async Task<ProtocolTransportResult> SendAsync(
        ProtocolRequest request,
        ProtocolTransportAuthMode authMode,
        CancellationToken cancellationToken = default)
    {
        if (authMode != ProtocolTransportAuthMode.None)
        {
            return new ProtocolTransportResult
            {
                IsSuccess = false,
                Error = new CallError
                {
                    Kind = CallErrorKinds.Transport,
                    Message = "Unsupported auth mode for Binance public transport.",
                },
            };
        }

        try
        {
            var pathAndQuery = ProtocolRequestFormatter.ToPathAndQuery(request);
            using var message = new HttpRequestMessage(new HttpMethod(request.Method), pathAndQuery);

            if (request.BodyText is not null)
            {
                message.Content = new StringContent(request.BodyText, Encoding.UTF8, "application/json");
            }

            using var response = await _httpClient.SendAsync(message, cancellationToken);
            var responseBody = response.Content is null
                ? null
                : await response.Content.ReadAsStringAsync(cancellationToken);

            var protocolResponse = new ProtocolResponse
            {
                StatusCode = (int)response.StatusCode,
                Headers = ProtocolHeaderReader.ReadHeaders(response),
                BodyText = responseBody,
            };

            await _debugLogger.LogAsync(new ProtocolDebugLogEntry
            {
                EndpointId = request.EndpointId,
                Method = request.Method,
                Path = request.Path,
                Query = request.Query,
                BodyText = request.BodyText,
                StatusCode = protocolResponse.StatusCode,
                ResponseBodyText = protocolResponse.BodyText,
                TimestampUtc = DateTimeOffset.UtcNow,
            }, cancellationToken);

            return new ProtocolTransportResult
            {
                IsSuccess = true,
                Response = protocolResponse,
            };
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or OperationCanceledException)
        {
            await _debugLogger.LogAsync(new ProtocolDebugLogEntry
            {
                EndpointId = request.EndpointId,
                Method = request.Method,
                Path = request.Path,
                Query = request.Query,
                BodyText = request.BodyText,
                StatusCode = null,
                ResponseBodyText = null,
                TimestampUtc = DateTimeOffset.UtcNow,
                ErrorMessage = ex.Message,
            }, cancellationToken);

            return new ProtocolTransportResult
            {
                IsSuccess = false,
                Error = new CallError
                {
                    Kind = CallErrorKinds.Transport,
                    Message = ex.Message,
                },
            };
        }
    }
}
