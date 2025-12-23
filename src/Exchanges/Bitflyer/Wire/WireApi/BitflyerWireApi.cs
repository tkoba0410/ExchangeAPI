using System;
using ExchangeApi.Core.Transport.Protocol;
using ExchangeApi.Exchanges.Bitflyer.Wire.Private;
using ExchangeApi.Exchanges.Bitflyer.Wire.Public;
using Raw = ExchangeApi.Exchanges.Bitflyer.Raw;
namespace ExchangeApi.Exchanges.Bitflyer.Wire;

/// <summary>
/// bitFlyer の Wire API アクセス。正規化済みの詳細情報を返す用途向け。
/// </summary>
public sealed class BitflyerWireApi : IBitflyerWireApi
{
    public IBitflyerWireMarketDataApi MarketData { get; }
    public IBitflyerWireTradingApi Trading { get; }
    public IBitflyerWireAccountApi Account { get; }
    public IBitflyerWireExchangeInfoApi ExchangeInfo { get; }

    /// <summary>
    /// Wire API 用の RestClient を受け取って Wire API を生成します。
    /// </summary>
    /// <param name="restClient">
    /// 署名・認証・ポリシー設定済みの RestClient を渡してください。
    /// 署名や認証が不要な Public API のみを使う場合でも、呼び出し側で責務を分離します。
    /// </param>
    public BitflyerWireApi(IRestClient restClient)
        : this(
            raw: new Raw.BitflyerRawApi(restClient ?? throw new ArgumentNullException(nameof(restClient))),
            restClient: restClient)
    {
    }

    internal BitflyerWireApi(Raw.IBitflyerRawApi raw, IRestClient restClient)
        : this(raw, restClient, new BitflyerPrivateTradingApi(restClient))
    {
    }

    internal BitflyerWireApi(
        Raw.IBitflyerRawApi raw,
        IRestClient restClient,
        IBitflyerWireTradingApi tradingApi)
    {
        if (raw is null) throw new ArgumentNullException(nameof(raw));
        if (restClient is null) throw new ArgumentNullException(nameof(restClient));
        if (tradingApi is null) throw new ArgumentNullException(nameof(tradingApi));

        var publicApi = new BitflyerPublicApi(raw.MarketData);
        var privateApi = new BitflyerPrivateApi(restClient);

        MarketData = publicApi;
        Trading = tradingApi;
        Account = privateApi;
        ExchangeInfo = publicApi;
    }

    internal BitflyerWireApi(
        IBitflyerWireMarketDataApi marketData,
        IBitflyerWireTradingApi trading,
        IBitflyerWireAccountApi account,
        IBitflyerWireExchangeInfoApi exchangeInfo)
    {
        MarketData = marketData ?? throw new ArgumentNullException(nameof(marketData));
        Trading = trading ?? throw new ArgumentNullException(nameof(trading));
        Account = account ?? throw new ArgumentNullException(nameof(account));
        ExchangeInfo = exchangeInfo ?? throw new ArgumentNullException(nameof(exchangeInfo));
    }
}
