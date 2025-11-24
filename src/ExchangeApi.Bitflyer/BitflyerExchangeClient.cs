using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Abstractions.Contracts;
using ExchangeApi.Abstractions.Dtos;
using ExchangeApi.Abstractions.Errors;
using ExchangeApi.Bitflyer.Models;

namespace ExchangeApi.Bitflyer;

/// <summary>
/// bitFlyer 用の IExchangeClient 実装。
/// Stage1 では BTC/JPY の Ticker 取得のみをサポートする。
/// </summary>
public sealed class BitflyerExchangeClient : IExchangeClient
{
    private readonly IBitflyerPublicApi _publicApi;

    /// <summary>
    /// 取引所 ID（"bitFlyer" 固定）。
    /// </summary>
    public string ExchangeId { get; }

    /// <summary>
    /// アカウント ID。
    /// Stage1 ではシングルアカウント想定のため "default" 固定。
    /// </summary>
    public string AccountId { get; }

    /// <summary>
    /// 既定の exchangeId/accountId でクライアントを作成する。
    /// </summary>
    public BitflyerExchangeClient(IBitflyerPublicApi publicApi)
        : this(publicApi, exchangeId: "bitFlyer", accountId: "default")
    {
    }

    /// <summary>
    /// exchangeId / accountId を明示指定してクライアントを作成する。
    /// 将来のマルチ取引所・マルチアカウント対応を見据えたコンストラクタ。
    /// </summary>
    public BitflyerExchangeClient(
        IBitflyerPublicApi publicApi,
        string exchangeId,
        string accountId)
    {
        _publicApi = publicApi ?? throw new ArgumentNullException(nameof(publicApi));
        ExchangeId = exchangeId ?? throw new ArgumentNullException(nameof(exchangeId));
        AccountId  = accountId  ?? throw new ArgumentNullException(nameof(accountId));
    }

    /// <inheritdoc />
    public async Task<Ticker> GetTickerAsync(
        string symbol,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(symbol))
        {
            throw new ArgumentException("symbol must not be null or whitespace.", nameof(symbol));
        }

        // Stage1 では BTC/JPY のみサポート（詳細チェックは MapSymbolToProductCode に集約）
        var productCode = MapSymbolToProductCode(symbol);

        BitflyerTickerRaw raw;
        try
        {
            raw = await _publicApi
                .GetTickerRawAsync(productCode, cancellationToken)
                .ConfigureAwait(false);
        }
        catch (ExchangeApiException)
        {
            // すでに ExchangeApiException としてラップされているものはそのまま流す
            throw;
        }
        catch (Exception ex)
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

        // Stage1 では BTC/JPY 以外はサポートしない
        throw new SymbolNotSupportedException(symbol);
    }

    /// <summary>
    /// bitFlyer 生レスポンスから共通 Ticker DTO へマッピングする。
    /// </summary>
    private static Ticker MapToTicker(string symbol, BitflyerTickerRaw raw)
    {
        if (raw is null)
        {
            throw new ExchangeApiException("bitFlyer ticker response was null.");
        }

        // timestamp は string なので、ここで UTC に正規化
        if (!DateTimeOffset.TryParse(
                raw.Timestamp,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                out var dto))
        {
            throw new ExchangeApiException(
                $"Failed to parse bitFlyer timestamp: '{raw.Timestamp}'.");
        }

        var timestampUtc = dto.UtcDateTime;

        // Volume は Stage1 では未利用なので null のままにしておく。
        // 必要になったタイミングで BitflyerTickerRaw に対応プロパティを追加し、
        // ここでマッピングする。
        return new Ticker(
            symbol,
            raw.BestBid,
            raw.BestAsk,
            raw.LastTradedPrice,
            Volume: null,
            timestampUtc);
    }
}
