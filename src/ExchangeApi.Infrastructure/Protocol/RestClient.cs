using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Abstractions.Errors;
using ExchangeApi.Infrastructure.Transport;

namespace ExchangeApi.Infrastructure.Protocol;

/// <summary>
/// <see cref="IHttpTransport"/> を用いて JSON ベースの REST API を呼び出す既定実装。
/// </summary>
public sealed class RestClient : IRestClient
{
    private readonly IHttpTransport _transport;
    private readonly Uri _baseUri;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// <see cref="RestClient"/> の新しいインスタンスを作成する。
    /// </summary>
    /// <param name="transport">HTTP トランスポート。</param>
    /// <param name="baseUri">API のベース URI（例: https://api.bitflyer.com/）。</param>
    /// <param name="jsonOptions">JSON シリアライザ オプション。省略時は既定値。</param>
    public RestClient(
        IHttpTransport transport,
        Uri baseUri,
        JsonSerializerOptions? jsonOptions = null)
    {
        _transport = transport ?? throw new ArgumentNullException(nameof(transport));
        _baseUri   = baseUri   ?? throw new ArgumentNullException(nameof(baseUri));

        _jsonOptions = jsonOptions ?? CreateDefaultJsonOptions();
    }

    /// <inheritdoc />
    public async Task<TResponse> GetAsync<TResponse>(
        string relativePath,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            throw new ArgumentException("relativePath must not be null or whitespace.", nameof(relativePath));
        }

        var requestUri = new Uri(_baseUri, relativePath);

        using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

        using var response = await _transport
            .SendAsync(request, cancellationToken)
            .ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            var snippet = await SafeReadBodySnippetAsync(response, cancellationToken)
                .ConfigureAwait(false);

            var message =
                $"Unexpected HTTP status code {(int)response.StatusCode} ({response.ReasonPhrase}) " +
                $"for {request.Method} {request.RequestUri}. " +
                (snippet is null ? string.Empty : $"Body snippet: {snippet}");

            throw new ExchangeApiException(message);
        }

        await using var stream = await response.Content
            .ReadAsStreamAsync(cancellationToken)
            .ConfigureAwait(false);

        var result = await JsonSerializer.DeserializeAsync<TResponse>(
            stream,
            _jsonOptions,
            cancellationToken).ConfigureAwait(false);

        if (result is null)
        {
            throw new ExchangeApiException(
                $"Failed to deserialize response body as {typeof(TResponse).Name}.");
        }

        return result;
    }

    private static JsonSerializerOptions CreateDefaultJsonOptions()
        => new()
        {
            PropertyNameCaseInsensitive = true
        };

    private static async Task<string?> SafeReadBodySnippetAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        try
        {
            var text = await response.Content
                .ReadAsStringAsync(cancellationToken)
                .ConfigureAwait(false);

            if (string.IsNullOrEmpty(text))
            {
                return null;
            }

            const int maxLen = 500;
            return text.Length <= maxLen ? text : text[..maxLen];
        }
        catch
        {
            // レスポンスボディが読めない場合はスニペット無しでよい
            return null;
        }
    }
}
