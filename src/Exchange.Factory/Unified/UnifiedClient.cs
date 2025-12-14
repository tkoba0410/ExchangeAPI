using Common.Contract.Interfaces;
using Common.Contract.Dtos;
using Common.Contract.Enums;
using Exchange.Bitflyer.Abstract;
using ExchangeApi.Adapter.Bittrade.Facade;

namespace ExchangeApi.Factory.Unified;

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
            PrimaryExchange.Bittrade => (Market: (IMarketDataApi)bittrade, Trading: bittrade, Account: bittrade, Margin: NotSupportedMarginAccountApi.Instance),
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

    private sealed class NotSupportedMarginAccountApi : IMarginAccountApi
    {
        public static readonly NotSupportedMarginAccountApi Instance = new();

        public Task<IReadOnlyList<Balance>> GetBalancesAsync(CancellationToken cancellationToken = default) =>
            Task.FromException<IReadOnlyList<Balance>>(new NotSupportedException("Account API not supported for this exchange."));

        public Task<IReadOnlyList<ExecutionAccount>> GetAccountExecutionsAsync(string productCode, CancellationToken cancellationToken = default) =>
            Task.FromException<IReadOnlyList<ExecutionAccount>>(new NotSupportedException("Account API not supported for this exchange."));

        public Task<IReadOnlyList<Position>> GetOpenPositionsAsync(string productCode, CancellationToken cancellationToken = default) =>
            Task.FromException<IReadOnlyList<Position>>(new NotSupportedException("Margin account API not supported for this exchange."));

        public Task<Collateral> GetCollateralAsync(CancellationToken cancellationToken = default) =>
            Task.FromException<Collateral>(new NotSupportedException("Margin account API not supported for this exchange."));
    }
}
