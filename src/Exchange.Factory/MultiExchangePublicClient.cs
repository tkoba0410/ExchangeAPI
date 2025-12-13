using Exchange.Bitflyer.Abstract;
using ExchangeApi.Adapter.Bittrade.Facade;

namespace ExchangeApi.Factory;

/// <summary>
/// 複数取引所の Public クライアントをまとめて保持する軽量クライアント。
/// </summary>
public sealed class MultiExchangePublicClient
{
    public MultiExchangePublicClient(BitflyerPublicClient bitflyer, BittradePublicClient bittrade)
    {
        Bitflyer = bitflyer;
        Bittrade = bittrade;
    }

    public BitflyerPublicClient Bitflyer { get; }
    public BittradePublicClient Bittrade { get; }
}
