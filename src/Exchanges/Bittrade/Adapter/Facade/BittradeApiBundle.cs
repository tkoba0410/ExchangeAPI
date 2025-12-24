using System;
using ExchangeApi.Exchanges.Bittrade.Raw;
using ExchangeApi.Exchanges.Bittrade.Wire;
using ExchangeApi.Exchanges.Bittrade.Wire.Public;
using ExchangeApi.Exchanges.Bittrade.Wire.Private;
using ExchangeApi.Core.Transport.Protocol;

namespace ExchangeApi.Exchanges.Bittrade.Adapter.Facade;

/// <summary>
/// Bittrade API 実装のセットをまとめるバンドル。
/// テスト向けにモック実装を差し替えやすくする。
/// </summary>
internal sealed class BittradeApiBundle
{
    public IBittradeWireMarketDataApi MarketData { get; }
    public IBittradeWireTradingApi Trading { get; }
    public IRestClient RestClient { get; }
    public string? AccountId { get; }
    public object? RawBundle { get; }
    public object? WireBundle { get; }

    public BittradeApiBundle(
        IBittradeWireMarketDataApi marketData,
        IBittradeWireTradingApi trading,
        IRestClient restClient,
        string? accountId = null,
        object? rawBundle = null,
        object? wireBundle = null)
    {
        MarketData = marketData ?? throw new ArgumentNullException(nameof(marketData));
        Trading = trading ?? throw new ArgumentNullException(nameof(trading));
        RestClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
        AccountId = string.IsNullOrWhiteSpace(accountId) ? null : accountId;
        RawBundle = rawBundle;
        WireBundle = wireBundle;
    }

    public static BittradeApiBundle FromRestClient(IRestClient restClient, string? accountId = null)
    {
        if (restClient is null) throw new ArgumentNullException(nameof(restClient));
        var normalizedAccountId = string.IsNullOrWhiteSpace(accountId) ? null : accountId;
        var raw = new BittradeRawApi(restClient);
        var wireMarket = new BittradeWireMarketDataApi(raw.MarketData);
        var wireCommon = new BittradeWireCommonApi(raw);
        var wireTrading = normalizedAccountId is null
            ? (IBittradeWireTradingApi)new BittradeWireTradingApiNotSupported()
            : new BittradeWireTradingApi(raw.Trading, normalizedAccountId);
        var wire = new BittradeWireApi(wireMarket, wireTrading, wireCommon);
        return new BittradeApiBundle(
            marketData: wireMarket,
            trading: wireTrading,
            restClient: restClient,
            accountId: normalizedAccountId,
            rawBundle: raw,
            wireBundle: wire);
    }
}
