namespace ExchangeApi.Contracts.Interfaces;

public interface IHasRawAccess
{
    bool TryGetRaw<T>(out T raw) where T : class;
}

public interface IHasExchangeAccess
{
    bool TryGetExchange<T>(out T exchange) where T : class;
}
