using ExchangeApi.Adapters.Cli.Commands.Binance.Protocol.Public;
using ExchangeApi.Adapters.Cli.Commands.Binance.Native.Public;
using ExchangeApi.Adapters.Cli.Commands.Bitflyer.Native.Private;
using ExchangeApi.Adapters.Cli.Commands.Bitflyer.Native.Public;
using ExchangeApi.Adapters.Cli.Commands.Bitflyer.Protocol.Private;
using ExchangeApi.Adapters.Cli.Commands.Bitflyer.Protocol.Public;
using ExchangeApi.Adapters.Cli.Infrastructure;

namespace ExchangeApi.Adapters.Cli.Commands;

public static class CommandCatalog
{
    public static IReadOnlyList<CommandDescriptor> All { get; } =
    [
        GetMarketsCommand.Create(),
        GetBoardCommand.Create(),
        GetBoardStateCommand.Create(),
        GetHealthCommand.Create(),
        GetFundingRateCommand.Create(),
        GetCorporateLeverageCommand.Create(),
        GetChatsCommand.Create(),
        GetExecutionsPublicCommand.Create(),
        GetTickerCommand.Create(),
        GetMarketsProtocolCommand.Create(),
        GetBoardProtocolCommand.Create(),
        GetBoardStateProtocolCommand.Create(),
        GetHealthProtocolCommand.Create(),
        GetFundingRateProtocolCommand.Create(),
        GetCorporateLeverageProtocolCommand.Create(),
        GetChatsProtocolCommand.Create(),
        GetTickerProtocolCommand.Create(),
        GetExecutionsPublicProtocolCommand.Create(),
        GetPermissionsProtocolCommand.Create(),
        GetAddressesProtocolCommand.Create(),
        GetBalanceProtocolCommand.Create(),
        GetBalanceHistoryProtocolCommand.Create(),
        GetBankAccountsProtocolCommand.Create(),
        GetChildOrdersProtocolCommand.Create(),
        GetCoinInsProtocolCommand.Create(),
        GetCoinOutsProtocolCommand.Create(),
        GetCollateralProtocolCommand.Create(),
        GetCollateralAccountsProtocolCommand.Create(),
        GetCollateralHistoryProtocolCommand.Create(),
        GetDepositsProtocolCommand.Create(),
        GetExecutionsPrivateProtocolCommand.Create(),
        GetParentOrderProtocolCommand.Create(),
        GetParentOrdersProtocolCommand.Create(),
        GetPositionsProtocolCommand.Create(),
        GetTradingCommissionProtocolCommand.Create(),
        GetWithdrawalsProtocolCommand.Create(),
        CancelChildOrderCommand.Create(),
        GetPermissionsCommand.Create(),
        GetAddressesCommand.Create(),
        GetBalanceCommand.Create(),
        GetBalanceHistoryCommand.Create(),
        GetBankAccountsCommand.Create(),
        GetChildOrdersCommand.Create(),
        GetCoinInsCommand.Create(),
        GetCoinOutsCommand.Create(),
        GetCollateralCommand.Create(),
        GetCollateralAccountsCommand.Create(),
        GetCollateralHistoryCommand.Create(),
        GetDepositsCommand.Create(),
        GetExecutionsPrivateCommand.Create(),
        GetParentOrderCommand.Create(),
        GetParentOrdersCommand.Create(),
        GetPositionsCommand.Create(),
        CancelParentOrderCommand.Create(),
        SendChildOrderCommand.Create(),
        SendParentOrderCommand.Create(),
        GetTradingCommissionCommand.Create(),
        GetWithdrawalsCommand.Create(),
        WithdrawCommand.Create(),
        GetKlinesCommand.Create(),
        GetKlinesProtocolCommand.Create(),
        CancelAllChildOrdersCommand.Create(),
    ];
}
