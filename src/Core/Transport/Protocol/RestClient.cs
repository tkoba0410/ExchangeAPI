using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Core.Contracts.Transport;
using ExchangeApi.Core.Transport.Observability;
using ExchangeApi.Core.Transport.Policy;
using ExchangeApi.Core.Transport.Http;

namespace ExchangeApi.Core.Transport.Protocol
{
    public class RestClient : IRestClient
    {
        private static readonly ProductInfoHeaderValue DefaultUserAgent =
        new("ExchangeApi", "1.0");
        private static readonly MediaTypeWithQualityHeaderValue JsonMediaType =
            new("application/json");
        private readonly Uri _baseUri;
        private readonly IHttpTransport _transport;
        private readonly JsonSerializerOptions _serializerOptions;
        private readonly IRequestSigner? _requestSigner;
        private readonly IHttpPolicy _policy;
        private readonly IRestClientLogger _logger;
        private readonly IRestCallObserver _observer;
        private readonly IExchangeErrorClassifier? _errorClassifier;
        private readonly IErrorPayloadParser _errorPayloadParser;

        public RestClient(
            Uri baseUri,
            IHttpTransport transport,
            JsonSerializerOptions? serializerOptions = null,
            IRequestSigner? requestSigner = null,
            IHttpPolicy? policy = null,
            IRestClientLogger? logger = null,
            IRestCallObserver? observer = null,
            IExchangeErrorClassifier? errorClassifier = null,
            IErrorPayloadParser? errorPayloadParser = null)
        {
            _baseUri = baseUri ?? throw new ArgumentNullException(nameof(baseUri));
            _transport = transport ?? throw new ArgumentNullException(nameof(transport));
            _serializerOptions = serializerOptions ?? new JsonSerializerOptions(JsonSerializerDefaults.Web);
            _requestSigner = requestSigner;
            _policy = policy ?? NoOpHttpPolicy.Instance;
            _logger = logger ?? NoOpRestClientLogger.Instance;
            _observer = observer ?? NoOpRestCallObserver.Instance;
            _errorClassifier = errorClassifier;
            _errorPayloadParser = errorPayloadParser ?? DefaultErrorPayloadParser.Instance;
        }

        public async Task<TResponse> GetAsync<TResponse>(
                    string path,
                    IReadOnlyDictionary<string, string?>? query = null,
                    CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Path must not be null or whitespace.", nameof(path));
            }

            var requestUri = BuildUri(path, query);

            using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            _logger.LogRequest(request);
            var context = new RestCallContext(request);
            var startedAt = DateTimeOffset.UtcNow;
            _observer.OnRequest(context);

            if (!request.Headers.UserAgent.Any())
            {
                request.Headers.UserAgent.Add(DefaultUserAgent);
            }

            if (!request.Headers.Accept.Any())
            {
                request.Headers.Accept.Add(JsonMediaType);
            }

