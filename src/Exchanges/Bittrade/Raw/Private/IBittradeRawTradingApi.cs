using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Raw;
using ExchangeApi.Exchanges.Bittrade.Raw.Call;
using ExchangeApi.Exchanges.Bittrade.Raw.Private;
using ExchangeApi.Exchanges.Bittrade.Raw.Private.Models;
using ExchangeApi.Exchanges.Bittrade.Raw.Public;
using ExchangeApi.Exchanges.Bittrade.Raw.Public.Models;
using ExchangeApi.Exchanges.Bittrade.Raw.Requests;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Private;

public interface IBittradeRawTradingApi
{
    Task<Call<CreateOrderRequest, RawPlaceOrderResponse>> CreateOrderAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<CancelOrderRequest, RawCancelOrderResponse>> CancelOrderAsync(
        CancelOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<CancelOrdersRequest, RawCancelOrdersResponse>> CancelOrdersAsync(
        CancelOrdersRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<CancelOpenOrdersRequest, RawCancelOpenOrdersResponse>> CancelOpenOrdersAsync(
        CancelOpenOrdersRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<CreateWithdrawRequest, RawCreateWithdrawResponse>> CreateWithdrawAsync(
        CreateWithdrawRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<CancelWithdrawRequest, RawCancelWithdrawResponse>> CancelWithdrawAsync(
        CancelWithdrawRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<CreateRetailOrderRequest, RawRetailOrderResponse>> CreateRetailOrderAsync(
        CreateRetailOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetOpenOrdersRequest, RawOpenOrdersResponse>> GetOpenOrdersAsync(
        GetOpenOrdersRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetOrderRequest, RawOrderDetailResponse>> GetOrderAsync(
        GetOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetMatchResultsRequest, RawMatchResultsResponse>> GetMatchResultsAsync(
        GetMatchResultsRequest request,
        CancellationToken cancellationToken = default);
}
