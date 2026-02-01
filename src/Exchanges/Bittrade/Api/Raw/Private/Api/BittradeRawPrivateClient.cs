using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Api.Raw.Private.Dtos;
using ExchangeApi.Exchanges.Bittrade.Api.Raw.Private.Requests;
using ExchangeApi.Exchanges.Bittrade.Api.Wire.Private.Endpoints;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Exchanges.Bittrade.Api.Raw.Api;

namespace ExchangeApi.Exchanges.Bittrade.Api.Raw.Private.Api;

/// <summary>
/// Bittrade Private REST API の Raw 実装。
/// </summary>
internal sealed class BittradeRawPrivateClient
{
    private readonly BittradeRawCallExecutor _executor;

    public BittradeRawPrivateClient(BittradeRawCallExecutor executor)
    {
        _executor = executor ?? throw new ArgumentNullException(nameof(executor));
    }

    public Task<Call<GetAccountsRequest, RawAccountsResponse>> GetAccountsCallAsync(
        GetAccountsRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bittrade.GetAccounts",
            BittradePrivateEndpoints.GetAccounts(),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawAccountsResponse>(json, "Bittrade.GetAccounts"));

    public Task<Call<GetAccountBalanceRequest, RawBalancesResponse>> GetAccountsBalanceByAccountIdCallAsync(
        GetAccountBalanceRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bittrade.GetAccountsBalanceByAccountId",
            BittradePrivateEndpoints.GetAccountsBalanceByAccountId(request.AccountId),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawBalancesResponse>(
                json,
                "Bittrade.GetAccountsBalanceByAccountId"));

    public Task<Call<GetOpenOrdersRequest, RawOpenOrdersResponse>> GetOpenOrdersCallAsync(
        GetOpenOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bittrade.GetOpenOrders",
            BittradePrivateEndpoints.GetOpenOrders(request.Symbol, request.AccountId),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawOpenOrdersResponse>(
                json,
                "Bittrade.GetOpenOrders"));

    public Task<Call<GetOrdersRequest, RawOrdersResponse>> GetOrdersCallAsync(
        GetOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bittrade.GetOrders",
            BittradePrivateEndpoints.GetOrders(),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawOrdersResponse>(json, "Bittrade.GetOrders"));

    public Task<Call<GetOrderRequest, RawOrderDetailResponse>> GetOrdersByOrderIdCallAsync(
        GetOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bittrade.GetOrdersByOrderId",
            BittradePrivateEndpoints.GetOrdersByOrderId(request.OrderId),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawOrderDetailResponse>(json, "Bittrade.GetOrdersByOrderId"));

    public Task<Call<GetOrderMatchResultsRequest, RawOrderMatchResultsResponse>> GetOrdersMatchResultsByOrderIdCallAsync(
        GetOrderMatchResultsRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bittrade.GetOrdersMatchResultsByOrderId",
            BittradePrivateEndpoints.GetOrdersMatchResultsByOrderId(request.OrderId),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawOrderMatchResultsResponse>(
                json,
                "Bittrade.GetOrdersMatchResultsByOrderId"));

    public Task<Call<GetMatchResultsRequest, RawMatchResultsResponse>> GetMatchResultsCallAsync(
        GetMatchResultsRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bittrade.GetMatchResults",
            BittradePrivateEndpoints.GetMatchResults(
                request.Symbol,
                request.Types,
                request.StartDate,
                request.EndDate,
                request.From?.ToString(CultureInfo.InvariantCulture),
                request.Direct,
                request.Size?.ToString(CultureInfo.InvariantCulture)),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawMatchResultsResponse>(
                json,
                "Bittrade.GetMatchResults"));

    public Task<Call<GetDepositWithdrawsRequest, RawDepositWithdrawsResponse>> GetDepositWithdrawCallAsync(
        GetDepositWithdrawsRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bittrade.GetDepositWithdraw",
            BittradePrivateEndpoints.GetDepositWithdraw(
                request.Type,
                request.Currency,
                request.From?.ToString(CultureInfo.InvariantCulture),
                request.Size?.ToString(CultureInfo.InvariantCulture),
                request.Direct),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawDepositWithdrawsResponse>(
                json,
                "Bittrade.GetDepositWithdraw"));

