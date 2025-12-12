using Common.Core.Contracts.Contracts;
using Exchange.Bitflyer.Facade;
using Exchange.Bittrade.Facade;

namespace Unified.Client;

/// <summary>
/// Exchange.* クライアントを束ね、Primary を設定で切り替えられる薄いファサード。
/// </summary>
public sealed class UnifiedClient : IUnifiedClient
{
    public UnifiedClient(
        BitflyerExchangeClient bitflyer,
        BittradeExchangeClient bittrade,
        PrimaryExchange primary = PrimaryExchange.Bitflyer)
    {
        Bitflyer = bitflyer ?? throw new ArgumentNullException(nameof(bitflyer));
        Bittrade = bittrade ?? throw new ArgumentNullException(nameof(bittrade));
        Primary = primary switch
        {
            PrimaryExchange.Bitflyer => (Market: (IMarketDataApi)bitflyer, Trading: bitflyer, Account: bitflyer, Margin: bitflyer),
            PrimaryExchange.Bittrade => (Market: (IMarketDataApi)bittrade, Trading: bittrade, Account: bittrade, Margin: bittrade),
            _ => throw new ArgumentOutOfRangeException(nameof(primary), primary, "Unsupported primary exchange."),
        };
    }

    public BitflyerExchangeClient Bitflyer { get; }
    public BittradeExchangeClient Bittrade { get; }

    public IMarketDataApi PrimaryMarket => Primary.Market;
    public ITradingApi PrimaryTrading => Primary.Trading;
    public IAccountApi PrimaryAccount => Primary.Account;
    public IMarginAccountApi PrimaryMargin => Primary.Margin;

    private (IMarketDataApi Market, ITradingApi Trading, IAccountApi Account, IMarginAccountApi Margin) Primary { get; }
}
