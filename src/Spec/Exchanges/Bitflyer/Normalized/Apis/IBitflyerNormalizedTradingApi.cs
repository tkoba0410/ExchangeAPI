using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Types;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Requests;
using ExchangeApi.Spec.CallCommon;

namespace ExchangeApi.Exchanges.Bitflyer.Normalize.Apis;

public interface IBitflyerNormalizedTradingApi
{
    Task<Call<PlaceOrderRequest, OrderResult>> PlaceOrderCallAsync(
        OrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<CancelOrderRequest, CancelResult>> CancelOrderCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default);

    Task<Call<GetOpenOrdersRequest, IReadOnlyList<OpenOrder>>> GetOpenOrdersCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default);

    Task<Call<GetOrderRequest, OrderStatus>> GetOrderCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default);

}
