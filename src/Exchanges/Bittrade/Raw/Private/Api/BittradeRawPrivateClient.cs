using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Raw.Private.Dtos;
using ExchangeApi.Exchanges.Bittrade.Raw.Private.Requests;
using ExchangeApi.Exchanges.Bittrade.Wire.Private.Endpoints;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Exchanges.Bittrade.Raw.Api;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Private.Api;

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
            "Bittrade.GetAccountBalance",
            BittradePrivateEndpoints.GetAccountsBalanceByAccountId(request.AccountId),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawBalancesResponse>(
                json,
                "Bittrade.GetAccountBalance"));

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
            "Bittrade.GetOrder",
            BittradePrivateEndpoints.GetOrdersByOrderId(request.OrderId),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawOrderDetailResponse>(json, "Bittrade.GetOrder"));

    public Task<Call<GetOrderMatchResultsRequest, RawOrderMatchResultsResponse>> GetOrdersMatchResultsByOrderIdCallAsync(
        GetOrderMatchResultsRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bittrade.GetOrderMatchResults",
            BittradePrivateEndpoints.GetOrdersMatchResultsByOrderId(request.OrderId),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawOrderMatchResultsResponse>(
                json,
                "Bittrade.GetOrderMatchResults"));

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
            "Bittrade.GetDepositWithdraws",
            BittradePrivateEndpoints.GetDepositWithdraw(
                request.Type,
                request.Currency,
                request.From?.ToString(CultureInfo.InvariantCulture),
                request.Size?.ToString(CultureInfo.InvariantCulture),
                request.Direct),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawDepositWithdrawsResponse>(
                json,
                "Bittrade.GetDepositWithdraws"));

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
            "Bittrade.GetRetailOrders",
            BittradePrivateEndpoints.GetRetailOrderList(
                request.Direct.ToString(CultureInfo.InvariantCulture),
                request.Status?.ToString(CultureInfo.InvariantCulture),
                request.StartTime?.ToUnixTimeMilliseconds().ToString(CultureInfo.InvariantCulture),
                request.EndTime?.ToUnixTimeMilliseconds().ToString(CultureInfo.InvariantCulture)),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawRetailOrdersResponse>(
                json,
                "Bittrade.GetRetailOrders"));

    public Task<Call<GetRetailOrderDetailByOrderIdRequest, RawRetailOrderDetailResponse>> GetRetailOrderDetailByOrderIdCallAsync(
        GetRetailOrderDetailByOrderIdRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bittrade.GetRetailOrderDetail",
            BittradePrivateEndpoints.GetRetailOrderDetailByOrderId(request.OrderId),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawRetailOrderDetailResponse>(
                json,
                "Bittrade.GetRetailOrderDetail"));

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
            "Bittrade.PlaceOrder",
            BittradePrivateEndpoints.PostOrdersPlace(
                BittradeRawJson.SerializeOrThrow(request.Body, "Bittrade.PlaceOrder")),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawPlaceOrderResponse>(json, "Bittrade.PlaceOrder"));

    public Task<Call<CancelOrderRequest, RawCancelOrderResponse>> PostOrdersSubmitCancelByOrderIdCallAsync(
        CancelOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bittrade.CancelOrder",
            BittradePrivateEndpoints.PostOrdersSubmitCancelByOrderId(request.OrderId),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawCancelOrderResponse>(json, "Bittrade.CancelOrder"));

    public Task<Call<CancelOrdersRequest, RawCancelOrdersResponse>> PostOrdersBatchCancelCallAsync(
        CancelOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bittrade.CancelOrders",
            BittradePrivateEndpoints.PostOrdersBatchCancel(
                BittradeRawJson.SerializeOrThrow(request.Body, "Bittrade.CancelOrders")),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawCancelOrdersResponse>(
                json,
                "Bittrade.CancelOrders"));

    public Task<Call<CancelOpenOrdersRequest, RawCancelOpenOrdersResponse>> PostOrdersBatchCancelOpenOrdersCallAsync(
        CancelOpenOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bittrade.CancelOpenOrders",
            BittradePrivateEndpoints.PostOrdersBatchCancelOpenOrders(
                BittradeRawJson.SerializeOrThrow(request.Body, "Bittrade.CancelOpenOrders")),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawCancelOpenOrdersResponse>(
                json,
                "Bittrade.CancelOpenOrders"));

    public Task<Call<CreateWithdrawRequest, RawCreateWithdrawResponse>> PostWithdrawApiCreateCallAsync(
        CreateWithdrawRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bittrade.CreateWithdraw",
            BittradePrivateEndpoints.PostWithdrawApiCreate(
                BittradeRawJson.SerializeOrThrow(request.Body, "Bittrade.CreateWithdraw")),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawCreateWithdrawResponse>(
                json,
                "Bittrade.CreateWithdraw"));

    public Task<Call<CreateWithdrawVirtualByAddressIdRequest, RawCreateWithdrawResponse>> PostWithdrawVirtualByAddressIdCreateCallAsync(
        CreateWithdrawVirtualByAddressIdRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bittrade.CreateWithdrawByAddressId",
            BittradePrivateEndpoints.PostWithdrawVirtualByAddressIdCreate(request.AddressId),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawCreateWithdrawResponse>(
                json,
                "Bittrade.CreateWithdrawByAddressId"));

    public Task<Call<CancelWithdrawRequest, RawCancelWithdrawResponse>> PostWithdrawVirtualByWithdrawIdCancelCallAsync(
        CancelWithdrawRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bittrade.CancelWithdraw",
            BittradePrivateEndpoints.PostWithdrawVirtualByWithdrawIdCancel(request.WithdrawId),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawCancelWithdrawResponse>(
                json,
                "Bittrade.CancelWithdraw"));

    public Task<Call<PlaceWithdrawVirtualRequest, RawCreateWithdrawResponse>> PostWithdrawVirtualByWithdrawIdPlaceCallAsync(
        PlaceWithdrawVirtualRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bittrade.PlaceWithdraw",
            BittradePrivateEndpoints.PostWithdrawVirtualByWithdrawIdPlace(request.WithdrawId),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawCreateWithdrawResponse>(
                json,
                "Bittrade.PlaceWithdraw"));

    public Task<Call<CreateRetailOrderRequest, RawRetailOrderResponse>> PostRetailOrderPlaceCallAsync(
        CreateRetailOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bittrade.CreateRetailOrder",
            BittradePrivateEndpoints.PostRetailOrderPlace(
                BittradeRawJson.SerializeOrThrow(request.Body, "Bittrade.CreateRetailOrder")),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawRetailOrderResponse>(
                json,
                "Bittrade.CreateRetailOrder"));

    public Task<Call<CancelRetailOrderRequest, RawRetailOrderResponse>> PostRetailOrderCancelByOrderIdCallAsync(
        CancelRetailOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bittrade.CancelRetailOrder",
            BittradePrivateEndpoints.PostRetailOrderCancelByOrderId(request.OrderId),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawRetailOrderResponse>(
                json,
                "Bittrade.CancelRetailOrder"));

    public Task<Call<PostRetailOrderHistoryRequest, RawRetailOrdersResponse>> PostRetailOrderHistoryCallAsync(
        PostRetailOrderHistoryRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bittrade.GetRetailOrderHistory",
            BittradePrivateEndpoints.PostRetailOrderHistory(
                BittradeRawJson.SerializeOrThrow(request.Body, "Bittrade.GetRetailOrderHistory")),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawRetailOrdersResponse>(
                json,
                "Bittrade.GetRetailOrderHistory"));

    public Task<Call<PostRetailOrderDetailRequest, RawRetailOrderDetailResponse>> PostRetailOrderDetailCallAsync(
        PostRetailOrderDetailRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bittrade.GetRetailOrderDetail",
            BittradePrivateEndpoints.PostRetailOrderDetail(
                BittradeRawJson.SerializeOrThrow(request.Body, "Bittrade.GetRetailOrderDetail")),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawRetailOrderDetailResponse>(
                json,
                "Bittrade.GetRetailOrderDetail"));

    public Task<Call<CreateRetailOrderRequest, RawRetailOrderResponse>> PostRetailOrderCreateCallAsync(
        CreateRetailOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bittrade.CreateRetailOrder",
            BittradePrivateEndpoints.PostRetailOrderCreate(
                BittradeRawJson.SerializeOrThrow(request.Body, "Bittrade.CreateRetailOrder")),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<RawRetailOrderResponse>(
                json,
                "Bittrade.CreateRetailOrder"));
}