    public Task<Call<GetWithdrawVirtualAddressesRequest, RawWithdrawVirtualAddressesResponse>> GetWithdrawVirtualAddressesCallAsync(
        GetWithdrawVirtualAddressesRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bittrade.GetWithdrawVirtualAddresses",
            BittradePrivateEndpoints.GetWithdrawVirtualAddresses(),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawWithdrawVirtualAddressesResponse>(
                json,
                "Bittrade.GetWithdrawVirtualAddresses"));

    public Task<Call<GetRetailOrdersRequest, RawRetailOrdersResponse>> GetRetailOrderListCallAsync(
        GetRetailOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bittrade.GetRetailOrderList",
            BittradePrivateEndpoints.GetRetailOrderList(
                request.Direct.ToString(CultureInfo.InvariantCulture),
                request.Status?.ToString(CultureInfo.InvariantCulture),
                request.StartTime?.ToUnixTimeMilliseconds().ToString(CultureInfo.InvariantCulture),
                request.EndTime?.ToUnixTimeMilliseconds().ToString(CultureInfo.InvariantCulture)),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawRetailOrdersResponse>(
                json,
                "Bittrade.GetRetailOrderList"));

    public Task<Call<GetRetailOrderDetailByOrderIdRequest, RawRetailOrderDetailResponse>> GetRetailOrderDetailByOrderIdCallAsync(
        GetRetailOrderDetailByOrderIdRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bittrade.PostRetailOrderDetail",
            BittradePrivateEndpoints.GetRetailOrderDetailByOrderId(request.OrderId),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawRetailOrderDetailResponse>(
                json,
                "Bittrade.PostRetailOrderDetail"));

    public Task<Call<GetRetailAccountBalanceRequest, RawRetailAccountBalanceResponse>> GetRetailAccountBalanceCallAsync(
        GetRetailAccountBalanceRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bittrade.GetRetailAccountBalance",
            BittradePrivateEndpoints.GetRetailAccountBalance(),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawRetailAccountBalanceResponse>(
                json,
                "Bittrade.GetRetailAccountBalance"));

