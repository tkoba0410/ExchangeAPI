using Exchange.Bitflyer.Abstract;
using Exchange.Bittrade.Abstract.Facade;
namespace Core.Composition.Factory;

/// <summary>
/// 複数取引所のクライアントをまとめて保持する全部入りクライアント。
/// </summary>
public sealed class MultiExchangeClient
{
    public MultiExchangeClient(BitflyerExchangeClient bitflyer, BittradeExchangeClient bittrade)
    {
        Bitflyer = bitflyer;
        Bittrade = bittrade;
    }

    public BitflyerExchangeClient Bitflyer { get; }
    public BittradeExchangeClient Bittrade { get; }
}
