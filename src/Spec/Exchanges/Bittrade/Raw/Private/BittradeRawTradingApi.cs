using System;
using System.Text.Json;
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

    public Task<BittradeRawCall<RawPlaceOrderResponse, JsonElement>> CreateOrderCallAsync(
        RawCreateOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _privateTradingApi.CreateOrderCallAsync(request, cancellationToken);

    public Task<BittradeRawCall<RawCancelOrderResponse, JsonElement>> CancelOrderCallAsync(
        RawOrderId orderId,
        CancellationToken cancellationToken = default) =>
        _privateTradingApi.CancelOrderCallAsync(orderId, cancellationToken);

    public Task<BittradeRawCall<RawOpenOrdersResponse, JsonElement>> GetOpenOrdersCallAsync(
        RawSymbol symbol,
        string accountId,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetOpenOrdersCallAsync(symbol, accountId, cancellationToken);

    public Task<BittradeRawCall<RawOrderDetailResponse, JsonElement>> GetOrderCallAsync(
        RawOrderId orderId,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetOrderCallAsync(orderId, cancellationToken);
}
