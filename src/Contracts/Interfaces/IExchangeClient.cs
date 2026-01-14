using ExchangeApi.Contracts.Common.DomainCommon.Enums;

namespace ExchangeApi.Contracts.Interfaces;

public interface IExchangeClient
{
    IMarketDataApi Market { get; }
    ITradingApi Trading { get; }
    IAccountApi Account { get; }
    ISpotHistoryApi History { get; }
    ExchangeCode ExchangeCode { get; }
}
