using System;
using Exchange.Bitflyer.Abstract.Apis.Account;
using Exchange.Bitflyer.Abstract.Apis.Market;
using Exchange.Bitflyer.Abstract.Apis.Trading;
using Exchange.Bitflyer.Raw.PrivateGet;
using Exchange.Bitflyer.Raw.PrivatePost;
using Exchange.Bitflyer.Raw.PublicGet;
using ExchangeApi.Transport.Protocol;

namespace Exchange.Bitflyer.Abstract.Facade;

/// <summary>
/// bitFlyer API 実装のセットをまとめるバンドル。
/// テスト向けにモック実装を差し替えやすくする。
/// </summary>
internal sealed class BitflyerApiBundle
{
    public IBitflyerPublicApi PublicApi { get; }
    public IBitflyerPrivateApi PrivateApi { get; }
    public IBitflyerPrivateTradingApi PrivateTradingApi { get; }

    public BitflyerApiBundle(
        IBitflyerPublicApi publicApi,
        IBitflyerPrivateApi privateApi,
        IBitflyerPrivateTradingApi privateTradingApi)
    {
        PublicApi = publicApi ?? throw new ArgumentNullException(nameof(publicApi));
        PrivateApi = privateApi ?? throw new ArgumentNullException(nameof(privateApi));
        PrivateTradingApi = privateTradingApi ?? throw new ArgumentNullException(nameof(privateTradingApi));
    }

    public static BitflyerApiBundle FromRestClient(IRestClient restClient)
    {
        if (restClient is null) throw new ArgumentNullException(nameof(restClient));
        var publicApi = new BitflyerPublicApi(restClient);
        var privateApi = new BitflyerPrivateApi(restClient);
        var privateTradingApi = new BitflyerPrivateTradingApi(restClient);
        return new BitflyerApiBundle(publicApi, privateApi, privateTradingApi);
    }
}
