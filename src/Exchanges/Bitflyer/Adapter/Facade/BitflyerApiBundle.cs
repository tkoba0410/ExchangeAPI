using System;
using ExchangeApi.Exchanges.Bitflyer.Wire;
using Raw = ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Core.Transport.Protocol;
namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Facade;

/// <summary>
/// bitFlyer API 実装のセットをまとめるバンドル。
/// テスト向けにモック実装を差し替えやすくする。
/// </summary>
internal sealed class BitflyerApiBundle
{
    public IBitflyerPublicApi PublicApi { get; }
    public IBitflyerPrivateApi PrivateApi { get; }
    public IBitflyerPrivateTradingApi PrivateTradingApi { get; }
    public object? RawBundle { get; }
    public object? WireBundle { get; }

    public BitflyerApiBundle(
        IBitflyerPublicApi publicApi,
        IBitflyerPrivateApi privateApi,
        IBitflyerPrivateTradingApi privateTradingApi,
        object? rawBundle = null,
        object? wireBundle = null)
    {
        PublicApi = publicApi ?? throw new ArgumentNullException(nameof(publicApi));
        PrivateApi = privateApi ?? throw new ArgumentNullException(nameof(privateApi));
        PrivateTradingApi = privateTradingApi ?? throw new ArgumentNullException(nameof(privateTradingApi));
        RawBundle = rawBundle;
        WireBundle = wireBundle;
    }

    public static BitflyerApiBundle FromRestClient(IRestClient restClient)
    {
        if (restClient is null) throw new ArgumentNullException(nameof(restClient));
        var raw = new Raw.BitflyerRawApi(restClient);
        var publicApi = new BitflyerPublicApi(raw.MarketData);
        var privateApi = new BitflyerPrivateApi(restClient);
        var privateTradingApi = new BitflyerPrivateTradingApi(restClient);
        var wire = new BitflyerWireApi(raw, restClient);
        return new BitflyerApiBundle(publicApi, privateApi, privateTradingApi, rawBundle: raw, wireBundle: wire);
    }
}
