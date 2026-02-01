using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Utilities.Trading;

public static class OrderRequestFactory
{
    public static OrderRequest Market(Symbol symbol, Side side, Size size) =>
        new(symbol, side, OrderType.Market, size);

    public static OrderRequest Limit(Symbol symbol, Side side, Size size, Price price) =>
        new(symbol, side, OrderType.Limit, size, price);
}
