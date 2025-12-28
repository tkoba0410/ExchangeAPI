using System.Threading;
using System.Threading.Tasks;

namespace ExchangeApi.Exchanges.Bittrade.Raw;

public interface IBittradeRawTradingApi
{
    Task<RawPlaceOrderResponse> CreateOrderAsync(RawCreateOrderRequest request, CancellationToken cancellationToken = default);

    Task<RawCancelOrderResponse> CancelOrderAsync(RawOrderId orderId, CancellationToken cancellationToken = default);

    Task<RawOpenOrdersResponse> GetOpenOrdersAsync(RawSymbol symbol, string accountId, CancellationToken cancellationToken = default);

    Task<RawOrderDetailResponse> GetOrderAsync(RawOrderId orderId, CancellationToken cancellationToken = default);
}
