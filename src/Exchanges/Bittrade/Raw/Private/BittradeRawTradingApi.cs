using System;
using System.Threading;
using System.Threading.Tasks;

namespace ExchangeApi.Exchanges.Bittrade.Raw;

internal sealed class BittradeRawTradingApi : IBittradeRawTradingApi
{
    private readonly IBittradePrivateApi _privateApi;
    private readonly IBittradePrivateTradingApi _privateTradingApi;

    public BittradeRawTradingApi(IBittradePrivateApi privateApi, IBittradePrivateTradingApi privateTradingApi)
    {
        _privateApi = privateApi ?? throw new ArgumentNullException(nameof(privateApi));
        _privateTradingApi = privateTradingApi ?? throw new ArgumentNullException(nameof(privateTradingApi));
    }

    public Task<RawPlaceOrderResponse> CreateOrderAsync(RawCreateOrderRequest request, CancellationToken cancellationToken = default) =>
        _privateTradingApi.CreateOrderAsync(request, cancellationToken);

    public Task<RawCancelOrderResponse> CancelOrderAsync(RawOrderId orderId, CancellationToken cancellationToken = default) =>
        _privateTradingApi.CancelOrderAsync(orderId, cancellationToken);

    public Task<RawOpenOrdersResponse> GetOpenOrdersAsync(RawSymbol symbol, string accountId, CancellationToken cancellationToken = default) =>
        _privateApi.GetOpenOrdersAsync(symbol, accountId, cancellationToken);

    public Task<RawOrderDetailResponse> GetOrderAsync(RawOrderId orderId, CancellationToken cancellationToken = default) =>
        _privateApi.GetOrderAsync(orderId, cancellationToken);
}