            try
            {
                if (_requestSigner is not null)
                {
                    await _requestSigner.SignAsync(request, cancellationToken).ConfigureAwait(false);
                }

                using var response = await _policy
                    .ExecuteAsync(request, ct => _transport.SendAsync(request, ct), cancellationToken)
                    .ConfigureAwait(false);

                var content = response.Content is null
                    ? string.Empty
                    : await response.Content
                        .ReadAsStringAsync(cancellationToken)
                        .ConfigureAwait(false);

                _logger.LogResponse(response, content);
                _observer.OnResponse(context, response, content, DateTimeOffset.UtcNow - startedAt);

                // ★ HTTP ステータス異常 → TransportException
                if (!response.IsSuccessStatusCode)
                {
                    var payload = _errorPayloadParser.Parse(content);
                    var errorCode = payload.ErrorCode;
                    var category = _errorClassifier?.Classify(response.StatusCode, errorCode);

                    var exception = new TransportException(
                        $"Request to '{requestUri}' failed with status {(int)response.StatusCode} ({response.StatusCode}). Error: {payload.ErrorMessage ?? content}",
                        statusCode: response.StatusCode,
                        errorCode: errorCode,
                        errorCategory: category);
                    _observer.OnError(context, exception, DateTimeOffset.UtcNow - startedAt, response.StatusCode);
                    throw exception;
                }

                try
                {
                    var result = JsonSerializer.Deserialize<TResponse>(content, _serializerOptions);

                    if (result is null)
                    {
                        var parseEx = new TransportException(
                            $"Failed to deserialize response from '{requestUri}' as {typeof(TResponse).Name}.");
                        _observer.OnError(context, parseEx, DateTimeOffset.UtcNow - startedAt);
                        throw new TransportException(
                            $"Failed to deserialize response from '{requestUri}' as {typeof(TResponse).Name}.");
                    }

                    return result;
                }
                catch (JsonException ex)
                {
                    // ★ JSON パース失敗 → TransportException
                    var wrapped = new TransportException(
                        "Failed to deserialize JSON response.",
                        innerException: ex);
                    _observer.OnError(context, wrapped, DateTimeOffset.UtcNow - startedAt);
                    throw wrapped;
                }
            }
            catch (TransportException ex)
            {
                // すでにラップ済みのものはそのまま上に投げる
                _observer.OnError(context, ex, DateTimeOffset.UtcNow - startedAt, ex.StatusCode);
                throw;
            }
            catch (HttpRequestException ex)
            {
                // 通信エラー → TransportException に詳細を引き継ぐ
                var category = _errorClassifier?.Classify(ex.StatusCode, null) ?? TransportErrorCategory.Network;
                var wrapped = new TransportException(
                    $"HTTP request failed for '{requestUri}'.",
                    statusCode: ex.StatusCode,
                    errorCategory: category,
                    innerException: ex);
                _logger.LogError(wrapped, request);
                _observer.OnError(context, wrapped, DateTimeOffset.UtcNow - startedAt, ex.StatusCode);
                throw wrapped;
            }

            catch (TaskCanceledException ex)
            {
                // ★ タイムアウト or キャンセル → TransportException
                var category = _errorClassifier?.Classify(null, null) ?? TransportErrorCategory.Network;
                var wrapped = new TransportException(
                    "HTTP request timed out or was canceled.",
                    errorCategory: category,
                    innerException: ex);
                _logger.LogError(wrapped, request);
                _observer.OnError(context, wrapped, DateTimeOffset.UtcNow - startedAt);
                throw wrapped;
            }
        }

        public async Task<HttpResponseMeta> GetRawAsync(
            string path,
            IReadOnlyDictionary<string, string?>? query = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Path must not be null or whitespace.", nameof(path));
            }

