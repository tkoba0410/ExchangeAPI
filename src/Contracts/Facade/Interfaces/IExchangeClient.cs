namespace ExchangeApi.Contracts.Facade.Interfaces;

public interface IExchangeClient
{
    // Facade capability は nullable。未対応は null でのみ表現する。
    IPublicApi? Public { get; }
    IPrivateApi? Private { get; }
}
