using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Abstractions.Contracts;
using ExchangeApi.Abstractions.Dtos;
using ExchangeApi.Abstractions.Errors;
using ExchangeApi.Bitflyer.Models;

namespace ExchangeApi.Bitflyer;

/// <summary>
/// bitFlyer 用の IExchangeClient 実装。
/// Stage1 では Public GET（ticker）、Stage2 では Private GET（getbalance）をサポートする。
/// </summary>
public sealed class BitflyerExchangeClient : IExchangeClient
{
    private readonly IBitflyerPublicApi _publicApi;
    private readonly IBitflyerPrivateApi _privateApi;
    private readonly string _exchangeId;
    private readonly string _accountId;

    /// <summary>
    /// 取引所 ID（例: "bitFlyer"）。
    /// </summary>
    public string ExchangeId => _exchangeId;

    /// <summary>
    /// アカウント ID。Stage1/2 時点では固定 "default"。
    /// </summary>
    public string AccountId => _accountId;

    public BitflyerExchangeClient(
        IBitflyerPublicApi publicApi,
        IBitflyerPrivateApi privateApi)
        : this(publicApi, privateApi, exchangeId: "bitFlyer", accountId: "default")
    {
    }

    public BitflyerExchangeClient(
        IBitflyerPublicApi publicApi,
        IBitflyerPrivateApi privateApi,
        string exchangeId,
        string accountId)
    {
        _publicApi = publicApi ?? throw new ArgumentNullException(nameof(publicApi));
        _privateApi = privateApi ?? throw new ArgumentNullException(nameof(privateApi));
        _exchangeId = exchangeId ?? throw new ArgumentNullException(nameof(exchangeId));
        _accountId = accountId ?? throw new ArgumentNullException(nameof(accountId));
    }

    #region IExchangeMarketClient (ticker)

    /// <summary>
    /// 指定されたシンボルのティッカーを取得する。
    /// </summary>
    public async Task<Ticker> GetTickerAsync(
        string symbol,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // シンボル→bitFlyer product_code 変換
            var productCode = MapSymbolToProductCode(symbol);

            var rawTicker = await _publicApi
                .GetTickerRawAsync(productCode, cancellationToken)
                .ConfigureAwait(false);

            return MapToTicker(symbol, rawTicker);
        }
        catch (SymbolNotSupportedException ex)
        {
            // ドメインエラー：サポートしていないシンボル
            throw new ExchangeApiException(
                message: $"Symbol '{ex.Symbol}' is not supported by bitFlyer.",
                exchangeId: _exchangeId,
                operation: "GetTicker",
                statusCode: null,
                innerException: ex);
        }
        catch (ExchangeApiException)
        {
            // RestClient / Transport 層からの E1 例外はそのまま伝播
            throw;
        }
        catch (Exception ex)
        {
            // 予期しない例外は Adapter 層で文脈付き ExchangeApiException に統一
            throw new ExchangeApiException(
                message: "Failed to call bitFlyer getticker API.",
                exchangeId: _exchangeId,
                operation: "GetTicker",
                statusCode: null,
                innerException: ex);
        }
    }

    #endregion

    #region IExchangeAccountClient (balances)

    /// <summary>
    /// 口座残高一覧を取得する。
    /// Stage2 では /v1/me/getbalance のみを対象とする。
    /// </summary>
    public async Task<IReadOnlyList<Balance>> GetBalancesAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var rawBalances = await _privateApi
                .GetBalancesAsync(cancellationToken)
                .ConfigureAwait(false);

            var result = rawBalances
                .Select(b => new Balance(
                    b.CurrencyCode,
                    b.Amount,
                    b.Available))
                .ToArray();

            return result;
        }
        catch (ExchangeApiException)
        {
            // RestClient / Transport 層からの E1 例外はそのまま
            throw;
        }
        catch (Exception ex)
        {
            // 予期しない例外は文脈付きにラップ
            throw new ExchangeApiException(
                message: "Failed to call bitFlyer getbalance API.",
                exchangeId: _exchangeId,
                operation: "GetBalances",
                statusCode: null,
                innerException: ex);
        }
    }

    #endregion

    #region private mapping helpers

    /// <summary>
    /// 抽象シンボル（例: BTC/JPY）を bitFlyer の product_code にマップする。
    /// Stage1/2 時点では BTC/JPY のみサポート。
    /// </summary>
    private static string MapSymbolToProductCode(string symbol)
    {
        // Stage1/2 ではシンプルな静的マッピングのみ
        if (string.Equals(symbol, Symbols.BtcJpy, StringComparison.Ordinal))
        {
            return "BTC_JPY";
        }

        // Stage1/2 では BTC/JPY 以外はサポートしない
        throw new SymbolNotSupportedException(symbol);
    }

    /// <summary>
    /// bitFlyer の Raw ティッカーを抽象層の Ticker に変換する。
    /// </summary>
    private static Ticker MapToTicker(string symbol, BitflyerTickerRaw raw)
    {
        // ここは既存の実装に合わせて調整してください。
        // 例：Bid/Ask/Last/BestBidSize/BestAskSize/Volume 等のマッピング。

        return new Ticker(
            Symbol: symbol,
            BestBid: raw.BestBid,
            BestAsk: raw.BestAsk,
            LastTradedPrice: raw.LastTradedPrice,
            Timestamp: raw.Timestamp);
    }

    #endregion
}
