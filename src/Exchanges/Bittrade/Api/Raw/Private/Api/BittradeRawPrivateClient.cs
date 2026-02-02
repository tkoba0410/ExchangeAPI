using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Api.Raw.Private.Dtos;
using ExchangeApi.Exchanges.Bittrade.Api.Raw.Private.Requests;
using ExchangeApi.Exchanges.Bittrade.Api.Wire.Constants;
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
            Component(BittradeEndpointIds.GetAccounts),
            BittradePrivateEndpoints.GetAccounts(),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawAccountsResponse>(json, Component(BittradeEndpointIds.GetAccounts)));

    public Task<Call<GetAccountBalanceRequest, RawBalancesResponse>> GetAccountsBalanceByAccountIdCallAsync(
        GetAccountBalanceRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            Component(BittradeEndpointIds.GetAccountsBalanceByAccountId),
            BittradePrivateEndpoints.GetAccountsBalanceByAccountId(request.AccountId),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawBalancesResponse>(
                json,
                Component(BittradeEndpointIds.GetAccountsBalanceByAccountId)));

    public Task<Call<GetOpenOrdersRequest, RawOpenOrdersResponse>> GetOpenOrdersCallAsync(
        GetOpenOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            Component(BittradeEndpointIds.GetOpenOrders),
            BittradePrivateEndpoints.GetOpenOrders(request.Symbol, request.AccountId),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawOpenOrdersResponse>(
                json,
                Component(BittradeEndpointIds.GetOpenOrders)));

    public Task<Call<GetOrdersRequest, RawOrdersResponse>> GetOrdersCallAsync(
        GetOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            Component(BittradeEndpointIds.GetOrders),
            BittradePrivateEndpoints.GetOrders(),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawOrdersResponse>(json, Component(BittradeEndpointIds.GetOrders)));

    public Task<Call<GetOrderRequest, RawOrderDetailResponse>> GetOrdersByOrderIdCallAsync(
        GetOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            Component(BittradeEndpointIds.GetOrdersByOrderId),
            BittradePrivateEndpoints.GetOrdersByOrderId(request.OrderId),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawOrderDetailResponse>(json, Component(BittradeEndpointIds.GetOrdersByOrderId)));

    public Task<Call<GetOrderMatchResultsRequest, RawOrderMatchResultsResponse>> GetOrdersMatchResultsByOrderIdCallAsync(
        GetOrderMatchResultsRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            Component(BittradeEndpointIds.GetOrdersMatchResultsByOrderId),
            BittradePrivateEndpoints.GetOrdersMatchResultsByOrderId(request.OrderId),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawOrderMatchResultsResponse>(
                json,
                Component(BittradeEndpointIds.GetOrdersMatchResultsByOrderId)));

    public Task<Call<GetMatchResultsRequest, RawMatchResultsResponse>> GetMatchResultsCallAsync(
        GetMatchResultsRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            Component(BittradeEndpointIds.GetMatchResults),
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
                Component(BittradeEndpointIds.GetMatchResults)));

    public Task<Call<GetDepositWithdrawsRequest, RawDepositWithdrawsResponse>> GetDepositWithdrawCallAsync(
        GetDepositWithdrawsRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            Component(BittradeEndpointIds.GetDepositWithdraw),
            BittradePrivateEndpoints.GetDepositWithdraw(
                request.Type,
                request.Currency,
                request.From?.ToString(CultureInfo.InvariantCulture),
                request.Size?.ToString(CultureInfo.InvariantCulture),
                request.Direct),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawDepositWithdrawsResponse>(
                json,
                Component(BittradeEndpointIds.GetDepositWithdraw)));

    public Task<Call<GetWithdrawVirtualAddressesRequest, RawWithdrawVirtualAddressesResponse>> GetWithdrawVirtualAddressesCallAsync(
        GetWithdrawVirtualAddressesRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            Component(BittradeEndpointIds.GetWithdrawVirtualAddresses),
            BittradePrivateEndpoints.GetWithdrawVirtualAddresses(),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawWithdrawVirtualAddressesResponse>(
                json,
                Component(BittradeEndpointIds.GetWithdrawVirtualAddresses)));

    public Task<Call<GetRetailOrdersRequest, RawRetailOrdersResponse>> GetRetailOrderListCallAsync(
        GetRetailOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            Component(BittradeEndpointIds.GetRetailOrderList),
            BittradePrivateEndpoints.GetRetailOrderList(
                request.Direct.ToString(CultureInfo.InvariantCulture),
                request.Status?.ToString(CultureInfo.InvariantCulture),
                request.StartTime?.ToUnixTimeMilliseconds().ToString(CultureInfo.InvariantCulture),
                request.EndTime?.ToUnixTimeMilliseconds().ToString(CultureInfo.InvariantCulture)),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawRetailOrdersResponse>(
                json,
                Component(BittradeEndpointIds.GetRetailOrderList)));

    public Task<Call<GetRetailOrderDetailByOrderIdRequest, RawRetailOrderDetailResponse>> GetRetailOrderDetailByOrderIdCallAsync(
        GetRetailOrderDetailByOrderIdRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            Component(BittradeEndpointIds.PostRetailOrderDetail),
            BittradePrivateEndpoints.GetRetailOrderDetailByOrderId(request.OrderId),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawRetailOrderDetailResponse>(
                json,
                Component(BittradeEndpointIds.PostRetailOrderDetail)));

    public Task<Call<GetRetailAccountBalanceRequest, RawRetailAccountBalanceResponse>> GetRetailAccountBalanceCallAsync(
        GetRetailAccountBalanceRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            Component(BittradeEndpointIds.GetRetailAccountBalance),
            BittradePrivateEndpoints.GetRetailAccountBalance(),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawRetailAccountBalanceResponse>(
                json,
                Component(BittradeEndpointIds.GetRetailAccountBalance)));

    public Task<Call<CreateOrderRequest, RawPlaceOrderResponse>> PostOrdersPlaceCallAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            Component(BittradeEndpointIds.PostOrdersPlace),
            BittradePrivateEndpoints.PostOrdersPlace(
                BittradeRawJson.SerializeOrThrow(request.Body, Component(BittradeEndpointIds.PostOrdersPlace))),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawPlaceOrderResponse>(json, Component(BittradeEndpointIds.PostOrdersPlace)));

    public Task<Call<CancelOrderRequest, RawCancelOrderResponse>> PostOrdersSubmitCancelByOrderIdCallAsync(
        CancelOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            Component(BittradeEndpointIds.PostOrdersSubmitCancelByOrderId),
            BittradePrivateEndpoints.PostOrdersSubmitCancelByOrderId(request.OrderId),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawCancelOrderResponse>(json, Component(BittradeEndpointIds.PostOrdersSubmitCancelByOrderId)));

    public Task<Call<CancelOrdersRequest, RawCancelOrdersResponse>> PostOrdersBatchCancelCallAsync(
        CancelOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            Component(BittradeEndpointIds.PostOrdersBatchCancel),
            BittradePrivateEndpoints.PostOrdersBatchCancel(
                BittradeRawJson.SerializeOrThrow(request.Body, Component(BittradeEndpointIds.PostOrdersBatchCancel))),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawCancelOrdersResponse>(
                json,
                Component(BittradeEndpointIds.PostOrdersBatchCancel)));

    public Task<Call<CancelOpenOrdersRequest, RawCancelOpenOrdersResponse>> PostOrdersBatchCancelOpenOrdersCallAsync(
        CancelOpenOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            Component(BittradeEndpointIds.PostOrdersBatchCancelOpenOrders),
            BittradePrivateEndpoints.PostOrdersBatchCancelOpenOrders(
                BittradeRawJson.SerializeOrThrow(request.Body, Component(BittradeEndpointIds.PostOrdersBatchCancelOpenOrders))),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawCancelOpenOrdersResponse>(
                json,
                Component(BittradeEndpointIds.PostOrdersBatchCancelOpenOrders)));

    public Task<Call<CreateWithdrawRequest, RawCreateWithdrawResponse>> PostWithdrawApiCreateCallAsync(
        CreateWithdrawRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            Component(BittradeEndpointIds.PostWithdrawApiCreate),
            BittradePrivateEndpoints.PostWithdrawApiCreate(
                BittradeRawJson.SerializeOrThrow(request.Body, Component(BittradeEndpointIds.PostWithdrawApiCreate))),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawCreateWithdrawResponse>(
                json,
                Component(BittradeEndpointIds.PostWithdrawApiCreate)));

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
            Component(BittradeEndpointIds.PostWithdrawVirtualByWithdrawIdCancel),
            BittradePrivateEndpoints.PostWithdrawVirtualByWithdrawIdCancel(request.WithdrawId),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawCancelWithdrawResponse>(
                json,
                Component(BittradeEndpointIds.PostWithdrawVirtualByWithdrawIdCancel)));

    public Task<Call<PlaceWithdrawVirtualRequest, RawCreateWithdrawResponse>> PostWithdrawVirtualByWithdrawIdPlaceCallAsync(
        PlaceWithdrawVirtualRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            Component(BittradeEndpointIds.PostWithdrawVirtualByWithdrawIdPlace),
            BittradePrivateEndpoints.PostWithdrawVirtualByWithdrawIdPlace(request.WithdrawId),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawCreateWithdrawResponse>(
                json,
                Component(BittradeEndpointIds.PostWithdrawVirtualByWithdrawIdPlace)));

    public Task<Call<CreateRetailOrderRequest, RawRetailOrderResponse>> PostRetailOrderPlaceCallAsync(
        CreateRetailOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            Component(BittradeEndpointIds.PostRetailOrderPlace),
            BittradePrivateEndpoints.PostRetailOrderPlace(
                BittradeRawJson.SerializeOrThrow(request.Body, Component(BittradeEndpointIds.PostRetailOrderPlace))),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawRetailOrderResponse>(
                json,
                Component(BittradeEndpointIds.PostRetailOrderPlace)));

    public Task<Call<CancelRetailOrderRequest, RawRetailOrderResponse>> PostRetailOrderCancelByOrderIdCallAsync(
        CancelRetailOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            Component(BittradeEndpointIds.PostRetailOrderCancelByOrderId),
            BittradePrivateEndpoints.PostRetailOrderCancelByOrderId(request.OrderId),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawRetailOrderResponse>(
                json,
                Component(BittradeEndpointIds.PostRetailOrderCancelByOrderId)));

    public Task<Call<PostRetailOrderHistoryRequest, RawRetailOrdersResponse>> PostRetailOrderHistoryCallAsync(
        PostRetailOrderHistoryRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            Component(BittradeEndpointIds.PostRetailOrderHistory),
            BittradePrivateEndpoints.PostRetailOrderHistory(
                BittradeRawJson.SerializeOrThrow(request.Body, Component(BittradeEndpointIds.PostRetailOrderHistory))),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawRetailOrdersResponse>(
                json,
                Component(BittradeEndpointIds.PostRetailOrderHistory)));

    public Task<Call<PostRetailOrderDetailRequest, RawRetailOrderDetailResponse>> PostRetailOrderDetailCallAsync(
        PostRetailOrderDetailRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            Component(BittradeEndpointIds.PostRetailOrderDetail),
            BittradePrivateEndpoints.PostRetailOrderDetail(
                BittradeRawJson.SerializeOrThrow(request.Body, Component(BittradeEndpointIds.PostRetailOrderDetail))),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawRetailOrderDetailResponse>(
                json,
                Component(BittradeEndpointIds.PostRetailOrderDetail)));

    public Task<Call<CreateRetailOrderRequest, RawRetailOrderResponse>> PostRetailOrderCreateCallAsync(
        CreateRetailOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            Component(BittradeEndpointIds.PostRetailOrderCreate),
            BittradePrivateEndpoints.PostRetailOrderCreate(
                BittradeRawJson.SerializeOrThrow(request.Body, Component(BittradeEndpointIds.PostRetailOrderCreate))),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawRetailOrderResponse>(
                json,
                Component(BittradeEndpointIds.PostRetailOrderCreate)));

    private static string Component(string endpointId) => $"Bittrade.{endpointId}";
}
