using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Common.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Normalize.Apis;

public interface IBitflyerNormalizedTradingApi
{
    Task<OrderResult> PlaceOrderAsync(OrderRequest request, CancellationToken cancellationToken = default);

    Task<CancelResult> CancelOrderAsync(Symbol symbol, OrderKey orderKey, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<OpenOrder>> GetOpenOrdersAsync(Symbol symbol, CancellationToken cancellationToken = default);

    Task<OrderStatus> GetOrderAsync(Symbol symbol, OrderKey orderKey, CancellationToken cancellationToken = default);
}
