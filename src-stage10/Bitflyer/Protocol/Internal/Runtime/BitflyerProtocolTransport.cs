using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Stage10.Bitflyer.Protocol.Internal.Runtime;

public sealed class BitflyerProtocolTransport : IWireTransport
{
    private static readonly IReadOnlyDictionary<string, string> PublicTags = new Dictionary<string, string>
    {
        ["Scope"] = "Public",
        ["Auth"] = "None",
        ["Retryable"] = "false",
    };

    private static readonly IReadOnlyDictionary<string, string> PrivateTags = new Dictionary<string, string>
    {
        ["Scope"] = "Private",
        ["Auth"] = "Required",
        ["Retryable"] = "false",
    };

    private readonly IRestClient _restClient;
    private readonly bool _useTickerAliasPath;

    public BitflyerProtocolTransport(
        IRestClient restClient,
        bool useTickerAliasPath = false)
    {
        _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
        _useTickerAliasPath = useTickerAliasPath;
    }

    public async Task<Call<WireCallSpec, WireResponse>> SendAsync(
        WireCallSpec request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var responseMeta = await _restClient
                .SendRawAsync(
                    request.Method,
                    ResolveTransportPath(request),
                    request.Query,
                    request.BodyJson,
                    request.Headers,
                    cancellationToken)
                .ConfigureAwait(false);
            var duration = DateTimeOffset.UtcNow - startedAt;

            var responseHeaders = responseMeta.Headers is null
                ? null
                : new Dictionary<string, string>(responseMeta.Headers, StringComparer.OrdinalIgnoreCase);

            var response = new WireResponse(
                StatusCode: responseMeta.StatusCode,
                Json: responseMeta.Body ?? string.Empty,
                Headers: responseHeaders,
                RequestId: TryGetRequestId(responseHeaders),
                ElapsedMs: (long)duration.TotalMilliseconds);

            return new Call<WireCallSpec, WireResponse>(
                Id: CallId.New(),
                StartedAt: startedAt,
                Duration: duration,
                Request: request,
                Result: new CallResult<WireResponse>.Ok(response),
                Meta: new CallMeta(
                    Layer: "Protocol",
                    Component: "Transport",
                    EndpointId: request.EndpointId,
                    Tags: GetTags(request.EndpointId),
                    Children: null));
        }
        catch (OperationCanceledException)
        {
            var duration = DateTimeOffset.UtcNow - startedAt;
            return CreateTransportErrorCall(
                request,
                startedAt,
                duration,
                new CallError(CallErrorKind.Transport, "Protocol transport canceled."));
        }
        catch (Exception ex)
        {
            var duration = DateTimeOffset.UtcNow - startedAt;
            return CreateTransportErrorCall(
                request,
                startedAt,
                duration,
                new CallError(CallErrorKind.Transport, "Protocol transport failed.", ex));
        }
    }

    private static Call<WireCallSpec, WireResponse> CreateTransportErrorCall(
        WireCallSpec request,
        DateTimeOffset startedAt,
        TimeSpan duration,
        CallError error) =>
        new(
            Id: CallId.New(),
            StartedAt: startedAt,
            Duration: duration,
            Request: request,
            Result: new CallResult<WireResponse>.Err(error),
            Meta: new CallMeta(
                Layer: "Protocol",
                Component: "Transport",
                EndpointId: request.EndpointId,
                Tags: GetTags(request.EndpointId),
                Children: null));

    private string ResolveTransportPath(WireCallSpec request)
    {
        if (_useTickerAliasPath && request.EndpointId == Vocabulary.EndpointIds.GetTicker)
        {
            return ProtocolPaths.GetTickerAlias;
        }

        return request.Path;
    }

    private static IReadOnlyDictionary<string, string> GetTags(string endpointId) =>
        endpointId == Vocabulary.EndpointIds.GetTicker ? PublicTags : PrivateTags;

    private static string? TryGetRequestId(IReadOnlyDictionary<string, string>? headers)
    {
        if (headers is null)
        {
            return null;
        }

        if (headers.TryGetValue("X-Request-Id", out var requestId))
        {
            return requestId;
        }

        return headers.TryGetValue("Request-Id", out requestId) ? requestId : null;
    }
}
