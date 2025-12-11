using ExchangeApi.Adapter.Bitflyer.Factory;
using ExchangeApi.Adapter.Bittrade.Factory;
using ExchangeApi.Adapter.Bittrade.Facade;
using ExchangeApi.Adapter.Bitflyer.Facade;

namespace ExchangeApi.Factory;

/// <summary>
/// 複数取引所のクライアントをまとめた全部入りクライアントを生成するファクトリ。
/// </summary>
public static class MultiExchangeClientFactory
{
    /// <summary>
    /// bitFlyer/Bittrade の全部入りクライアントを作成する。
    /// </summary>
    public static MultiExchangeClient CreateDefault(
        string bitflyerApiKey,
        string bitflyerApiSecret,
        string bittradeAccessKey,
        string bittradeSecretKey,
        string bittradeAccountId)
    {
        BitflyerExchangeClient bf = BitflyerClientFactory.Create(bitflyerApiKey, bitflyerApiSecret);
        BittradeExchangeClient bt = BittradeClientFactory.CreateDefault(bittradeAccessKey, bittradeSecretKey, bittradeAccountId);
        return new MultiExchangeClient(bf, bt);
    }
}
