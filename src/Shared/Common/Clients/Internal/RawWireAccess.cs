namespace ExchangeApi.Common.Clients.Internal;

internal interface IHasRawAccess
{
    bool TryGetRaw<T>(out T raw) where T : class;
}

internal interface IHasWireAccess
{
    bool TryGetWire<T>(out T wire) where T : class;
}
