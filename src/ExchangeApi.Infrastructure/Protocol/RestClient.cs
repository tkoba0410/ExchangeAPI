using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using ExchangeApi.Abstractions.Errors;
using ExchangeApi.Infrastructure.Transport;

namespace ExchangeApi.Infrastructure.Protocol
{
    public class RestClient : IRestClient
    {
        private static readonly ProductInfoHeaderValue DefaultUserAgent =
        new("ExchangeApi", "1.0");
        private static readonly MediaTypeWithQualityHeaderValue JsonMediaType =
            new("application/json");
        private readonly Uri _baseUri;
        private readonly IHttpTransport _transport;

        public RestClient(Uri baseUri, IHttpTransport transport)
        {
            _baseUri = baseUri ?? throw new ArgumentNullException(nameof(baseUri));
            _transport = transport ?? throw new ArgumentNullException(nameof(transport));
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
                using var response = await _transport
                    .SendAsync(request, cancellationToken)
                    .ConfigureAwait(false);

                var content = await response.Content
                    .ReadAsStringAsync(cancellationToken)
                    .ConfigureAwait(false);

                // ★ HTTP ステータス異常 → ExchangeApiException（E1）
                if (!response.IsSuccessStatusCode)
                {
                    throw new ExchangeApiException(
                        $"Request to '{requestUri}' failed with status {(int)response.StatusCode} ({response.StatusCode}). Body: {content}",
                        exchangeId: null,
                        operation: null,
                        statusCode: response.StatusCode);
                }

                try
                {
                    var result = JsonSerializer.Deserialize<TResponse>(content);

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
                // ★ 通信エラー → ExchangeApiException
                throw new ExchangeApiException(
                    "HTTP request failed.",
                    innerException: ex);
            }
            catch (TaskCanceledException ex)
            {
                // ★ タイムアウト or キャンセル → ExchangeApiException
                throw new ExchangeApiException(
                    "HTTP request timed out or was canceled.",
                    innerException: ex);
            }
        }
        private Uri BuildUri(string path, IReadOnlyDictionary<string, string?>? query)
        {
            // path が "/v1/ticker" でも "v1/ticker" でも動くようにしておく
            var uriBuilder = new UriBuilder(new Uri(_baseUri, path));

            if (query is { Count: > 0 })
            {
                var queryString = string.Join(
                    "&",
                    query
                        .Where(kv => kv.Value is not null)
                        .Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value!)}"));

                uriBuilder.Query = queryString;
            }

            return uriBuilder.Uri;
        }
    }
}
