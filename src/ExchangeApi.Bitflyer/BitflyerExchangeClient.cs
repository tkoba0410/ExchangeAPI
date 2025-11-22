using System.Globalization;
using ExchangeApi.Bitflyer.Models;

namespace ExchangeApi.Bitflyer
{
    /// <summary>
    /// bitFlyer 用の IExchangeClient 実装。
    /// Stage1 では BTC/JPY の Ticker 取得のみをサポートする。
    /// </summary>
    public sealed class BitflyerExchangeClient : IExchangeClient
    {
        private readonly IBitflyerPublicApi _publicApi;

        public BitflyerExchangeClient(IBitflyerPublicApi publicApi)
        {
            _publicApi = publicApi ?? throw new ArgumentNullException(nameof(publicApi));
        }

        public async Task<Ticker> GetTickerAsync(
            string symbol,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(symbol))
            {
                throw new ArgumentException("symbol must not be null or whitespace.", nameof(symbol));
            }

            // Stage1 では BTC/JPY のみサポート
            if (!string.Equals(symbol, Symbols.BtcJpy, StringComparison.Ordinal))
            {
                throw new ArgumentException(
                    $"Unsupported symbol: {symbol}. Only {Symbols.BtcJpy} is supported in Stage1.",
                    nameof(symbol));
            }

            var productCode = MapSymbolToProductCode(symbol);

            BitflyerTickerRaw raw;
            try
            {
                raw = await _publicApi
                    .GetTickerRawAsync(productCode, cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (Exception ex) when (ex is not ExchangeApiException && ex is not ArgumentException)
            {
                // 下位の通信エラーなどを ExchangeApiException にラップ
                throw new ExchangeApiException("Failed to call bitFlyer getticker API.", ex);
            }

            return MapToTicker(symbol, raw);
        }

        private static string MapSymbolToProductCode(string symbol)
        {
            // Stage1 ではシンプルな静的マッピングのみ
            if (string.Equals(symbol, Symbols.BtcJpy, StringComparison.Ordinal))
            {
                return "BTC_JPY";
            }

            // ここに来るのは想定外（上で弾いている）が念のため
            throw new ArgumentException($"Unsupported symbol: {symbol}.", nameof(symbol));
        }

        private static Ticker MapToTicker(string symbol, BitflyerTickerRaw raw)
        {
            if (raw is null)
            {
                throw new ExchangeApiException("bitFlyer ticker response was null.");
            }

            // timestamp は Raw では string なので、ここで UTC に正規化
            DateTime timestampUtc;
            if (!DateTimeOffset.TryParse(
                    raw.Timestamp,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                    out var dto))
            {
                throw new ExchangeApiException(
                    $"Failed to parse bitFlyer timestamp: '{raw.Timestamp}'.");
            }

            timestampUtc = dto.UtcDateTime;

            return new Ticker
            {
                Symbol = symbol,
                BestBid = raw.BestBid,
                BestAsk = raw.BestAsk,
                LastTradedPrice = raw.LastTradedPrice,
                TimestampUtc = timestampUtc
            };
        }
    }
}
