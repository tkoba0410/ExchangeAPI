using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Types;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Exchanges.Bittrade.Normalize;

namespace ExchangeApi.Exchanges.Bittrade.Normalize.Apis;

public interface IBittradeNormalizedTradingApi
{
    Task<OrderResult> PlaceOrderAsync(OrderRequest request, CancellationToken ct = default);

    Task<CancelResult> CancelOrderAsync(Symbol symbol, OrderKey orderKey, CancellationToken ct = default);

    Task<IReadOnlyList<OpenOrder>> GetOpenOrdersAsync(Symbol symbol, CancellationToken ct = default);

    Task<OrderStatus> GetOrderAsync(Symbol symbol, OrderKey orderKey, CancellationToken ct = default);

    Task<BittradeNormalizedCall<OrderResult, JsonElement>> PlaceOrderCallAsync(
        OrderRequest request,
        CancellationToken ct = default);

    Task<BittradeNormalizedCall<CancelResult, JsonElement>> CancelOrderCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken ct = default);

    Task<BittradeNormalizedCall<IReadOnlyList<OpenOrder>, JsonElement>> GetOpenOrdersCallAsync(
        Symbol symbol,
        CancellationToken ct = default);

    Task<BittradeNormalizedCall<OrderStatus, JsonElement>> GetOrderCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken ct = default);
}
