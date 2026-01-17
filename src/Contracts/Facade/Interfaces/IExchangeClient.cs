namespace ExchangeApi.Contracts.Facade.Interfaces;

public interface IExchangeClient
{
    IMarketDataApi Market { get; }
    ITradingApi Trading { get; }
    IAccountApi Account { get; }
    ISpotHistoryApi History { get; }
}
