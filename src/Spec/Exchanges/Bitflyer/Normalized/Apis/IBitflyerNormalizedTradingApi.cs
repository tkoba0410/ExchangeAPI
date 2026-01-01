using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Types;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Normalize;

namespace ExchangeApi.Exchanges.Bitflyer.Normalize.Apis;

public interface IBitflyerNormalizedTradingApi
{
    Task<OrderResult> PlaceOrderAsync(OrderRequest request, CancellationToken cancellationToken = default);

    Task<CancelResult> CancelOrderAsync(Symbol symbol, OrderKey orderKey, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<OpenOrder>> GetOpenOrdersAsync(Symbol symbol, CancellationToken cancellationToken = default);

    Task<OrderStatus> GetOrderAsync(Symbol symbol, OrderKey orderKey, CancellationToken cancellationToken = default);

    Task<BitflyerNormalizedCall<OrderResult, JsonElement>> PlaceOrderCallAsync(
        OrderRequest request,
        CancellationToken cancellationToken = default);

    Task<BitflyerNormalizedCall<CancelResult, JsonElement>> CancelOrderCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default);

    Task<BitflyerNormalizedCall<IReadOnlyList<OpenOrder>, JsonElement>> GetOpenOrdersCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default);

    Task<BitflyerNormalizedCall<OrderStatus, JsonElement>> GetOrderCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default);
}
