using System.Text.Json;
using ExchangeApi.Bitflyer.Models;

namespace ExchangeApi.Bitflyer
{
    /// <summary>
    /// bitFlyer Public REST API のシンプルな実装。
    /// Stage1 では最小限の HTTP / JSON 処理のみ行う。
    /// </summary>
    public sealed class BitflyerPublicApi : IBitflyerPublicApi
    {
        private static readonly Uri DefaultBaseUri = new("https://api.bitflyer.com", UriKind.Absolute);

        private readonly HttpClient _httpClient;
        private readonly Uri _baseUri;
        private readonly JsonSerializerOptions _jsonOptions;

        public BitflyerPublicApi(HttpClient httpClient)
            : this(httpClient, DefaultBaseUri)
        {
        }

        public BitflyerPublicApi(HttpClient httpClient, Uri baseUri)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _baseUri = baseUri ?? throw new ArgumentNullException(nameof(baseUri));

            _jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        }

        public async Task<BitflyerTickerRaw> GetTickerRawAsync(
            string productCode,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(productCode))
            {
                throw new ArgumentException("productCode must not be null or whitespace.", nameof(productCode));
            }

            var uriBuilder = new UriBuilder(_baseUri)
            {
                Path = "/v1/getticker",
                Query = $"product_code={Uri.EscapeDataString(productCode)}"
            };

            using var request = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri);

            // User-Agent は明示設定（Stage1ポリシー）
            if (!request.Headers.UserAgent.TryParseAdd("ExchangeApi.Bitflyer/1.0"))
            {
                // 失敗しても致命的ではないので無視
            }

            using var response = await _httpClient.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken).ConfigureAwait(false);

            // 成功ステータスでなければ例外
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                // TODO: Stage1 で ExchangeApiException を定義したら差し替える
                throw new InvalidOperationException(
                    $"bitFlyer returned {(int)response.StatusCode} ({response.ReasonPhrase}). Content: {content}");
            }

            await using var stream = await response.Content
                .ReadAsStreamAsync(cancellationToken)
                .ConfigureAwait(false);

            var raw = await JsonSerializer.DeserializeAsync<BitflyerTickerRaw>(
                stream,
                _jsonOptions,
                cancellationToken).ConfigureAwait(false);

            if (raw is null)
            {
                // JSON パースエラー扱い
                throw new InvalidOperationException("Failed to deserialize bitFlyer ticker response.");
            }

            return raw;
        }
    }
}
