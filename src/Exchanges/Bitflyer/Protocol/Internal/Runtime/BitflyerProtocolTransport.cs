using System.Globalization;
using System.Text;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Auth;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Shared;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Runtime;

public sealed class BitflyerProtocolTransport : IProtocolTransport
{
    private readonly HttpClient _httpClient;
    private readonly Uri _baseUri;
    private readonly IProtocolDebugLogger _debugLogger;
    private readonly string? _apiKey;
    private readonly string? _apiSecret;
    private readonly TimeSpan? _requestTimeout;

    public BitflyerProtocolTransport(
        HttpClient httpClient,
        IProtocolDebugLogger debugLogger,
        string? apiKey,
        string? apiSecret)
        : this(
            httpClient,
            httpClient.BaseAddress ?? throw new ArgumentException("HttpClient.BaseAddress is required when using this constructor.", nameof(httpClient)),
            debugLogger,
            apiKey,
            apiSecret,
            null)
    {
    }

    public BitflyerProtocolTransport(
        HttpClient httpClient,
        Uri baseUri,
        IProtocolDebugLogger debugLogger,
        string? apiKey,
        string? apiSecret,
        TimeSpan? requestTimeout = null)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentNullException.ThrowIfNull(baseUri);
        ArgumentNullException.ThrowIfNull(debugLogger);

        if (!baseUri.IsAbsoluteUri)
        {
            throw new ArgumentException("BaseUri must be absolute.", nameof(baseUri));
        }

        if (requestTimeout is not null && requestTimeout <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(requestTimeout), "RequestTimeout must be greater than zero.");
        }

        _httpClient = httpClient;
        _baseUri = baseUri;
        _debugLogger = debugLogger;
        _apiKey = apiKey;
        _apiSecret = apiSecret;
        _requestTimeout = requestTimeout;
    }

    public async Task<ProtocolTransportResult> SendAsync(
        ProtocolRequest request,
        ProtocolTransportAuthMode authMode,
        CancellationToken cancellationToken = default)
    {
        using var requestCancellation = RequestCancellationScope.Create(cancellationToken, _requestTimeout);

        try
        {
            var pathAndQuery = ProtocolRequestFormatter.ToPathAndQuery(request);
            var requestUri = ProtocolRequestFormatter.ToRequestUri(_baseUri, request);
            using var message = new HttpRequestMessage(new HttpMethod(request.Method), requestUri);
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

            using var response = await _httpClient.SendAsync(message, requestCancellation.EffectiveToken);
            var responseBody = response.Content is null
                ? null
                : await response.Content.ReadAsStringAsync(requestCancellation.EffectiveToken);

            var protocolResponse = new ProtocolResponse
            {
                StatusCode = (int)response.StatusCode,
                Headers = ProtocolHeaderReader.ReadHeaders(response),
                BodyText = responseBody,
            };

            var nowUtc = DateTimeOffset.UtcNow;
            await TryLogAsync(new ProtocolDebugLogEntry
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
            });

            return new ProtocolTransportResult
            {
                IsSuccess = true,
                Response = protocolResponse,
            };
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or OperationCanceledException)
        {
            var errorMessage = requestCancellation.BuildErrorMessage(ex);
            var nowUtc = DateTimeOffset.UtcNow;
            await TryLogAsync(new ProtocolDebugLogEntry
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
                ErrorMessage = errorMessage,
            });

            return new ProtocolTransportResult
            {
                IsSuccess = false,
                Error = new CallError
                {
                    Kind = CallErrorKinds.Transport,
                    Message = errorMessage,
                },
            };
        }
    }

    private async Task TryLogAsync(ProtocolDebugLogEntry entry)
    {
        try
        {
            await _debugLogger.LogAsync(entry, CancellationToken.None);
        }
        catch
        {
            // Debug logging is best-effort and must not change the functional call result.
        }
    }

    private sealed class RequestCancellationScope : IDisposable
    {
        private const int CancellationNone = 0;
        private const int CancellationCaller = 1;
        private const int CancellationTimeout = 2;

        private readonly TimeSpan? _requestTimeout;
        private readonly CancellationTokenSource? _timeoutCts;
        private readonly CancellationTokenSource? _linkedCts;
        private readonly CancellationTokenRegistration _callerRegistration;
        private readonly CancellationTokenRegistration _timeoutRegistration;
        private int _cancellationKind;

        private RequestCancellationScope(CancellationToken callerCancellationToken, TimeSpan? requestTimeout)
        {
            _requestTimeout = requestTimeout;

            if (requestTimeout is null)
            {
                EffectiveToken = callerCancellationToken;
            }
            else
            {
                _timeoutCts = new CancellationTokenSource(requestTimeout.Value);
                _linkedCts = CancellationTokenSource.CreateLinkedTokenSource(callerCancellationToken, _timeoutCts.Token);
                EffectiveToken = _linkedCts.Token;
                _timeoutRegistration = _timeoutCts.Token.Register(
                    static state => ((RequestCancellationScope)state!).TrySetCancellationKind(CancellationTimeout),
                    this);
            }

            if (callerCancellationToken.CanBeCanceled)
            {
                _callerRegistration = callerCancellationToken.Register(
                    static state => ((RequestCancellationScope)state!).TrySetCancellationKind(CancellationCaller),
                    this);
            }
        }

        public CancellationToken EffectiveToken { get; }

        public static RequestCancellationScope Create(CancellationToken callerCancellationToken, TimeSpan? requestTimeout)
        {
            return new RequestCancellationScope(callerCancellationToken, requestTimeout);
        }

        public string BuildErrorMessage(Exception ex)
        {
            return Volatile.Read(ref _cancellationKind) switch
            {
                CancellationTimeout when _requestTimeout is not null
                    => $"Request timed out after {_requestTimeout.Value.ToString("c", CultureInfo.InvariantCulture)}.",
                CancellationCaller => "The request was canceled by the caller.",
                _ => ex.Message,
            };
        }

        public void Dispose()
        {
            _callerRegistration.Dispose();
            _timeoutRegistration.Dispose();
            _linkedCts?.Dispose();
            _timeoutCts?.Dispose();
        }

        private void TrySetCancellationKind(int cancellationKind)
        {
            Interlocked.CompareExchange(ref _cancellationKind, cancellationKind, CancellationNone);
        }
    }
}
