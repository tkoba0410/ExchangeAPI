using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Contracts.Errors;

namespace ExchangeApi.Contracts.Extensions;

public static class ExchangeClientExtensions
{
    public static T Raw<T>(this IExchangeClient client) where T : class
    {
        if (client is IHasRawAccess rawAccess && rawAccess.TryGetRaw<T>(out var raw))
        {
            return raw;
        }

        throw new ExchangeFeatureNotSupportedException(
            client.ExchangeCode,
            $"RawApi:{typeof(T).Name}");
    }

    public static T As<T>(this IExchangeClient client) where T : class
    {
        if (client is T typed)
        {
            return typed;
        }

        throw new ExchangeFeatureNotSupportedException(
            client.ExchangeCode,
            $"Client:{typeof(T).Name}");
    }
}
