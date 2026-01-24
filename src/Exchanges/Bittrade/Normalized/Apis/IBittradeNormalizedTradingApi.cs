using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Exchanges.Bittrade.Normalized.Dtos.Trading;
using ExchangeApi.Exchanges.Bittrade.Normalized.Dtos;
using ExchangeApi.Exchanges.Bittrade.Normalized.Requests;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Apis;

internal interface IBittradeNormalizedTradingApi
{
    Task<Call<PlaceOrderRequest, BittradeOrderResult>> PlaceOrderCallAsync(
        BittradeOrderRequest request,
        CancellationToken ct = default);

    Task<Call<CancelOrderRequest, BittradeCancelResult>> CancelOrderCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken ct = default);

    Task<Call<GetOpenOrdersRequest, IReadOnlyList<BittradeOpenOrder>>> GetOpenOrdersCallAsync(
        Symbol symbol,
        CancellationToken ct = default);

    Task<Call<GetOrderRequest, BittradeOrderStatus>> GetOrdersByOrderIdCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken ct = default);

    Task<Call<GetAccountExecutionsRequest, IReadOnlyList<BittradeExecutionNormalized>>> GetMatchResultsCallAsync(
        Symbol symbol,
        int? limit = null,
        CancellationToken ct = default);
}
