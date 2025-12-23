using System.Threading;
using System.Threading.Tasks;

namespace ExchangeApi.Exchanges.Bittrade.Raw;

public interface IBittradeRawTradingApi
{
    Task<PlaceOrderResponse> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken = default);

    Task<CancelOrderResponse> CancelOrderAsync(OrderId orderId, CancellationToken cancellationToken = default);

    Task<OpenOrdersResponse> GetOpenOrdersAsync(Symbol symbol, string accountId, CancellationToken cancellationToken = default);

    Task<OrderDetailResponse> GetOrderAsync(OrderId orderId, CancellationToken cancellationToken = default);
}
