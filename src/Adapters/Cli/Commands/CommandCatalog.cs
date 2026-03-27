using ExchangeApi.Adapters.Cli.Commands.Binance.Native.Public;
using ExchangeApi.Adapters.Cli.Commands.Bitflyer.Native.Private;
using ExchangeApi.Adapters.Cli.Commands.Bitflyer.Native.Public;
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
        GetKlinesCommand.Create(),
        CancelAllChildOrdersCommand.Create(),
    ];
}
