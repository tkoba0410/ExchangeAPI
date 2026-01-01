using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ExchangeApi.Exchanges.Bittrade.Raw;

public interface IBittradeRawTradingApi
{
    Task<RawPlaceOrderResponse> CreateOrderAsync(RawCreateOrderRequest request, CancellationToken cancellationToken = default);

    Task<RawCancelOrderResponse> CancelOrderAsync(RawOrderId orderId, CancellationToken cancellationToken = default);

    Task<RawOpenOrdersResponse> GetOpenOrdersAsync(RawSymbol symbol, string accountId, CancellationToken cancellationToken = default);

    Task<RawOrderDetailResponse> GetOrderAsync(RawOrderId orderId, CancellationToken cancellationToken = default);

    Task<BittradeRawCall<RawPlaceOrderResponse, JsonElement>> CreateOrderCallAsync(
        RawCreateOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<BittradeRawCall<RawCancelOrderResponse, JsonElement>> CancelOrderCallAsync(
        RawOrderId orderId,
        CancellationToken cancellationToken = default);

    Task<BittradeRawCall<RawOpenOrdersResponse, JsonElement>> GetOpenOrdersCallAsync(
        RawSymbol symbol,
        string accountId,
        CancellationToken cancellationToken = default);

    Task<BittradeRawCall<RawOrderDetailResponse, JsonElement>> GetOrderCallAsync(
        RawOrderId orderId,
        CancellationToken cancellationToken = default);
}
