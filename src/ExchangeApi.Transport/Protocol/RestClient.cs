using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Errors;
using ExchangeApi.Transport.Logging;
using ExchangeApi.Transport.Policy;
using ExchangeApi.Transport.Transport;

namespace ExchangeApi.Transport.Protocol
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

        public RestClient(
            Uri baseUri,
            IHttpTransport transport,
            JsonSerializerOptions? serializerOptions = null,
            IRequestSigner? requestSigner = null,
            IHttpPolicy? policy = null,
            IRestClientLogger? logger = null)
        {
            _baseUri = baseUri ?? throw new ArgumentNullException(nameof(baseUri));
            _transport = transport ?? throw new ArgumentNullException(nameof(transport));
            _serializerOptions = serializerOptions ?? new JsonSerializerOptions(JsonSerializerDefaults.Web);
            _requestSigner = requestSigner;
            _policy = policy ?? NoOpHttpPolicy.Instance;
            _logger = logger ?? NoOpRestClientLogger.Instance;
        }

        private static string? TryParseErrorCode(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return null;
            }

            try
            {
                using var doc = JsonDocument.Parse(content);
                var root = doc.RootElement;

                if (root.ValueKind == JsonValueKind.Object)
                {
                    if (root.TryGetProperty("error_message", out var msg) && msg.ValueKind == JsonValueKind.String)
                    {
                        return msg.GetString();
                    }

                    if (root.TryGetProperty("error_code", out var code) && code.ValueKind == JsonValueKind.String)
                    {
                        return code.GetString();
                    }
                }
            }
            catch (JsonException)
            {
                // ignore parse failures; return null
            }

            return null;
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
                    .ExecuteAsync(() => _transport.SendAsync(request, cancellationToken), cancellationToken)
                    .ConfigureAwait(false);

                var content = response.Content is null
                    ? string.Empty
                    : await response.Content
                        .ReadAsStringAsync(cancellationToken)
                        .ConfigureAwait(false);

                _logger.LogResponse(response, content);

                // ★ HTTP ステータス異常 → ExchangeApiException（E1）
                if (!response.IsSuccessStatusCode)
                {
                    var errorCode = TryParseErrorCode(content);

                    throw new ExchangeApiException(
                        $"Request to '{requestUri}' failed with status {(int)response.StatusCode} ({response.StatusCode}). Body: {content}",
                        exchangeId: null,
                        operation: null,
                        statusCode: response.StatusCode,
                        exchangeErrorCode: errorCode);
                }

                try
                {
                    var result = JsonSerializer.Deserialize<TResponse>(content, _serializerOptions);

                    if (result is null)
                    {
                        throw new ExchangeApiException(
                            $"Failed to deserialize response from '{requestUri}' as {typeof(TResponse).Name}.");
                    }

                    return result;
                }
                catch (JsonException ex)
                {
                    // ★ JSON パース失敗 → ExchangeApiException
                    throw new ExchangeApiException(
                        "Failed to deserialize JSON response.",
                        innerException: ex);
                }
            }
            catch (ExchangeApiException)
            {
                // すでにラップ済みのものはそのまま上に投げる
                throw;
            }
            catch (HttpRequestException ex)
            {
                // 通信エラー → ExchangeApiException に詳細を引き継ぐ
                var wrapped = new ExchangeApiException(
                    $"HTTP request failed for '{requestUri}'.",
                    statusCode: ex.StatusCode,
                    innerException: ex);
                _logger.LogError(wrapped, request);
                throw wrapped;
            }

            catch (TaskCanceledException ex)
            {
                // ★ タイムアウト or キャンセル → ExchangeApiException
                var wrapped = new ExchangeApiException(
                    "HTTP request timed out or was canceled.",
                    innerException: ex);
                _logger.LogError(wrapped, request);
                throw wrapped;
            }
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
                    .ExecuteAsync(() => _transport.SendAsync(request, cancellationToken), cancellationToken)
                    .ConfigureAwait(false);

                var content = response.Content is null
                    ? string.Empty
                    : await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

                _logger.LogResponse(response, content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorCode = TryParseErrorCode(content);

                    throw new ExchangeApiException(
                        $"Request to '{requestUri}' failed with status {(int)response.StatusCode} ({response.StatusCode}). Body: {content}",
                        exchangeId: null,
                        operation: null,
                        statusCode: response.StatusCode,
                        exchangeErrorCode: errorCode);
                }

                try
                {
                    var result = JsonSerializer.Deserialize<TResponse>(content, _serializerOptions);
                    if (result is null)
                    {
                        throw new ExchangeApiException(
                            $"Failed to deserialize response from '{requestUri}' as {typeof(TResponse).Name}.");
                    }

                    return result;
                }
                catch (JsonException ex)
                {
                    throw new ExchangeApiException(
                        "Failed to deserialize JSON response.",
                        innerException: ex);
                }
            }
            catch (ExchangeApiException)
            {
                throw;
            }
            catch (HttpRequestException ex)
            {
                var wrapped = new ExchangeApiException(
                    $"HTTP request failed for '{requestUri}'.",
                    statusCode: ex.StatusCode,
                    innerException: ex);
                _logger.LogError(wrapped, request);
                throw wrapped;
            }
            catch (TaskCanceledException ex)
            {
                var wrapped = new ExchangeApiException(
                    "HTTP request timed out or was canceled.",
                    innerException: ex);
                _logger.LogError(wrapped, request);
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

    }
}
