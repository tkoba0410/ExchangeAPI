using ExchangeApi.Common.Clients.Internal;
using ExchangeApi.Common.Interfaces;
using ExchangeApi.Core.Contracts.Errors;

namespace ExchangeApi.Common.Extensions;

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

    public static T Wire<T>(this IExchangeClient client) where T : class
    {
        if (client is IHasWireAccess wireAccess && wireAccess.TryGetWire<T>(out var wire))
        {
            return wire;
        }

        throw new ExchangeFeatureNotSupportedException(
            client.ExchangeCode,
            $"WireApi:{typeof(T).Name}");
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
