using System;
using ExchangeApi.Exchanges.Bittrade.Raw.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Raw.Public.Api;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Exchanges.Bittrade.Raw;

/// <summary>
/// Bittrade の Raw API アクセス（Public/Private/Trading をまとめた単一入口）。
/// </summary>
public sealed class BittradeRawApi : IBittradeRawApi
{
    private readonly IBittradePublicApi _publicApi;
    private readonly IBittradePrivateApi _privateApi;
    private readonly IBittradePrivateTradingApi _privateTradingApi;

    public IBittradeRawMarketDataApi MarketData { get; }
    public IBittradeRawTradingApi Trading { get; }
    public IBittradeRawExchangeInfoApi ExchangeInfo { get; }
    public IBittradeRawAccountApi Account { get; }

    public BittradeRawApi(IWireTransport wire)
        : this(
            publicApi: new BittradePublicApi(wire ?? throw new ArgumentNullException(nameof(wire))),
            privateApi: new BittradePrivateApi(wire),
            privateTradingApi: new BittradePrivateTradingApi(wire))
    {
    }

    internal BittradeRawApi(
        IBittradePublicApi publicApi,
        IBittradePrivateApi privateApi,
        IBittradePrivateTradingApi privateTradingApi)
    {
        _publicApi = publicApi ?? throw new ArgumentNullException(nameof(publicApi));
        _privateApi = privateApi ?? throw new ArgumentNullException(nameof(privateApi));
        _privateTradingApi = privateTradingApi ?? throw new ArgumentNullException(nameof(privateTradingApi));
        MarketData = new BittradeRawMarketDataApi(_publicApi);
        Trading = new BittradeRawTradingApi(_privateApi, _privateTradingApi);
        ExchangeInfo = new BittradeRawExchangeInfoApi(_publicApi);
        Account = new BittradeRawAccountApi(_privateApi);
    }
}
