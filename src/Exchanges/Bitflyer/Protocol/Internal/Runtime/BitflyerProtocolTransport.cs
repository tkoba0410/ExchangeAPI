using System.Text;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Auth;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Shared;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Runtime;

public sealed class BitflyerProtocolTransport : IProtocolTransport
{
    private readonly HttpClient _httpClient;
    private readonly IProtocolDebugLogger _debugLogger;
    private readonly string? _apiKey;
    private readonly string? _apiSecret;

    public BitflyerProtocolTransport(
        HttpClient httpClient,
        IProtocolDebugLogger debugLogger,
        string? apiKey,
        string? apiSecret)
    {
        _httpClient = httpClient;
        _debugLogger = debugLogger;
        _apiKey = apiKey;
        _apiSecret = apiSecret;
    }

    public async Task<ProtocolTransportResult> SendAsync(
        ProtocolRequest request,
        ProtocolTransportAuthMode authMode,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var pathAndQuery = ProtocolRequestFormatter.ToPathAndQuery(request);
            using var message = new HttpRequestMessage(new HttpMethod(request.Method), pathAndQuery);
            var bodyText = request.BodyText ?? string.Empty;

            if (request.BodyText is not null)
            {
                message.Content = new StringContent(request.BodyText, Encoding.UTF8, "application/json");
            }

            if (authMode == ProtocolTransportAuthMode.KeySecret)
            {
                if (string.IsNullOrWhiteSpace(_apiKey) || string.IsNullOrWhiteSpace(_apiSecret))
                {
                    return new ProtocolTransportResult
                    {
                        IsSuccess = false,
                        Error = new CallError
                        {
                            Kind = CallErrorKinds.Transport,
                            Message = "Private credentials are required.",
                        },
                    };
                }

                BitflyerRequestSigner.ApplyPrivateHeaders(
                    message,
                    request.Method,
                    pathAndQuery,
                    bodyText,
                    _apiKey,
                    _apiSecret);
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

            var nowUtc = DateTimeOffset.UtcNow;
            await _debugLogger.LogAsync(new ProtocolDebugLogEntry
            {
                EndpointId = request.EndpointId,
                Method = request.Method,
                Path = request.Path,
                Query = request.Query,
                BodyText = request.BodyText,
                StatusCode = protocolResponse.StatusCode,
                ResponseBodyText = protocolResponse.BodyText,
                TimestampUtc = nowUtc,
                TimestampJst = nowUtc.ToOffset(TimeSpan.FromHours(9)),
            }, cancellationToken);

            return new ProtocolTransportResult
            {
                IsSuccess = true,
                Response = protocolResponse,
            };
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or OperationCanceledException)
        {
            var nowUtc = DateTimeOffset.UtcNow;
            await _debugLogger.LogAsync(new ProtocolDebugLogEntry
            {
                EndpointId = request.EndpointId,
                Method = request.Method,
                Path = request.Path,
                Query = request.Query,
                BodyText = request.BodyText,
                StatusCode = null,
                ResponseBodyText = null,
                TimestampUtc = nowUtc,
                TimestampJst = nowUtc.ToOffset(TimeSpan.FromHours(9)),
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
