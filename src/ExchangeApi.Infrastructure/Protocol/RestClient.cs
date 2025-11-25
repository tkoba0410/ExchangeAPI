using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Abstractions.Errors;
using ExchangeApi.Infrastructure.Transport;

namespace ExchangeApi.Infrastructure.Protocol;

/// <summary>
/// <see cref="IHttpTransport"/> を用いて JSON ベースの REST API を呼び出す既定実装。
/// </summary>
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

            // ★ E1: HTTP ステータス異常 → ExchangeApiException
            if (!response.IsSuccessStatusCode)
            {
                // 必要に応じて body を短くするなど調整してください
                throw new ExchangeApiException(
                    $"Request to '{requestUri}' failed with status {(int)response.StatusCode} ({response.StatusCode}). Body: {content}");
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
                // ★ E1: JSON パース失敗 → ExchangeApiException
                throw new ExchangeApiException("Failed to deserialize JSON response.", ex);
            }
        }
        catch (HttpRequestException ex)
        {
            // ★ E1: 通信エラー → ExchangeApiException
            throw new ExchangeApiException("HTTP request failed.", ex);
        }
        catch (TaskCanceledException ex)
        {
            // ★ E1: タイムアウト or キャンセル → ExchangeApiException
            throw new ExchangeApiException("HTTP request timed out or was canceled.", ex);
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
