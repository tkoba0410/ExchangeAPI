namespace ExchangeApi.Contracts.Facade.Interfaces;

public interface IExchangeClient
{
    // Facade capability は nullable。未対応は null でのみ表現する。
    IMarketDataApi? Market { get; }
    ITradingApi? Trading { get; }
    IAccountApi? Account { get; }
    ISpotHistoryApi? History { get; }
}