            var requestUri = BuildUri(path, query);
            using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            return await SendRawAsync(request, cancellationToken).ConfigureAwait(false);
        }

        public async Task<TResponse> PostAsync<TRequest, TResponse>(
            string path,
            TRequest body,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Path must not be null or whitespace.", nameof(path));
            }

            var requestUri = BuildUri(path, query: null);
            var json = JsonSerializer.Serialize(body, _serializerOptions);

            using var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json"),
            };

            _logger.LogRequest(request);
            var context = new RestCallContext(request);
            var startedAt = DateTimeOffset.UtcNow;
            _observer.OnRequest(context);

            if (!request.Headers.UserAgent.Any())
            {
                request.Headers.UserAgent.Add(DefaultUserAgent);
            }

            if (!request.Headers.Accept.Any())
            {
                request.Headers.Accept.Add(JsonMediaType);
            }

            try
            {
                if (_requestSigner is not null)
                {
                    await _requestSigner.SignAsync(request, cancellationToken).ConfigureAwait(false);
                }

                using var response = await _policy
                    .ExecuteAsync(request, ct => _transport.SendAsync(request, ct), cancellationToken)
                    .ConfigureAwait(false);

                var content = response.Content is null
                    ? string.Empty
                    : await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

                _logger.LogResponse(response, content);
                _observer.OnResponse(context, response, content, DateTimeOffset.UtcNow - startedAt);

                if (!response.IsSuccessStatusCode)
                {
                    var payload = _errorPayloadParser.Parse(content);
                    var errorCode = payload.ErrorCode;
                    var category = _errorClassifier?.Classify(response.StatusCode, errorCode);

                    var exception = new TransportException(
                        $"Request to '{requestUri}' failed with status {(int)response.StatusCode} ({response.StatusCode}). Error: {payload.ErrorMessage ?? content}",
                        statusCode: response.StatusCode,
                        errorCode: errorCode,
                        errorCategory: category);
                    _observer.OnError(context, exception, DateTimeOffset.UtcNow - startedAt, response.StatusCode);
                    throw exception;
                }

                try
                {
                    var result = JsonSerializer.Deserialize<TResponse>(content, _serializerOptions);
                    if (result is null)
                    {
                        var parseEx = new TransportException(
                            $"Failed to deserialize response from '{requestUri}' as {typeof(TResponse).Name}.");
                        _observer.OnError(context, parseEx, DateTimeOffset.UtcNow - startedAt);
                        throw new TransportException(
                            $"Failed to deserialize response from '{requestUri}' as {typeof(TResponse).Name}.");
                    }

                    return result;
                }
                catch (JsonException ex)
                {
                    var wrapped = new TransportException(
                        "Failed to deserialize JSON response.",
                        innerException: ex);
                    _observer.OnError(context, wrapped, DateTimeOffset.UtcNow - startedAt);
                    throw wrapped;
                }
            }
            catch (TransportException ex)
            {
                _observer.OnError(context, ex, DateTimeOffset.UtcNow - startedAt, ex.StatusCode);
                throw;
            }
            catch (HttpRequestException ex)
            {
                var category = _errorClassifier?.Classify(ex.StatusCode, null) ?? TransportErrorCategory.Network;
                var wrapped = new TransportException(
                    $"HTTP request failed for '{requestUri}'.",
                    statusCode: ex.StatusCode,
                    errorCategory: category,
                    innerException: ex);
                _logger.LogError(wrapped, request);
                _observer.OnError(context, wrapped, DateTimeOffset.UtcNow - startedAt, ex.StatusCode);
                throw wrapped;
            }
            catch (TaskCanceledException ex)
            {
                var category = _errorClassifier?.Classify(null, null) ?? TransportErrorCategory.Network;
                var wrapped = new TransportException(
                    "HTTP request timed out or was canceled.",
                    errorCategory: category,
                    innerException: ex);
                _logger.LogError(wrapped, request);
                _observer.OnError(context, wrapped, DateTimeOffset.UtcNow - startedAt);
                throw wrapped;
            }
        }

        public async Task<HttpResponseMeta> PostRawAsync<TRequest>(
            string path,
            TRequest body,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Path must not be null or whitespace.", nameof(path));
            }

            var requestUri = BuildUri(path, query: null);
            var json = JsonSerializer.Serialize(body, _serializerOptions);

            using var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json"),
            };

            return await SendRawAsync(request, cancellationToken).ConfigureAwait(false);
        }

        public async Task<HttpResponseMeta> SendRawAsync(
            string method,
            string path,
            string? query = null,
            string? bodyJson = null,
            IReadOnlyDictionary<string, string>? headers = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(method))
            {
                throw new ArgumentException("Method must not be null or whitespace.", nameof(method));
            }

            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Path must not be null or whitespace.", nameof(path));
            }

            var requestUri = BuildUriWithQuery(path, query);
            using var request = new HttpRequestMessage(new HttpMethod(method), requestUri);

            if (!string.IsNullOrWhiteSpace(bodyJson))
            {
                request.Content = new StringContent(bodyJson, Encoding.UTF8, "application/json");
            }

            if (headers is not null)
            {
                ApplyHeaders(request, headers);
            }

            return await SendRawAsync(request, cancellationToken).ConfigureAwait(false);
        }

        private async Task<HttpResponseMeta> SendRawAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            _logger.LogRequest(request);
            var context = new RestCallContext(request);
            var startedAt = DateTimeOffset.UtcNow;
            _observer.OnRequest(context);

            if (!request.Headers.UserAgent.Any())
            {
                request.Headers.UserAgent.Add(DefaultUserAgent);
            }

            if (!request.Headers.Accept.Any())
            {
                request.Headers.Accept.Add(JsonMediaType);
            }

            try
            {
                if (_requestSigner is not null)
                {
                    await _requestSigner.SignAsync(request, cancellationToken).ConfigureAwait(false);
                }

                using var response = await _policy
                    .ExecuteAsync(request, ct => _transport.SendAsync(request, ct), cancellationToken)
                    .ConfigureAwait(false);

                var content = response.Content is null
                    ? string.Empty
                    : await response.Content
                        .ReadAsStringAsync(cancellationToken)
                        .ConfigureAwait(false);

                _logger.LogResponse(response, content);
                _observer.OnResponse(context, response, content, DateTimeOffset.UtcNow - startedAt);

                var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (var header in response.Headers)
                {
                    headers[header.Key] = string.Join(",", header.Value);
                }

                if (response.Content?.Headers is not null)
                {
                    foreach (var header in response.Content.Headers)
                    {
                        headers[header.Key] = string.Join(",", header.Value);
                    }
                }

                return new HttpResponseMeta(
                    StatusCode: (int)response.StatusCode,
                    Headers: headers.Count == 0 ? null : headers,
                    Body: content);
            }
            catch (HttpRequestException ex)
            {
                var category = _errorClassifier?.Classify(ex.StatusCode, null) ?? TransportErrorCategory.Network;
                var wrapped = new TransportException(
                    "HTTP request failed.",
                    statusCode: ex.StatusCode,
                    errorCategory: category,
                    innerException: ex);
                _logger.LogError(wrapped, request);
                _observer.OnError(context, wrapped, DateTimeOffset.UtcNow - startedAt, ex.StatusCode);
                throw wrapped;
            }
            catch (TaskCanceledException ex)
            {
                var category = _errorClassifier?.Classify(null, null) ?? TransportErrorCategory.Network;
                var wrapped = new TransportException(
                    "HTTP request timed out or was canceled.",
                    errorCategory: category,
                    innerException: ex);
                _logger.LogError(wrapped, request);
                _observer.OnError(context, wrapped, DateTimeOffset.UtcNow - startedAt);
                throw wrapped;
            }
        }
        private Uri BuildUri(string path, IReadOnlyDictionary<string, string?>? query)
        {
            var combined = new Dictionary<string, string>(StringComparer.Ordinal);

            // ベース + path
            var baseUri = new Uri(_baseUri, path);
            var builder = new UriBuilder(baseUri);

            // 既存クエリを先に取り込む
            if (!string.IsNullOrWhiteSpace(builder.Query))
            {
                var pairs = builder.Query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries);
                foreach (var pair in pairs)
                {
                    var kv = pair.Split('=', 2);
                    var key = Uri.UnescapeDataString(kv[0]);
                    var value = kv.Length > 1 ? Uri.UnescapeDataString(kv[1]) : string.Empty;
                    combined[key] = value;
                }
            }

            // 引数のqueryをマージ（重複は例外）
            if (query is { Count: > 0 })
            {
                foreach (var kv in query.Where(kv => kv.Value is not null))
                {
                    if (combined.TryGetValue(kv.Key, out var existing) && existing != kv.Value)
                    {
                        throw new ArgumentException($"Duplicate query parameter '{kv.Key}'.");
                    }
                    combined[kv.Key] = kv.Value!;
                }
            }

            if (combined.Count > 0)
            {
                var queryString = string.Join(
                    "&",
                    combined.Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}"));
                builder.Query = queryString;
            }
            else
            {
                builder.Query = string.Empty;
            }

            return builder.Uri;
        }

        private Uri BuildUriWithQuery(string path, string? query)
        {
            var baseUri = new Uri(_baseUri, path);
            if (string.IsNullOrWhiteSpace(query))
            {
                return baseUri;
            }

            var builder = new UriBuilder(baseUri)
            {
                Query = query.StartsWith("?", StringComparison.Ordinal)
                    ? query[1..]
                    : query,
            };
            return builder.Uri;
        }

        private static void ApplyHeaders(HttpRequestMessage request, IReadOnlyDictionary<string, string> headers)
        {
            foreach (var (key, value) in headers)
            {
                if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value))
                {
                    continue;
                }

                if (string.Equals(key, "Content-Type", StringComparison.OrdinalIgnoreCase))
                {
                    if (request.Content is null)
                    {
                        continue;
                    }

                    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(value);
                    continue;
                }

                request.Headers.TryAddWithoutValidation(key, value);
            }
        }

    }
}