    public Task<Call<CreateOrderRequest, RawPlaceOrderResponse>> PostOrdersPlaceCallAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bittrade.PostOrdersPlace",
            BittradePrivateEndpoints.PostOrdersPlace(
                BittradeRawJson.SerializeOrThrow(request.Body, "Bittrade.PostOrdersPlace")),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawPlaceOrderResponse>(json, "Bittrade.PostOrdersPlace"));

    public Task<Call<CancelOrderRequest, RawCancelOrderResponse>> PostOrdersSubmitCancelByOrderIdCallAsync(
        CancelOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bittrade.PostOrdersSubmitCancelByOrderId",
            BittradePrivateEndpoints.PostOrdersSubmitCancelByOrderId(request.OrderId),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawCancelOrderResponse>(json, "Bittrade.PostOrdersSubmitCancelByOrderId"));

    public Task<Call<CancelOrdersRequest, RawCancelOrdersResponse>> PostOrdersBatchCancelCallAsync(
        CancelOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bittrade.PostOrdersBatchCancel",
            BittradePrivateEndpoints.PostOrdersBatchCancel(
                BittradeRawJson.SerializeOrThrow(request.Body, "Bittrade.PostOrdersBatchCancel")),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawCancelOrdersResponse>(
                json,
                "Bittrade.PostOrdersBatchCancel"));

    public Task<Call<CancelOpenOrdersRequest, RawCancelOpenOrdersResponse>> PostOrdersBatchCancelOpenOrdersCallAsync(
        CancelOpenOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bittrade.PostOrdersBatchCancelOpenOrders",
            BittradePrivateEndpoints.PostOrdersBatchCancelOpenOrders(
                BittradeRawJson.SerializeOrThrow(request.Body, "Bittrade.PostOrdersBatchCancelOpenOrders")),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawCancelOpenOrdersResponse>(
                json,
                "Bittrade.PostOrdersBatchCancelOpenOrders"));

    public Task<Call<CreateWithdrawRequest, RawCreateWithdrawResponse>> PostWithdrawApiCreateCallAsync(
        CreateWithdrawRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bittrade.PostWithdrawApiCreate",
            BittradePrivateEndpoints.PostWithdrawApiCreate(
                BittradeRawJson.SerializeOrThrow(request.Body, "Bittrade.PostWithdrawApiCreate")),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawCreateWithdrawResponse>(
                json,
                "Bittrade.PostWithdrawApiCreate"));

    public Task<Call<CreateWithdrawVirtualByAddressIdRequest, RawCreateWithdrawResponse>> PostWithdrawVirtualByAddressIdCreateCallAsync(
        CreateWithdrawVirtualByAddressIdRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bittrade.PostWithdrawApiCreateByAddressId",
            BittradePrivateEndpoints.PostWithdrawVirtualByAddressIdCreate(request.AddressId),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawCreateWithdrawResponse>(
                json,
                "Bittrade.PostWithdrawApiCreateByAddressId"));

    public Task<Call<CancelWithdrawRequest, RawCancelWithdrawResponse>> PostWithdrawVirtualByWithdrawIdCancelCallAsync(
        CancelWithdrawRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bittrade.PostWithdrawVirtualByWithdrawIdCancel",
            BittradePrivateEndpoints.PostWithdrawVirtualByWithdrawIdCancel(request.WithdrawId),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawCancelWithdrawResponse>(
                json,
                "Bittrade.PostWithdrawVirtualByWithdrawIdCancel"));

    public Task<Call<PlaceWithdrawVirtualRequest, RawCreateWithdrawResponse>> PostWithdrawVirtualByWithdrawIdPlaceCallAsync(
        PlaceWithdrawVirtualRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bittrade.PostWithdrawVirtualByWithdrawIdPlace",
            BittradePrivateEndpoints.PostWithdrawVirtualByWithdrawIdPlace(request.WithdrawId),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawCreateWithdrawResponse>(
                json,
                "Bittrade.PostWithdrawVirtualByWithdrawIdPlace"));

    public Task<Call<CreateRetailOrderRequest, RawRetailOrderResponse>> PostRetailOrderPlaceCallAsync(
        CreateRetailOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bittrade.PostRetailOrderPlace",
            BittradePrivateEndpoints.PostRetailOrderPlace(
                BittradeRawJson.SerializeOrThrow(request.Body, "Bittrade.PostRetailOrderPlace")),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawRetailOrderResponse>(
                json,
                "Bittrade.PostRetailOrderPlace"));

    public Task<Call<CancelRetailOrderRequest, RawRetailOrderResponse>> PostRetailOrderCancelByOrderIdCallAsync(
        CancelRetailOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bittrade.PostRetailOrderCancelByOrderId",
            BittradePrivateEndpoints.PostRetailOrderCancelByOrderId(request.OrderId),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawRetailOrderResponse>(
                json,
                "Bittrade.PostRetailOrderCancelByOrderId"));

    public Task<Call<PostRetailOrderHistoryRequest, RawRetailOrdersResponse>> PostRetailOrderHistoryCallAsync(
        PostRetailOrderHistoryRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bittrade.PostRetailOrderHistory",
            BittradePrivateEndpoints.PostRetailOrderHistory(
                BittradeRawJson.SerializeOrThrow(request.Body, "Bittrade.PostRetailOrderHistory")),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawRetailOrdersResponse>(
                json,
                "Bittrade.PostRetailOrderHistory"));

    public Task<Call<PostRetailOrderDetailRequest, RawRetailOrderDetailResponse>> PostRetailOrderDetailCallAsync(
        PostRetailOrderDetailRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bittrade.PostRetailOrderDetail",
            BittradePrivateEndpoints.PostRetailOrderDetail(
                BittradeRawJson.SerializeOrThrow(request.Body, "Bittrade.PostRetailOrderDetail")),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawRetailOrderDetailResponse>(
                json,
                "Bittrade.PostRetailOrderDetail"));

    public Task<Call<CreateRetailOrderRequest, RawRetailOrderResponse>> PostRetailOrderCreateCallAsync(
        CreateRetailOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bittrade.PostRetailOrderCreate",
            BittradePrivateEndpoints.PostRetailOrderCreate(
                BittradeRawJson.SerializeOrThrow(request.Body, "Bittrade.PostRetailOrderCreate")),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawRetailOrderResponse>(
                json,
                "Bittrade.PostRetailOrderCreate"));
}
