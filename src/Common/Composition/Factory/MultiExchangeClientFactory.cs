using Exchange.Bitflyer.Abstract;
using Exchange.Bittrade.Abstract.Factory;
using Exchange.Bittrade.Abstract.Facade;
namespace Common.Composition.Factory;

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

    /// <summary>
    /// bitFlyer/Bittrade の Public API だけをまとめた軽量クライアントを作成する。
    /// </summary>
    public static MultiExchangePublicClient CreatePublic()
    {
        BitflyerPublicClient bf = BitflyerClientFactory.CreatePublic();
        BittradePublicClient bt = BittradeClientFactory.CreatePublicClient();
        return new MultiExchangePublicClient(bf, bt);
    }
}
