using System;
using ExchangeApi.Core.Transport.Protocol;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire.Public;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire.Private;
namespace ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire;

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
    {
        if (restClient is null) throw new ArgumentNullException(nameof(restClient));

        var publicApi = new BitflyerPublicApi(restClient);
        var tradingApi = new Private.BitflyerWireTradingApi(restClient);
        var accountApi = new Private.BitflyerWireAccountApi(restClient);

        MarketData = publicApi;
        Trading = tradingApi;
        Account = accountApi;
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
