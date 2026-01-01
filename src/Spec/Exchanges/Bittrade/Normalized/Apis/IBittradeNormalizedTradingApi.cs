using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Common.Types;

namespace ExchangeApi.Exchanges.Bittrade.Normalize.Apis;

public interface IBittradeNormalizedTradingApi
{
    Task<OrderResult> PlaceOrderAsync(OrderRequest request, CancellationToken ct = default);

    Task<CancelResult> CancelOrderAsync(Symbol symbol, OrderKey orderKey, CancellationToken ct = default);

    Task<IReadOnlyList<OpenOrder>> GetOpenOrdersAsync(Symbol symbol, CancellationToken ct = default);

    Task<OrderStatus> GetOrderAsync(Symbol symbol, OrderKey orderKey, CancellationToken ct = default);
}
