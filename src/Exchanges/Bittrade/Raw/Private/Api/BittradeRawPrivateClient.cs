using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Raw.Private.Dtos;
using ExchangeApi.Exchanges.Bittrade.Raw.Private.Requests;
using ExchangeApi.Exchanges.Bittrade.Wire.Constants;
using ExchangeApi.Exchanges.Bittrade.Wire.Internal;
using ExchangeApi.Exchanges.Bittrade.Wire.Private.Endpoints;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Exchanges.Bittrade.Raw.Api;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Private.Api;

/// <summary>
/// Bittrade Private REST API の Raw 実装。
/// </summary>
internal sealed class BittradeRawPrivateClient
{
    private readonly IBittradeWireCallExecutor _wire;
    private readonly BittradeRawCallExecutor _executor;

    public BittradeRawPrivateClient(IBittradeWireCallExecutor wire, BittradeRawCallExecutor executor)
    {
        _wire = wire ?? throw new ArgumentNullException(nameof(wire));
        _executor = executor ?? throw new ArgumentNullException(nameof(executor));
    }

    public Task<Call<GetAccountsRequest, GetAccountsResponse>> GetAccountsCallAsync(
        GetAccountsRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BittradeEndpointIds.GetAccounts),
            BittradePrivateEndpoints.GetAccounts(),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<GetAccountsResponse>(json, Component(BittradeEndpointIds.GetAccounts)));

    public Task<Call<GetAccountsBalanceByAccountIdRequest, GetAccountsBalanceByAccountIdResponse>> GetAccountsBalanceByAccountIdCallAsync(
        GetAccountsBalanceByAccountIdRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BittradeEndpointIds.GetAccountsBalanceByAccountId),
            BittradePrivateEndpoints.GetAccountsBalanceByAccountId(request.AccountId.Value),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<GetAccountsBalanceByAccountIdResponse>(
                json,
                Component(BittradeEndpointIds.GetAccountsBalanceByAccountId)));

    public Task<Call<GetOpenOrdersRequest, GetOpenOrdersResponse>> GetOpenOrdersCallAsync(
        GetOpenOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BittradeEndpointIds.GetOpenOrders),
            BittradePrivateEndpoints.GetOpenOrders(request.Symbol.Value, request.AccountId.Value),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<GetOpenOrdersResponse>(
                json,
                Component(BittradeEndpointIds.GetOpenOrders)));

    public Task<Call<GetOrdersRequest, GetOrdersResponse>> GetOrdersCallAsync(
        GetOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BittradeEndpointIds.GetOrders),
            BittradePrivateEndpoints.GetOrders(),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<GetOrdersResponse>(json, Component(BittradeEndpointIds.GetOrders)));

    public Task<Call<GetOrdersByOrderIdRequest, GetOrdersByOrderIdResponse>> GetOrdersByOrderIdCallAsync(
        GetOrdersByOrderIdRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BittradeEndpointIds.GetOrdersByOrderId),
            BittradePrivateEndpoints.GetOrdersByOrderId(request.OrderId.Value),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<GetOrdersByOrderIdResponse>(json, Component(BittradeEndpointIds.GetOrdersByOrderId)));

    public Task<Call<GetOrdersMatchResultsByOrderIdRequest, GetOrdersMatchResultsByOrderIdResponse>> GetOrdersMatchResultsByOrderIdCallAsync(
        GetOrdersMatchResultsByOrderIdRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BittradeEndpointIds.GetOrdersMatchResultsByOrderId),
            BittradePrivateEndpoints.GetOrdersMatchResultsByOrderId(request.OrderId.Value),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<GetOrdersMatchResultsByOrderIdResponse>(
                json,
                Component(BittradeEndpointIds.GetOrdersMatchResultsByOrderId)));

    public Task<Call<GetMatchResultsRequest, GetMatchResultsResponse>> GetMatchResultsCallAsync(
        GetMatchResultsRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BittradeEndpointIds.GetMatchResults),
            BittradePrivateEndpoints.GetMatchResults(
                request.Symbol?.Value,
                request.Types?.Value,
                request.StartDate?.Value,
                request.EndDate?.Value,
                request.From?.ToString(CultureInfo.InvariantCulture),
                request.Direct?.Value,
                request.Size?.ToString(CultureInfo.InvariantCulture)),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<GetMatchResultsResponse>(
                json,
                Component(BittradeEndpointIds.GetMatchResults)));

    public Task<Call<GetDepositWithdrawRequest, GetDepositWithdrawResponse>> GetDepositWithdrawCallAsync(
        GetDepositWithdrawRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BittradeEndpointIds.GetDepositWithdraw),
            BittradePrivateEndpoints.GetDepositWithdraw(
                request.Type.Value,
                request.Currency?.Value,
                request.From?.ToString(CultureInfo.InvariantCulture),
                request.Size?.ToString(CultureInfo.InvariantCulture),
                request.Direct?.Value),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<GetDepositWithdrawResponse>(
                json,
                Component(BittradeEndpointIds.GetDepositWithdraw)));

    public Task<Call<GetWithdrawVirtualAddressesRequest, GetWithdrawVirtualAddressesResponse>> GetWithdrawVirtualAddressesCallAsync(
        GetWithdrawVirtualAddressesRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BittradeEndpointIds.GetWithdrawVirtualAddresses),
            BittradePrivateEndpoints.GetWithdrawVirtualAddresses(),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<GetWithdrawVirtualAddressesResponse>(
                json,
                Component(BittradeEndpointIds.GetWithdrawVirtualAddresses)));

    public Task<Call<GetRetailOrderListRequest, GetRetailOrderListResponse>> GetRetailOrderListCallAsync(
        GetRetailOrderListRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BittradeEndpointIds.GetRetailOrderList),
            BittradePrivateEndpoints.GetRetailOrderList(
                request.Direct.ToString(CultureInfo.InvariantCulture),
                request.Status?.ToString(CultureInfo.InvariantCulture),
                request.StartTime?.ToUnixTimeMilliseconds().ToString(CultureInfo.InvariantCulture),
                request.EndTime?.ToUnixTimeMilliseconds().ToString(CultureInfo.InvariantCulture)),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<GetRetailOrderListResponse>(
                json,
                Component(BittradeEndpointIds.GetRetailOrderList)));

    public Task<Call<GetRetailOrderDetailByOrderIdRequest, GetRetailOrderDetailByOrderIdResponse>> GetRetailOrderDetailByOrderIdCallAsync(
        GetRetailOrderDetailByOrderIdRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BittradeEndpointIds.PostRetailOrderDetail),
            BittradePrivateEndpoints.GetRetailOrderDetailByOrderId(request.OrderId.Value),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<GetRetailOrderDetailByOrderIdResponse>(
                json,
                Component(BittradeEndpointIds.PostRetailOrderDetail)));

    public Task<Call<GetRetailAccountBalanceRequest, GetRetailAccountBalanceResponse>> GetRetailAccountBalanceCallAsync(
        GetRetailAccountBalanceRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BittradeEndpointIds.GetRetailAccountBalance),
            BittradePrivateEndpoints.GetRetailAccountBalance(),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<GetRetailAccountBalanceResponse>(
                json,
                Component(BittradeEndpointIds.GetRetailAccountBalance)));

    public Task<Call<PostOrdersPlaceRequest, PostOrdersPlaceResponse>> PostOrdersPlaceCallAsync(
        PostOrdersPlaceRequest request,
        CancellationToken cancellationToken = default) =>
        TryBuildSpec(
            request,
            Component(BittradeEndpointIds.PostOrdersPlace),
            () =>
            {
                if (!BittradeRawJson.TrySerialize(request.Body, out var body, out var error))
                {
                    return (Spec: (WireCallSpec?)null, Error: error);
                }

                return (Spec: BittradePrivateEndpoints.PostOrdersPlace(body!), Error: (Exception?)null);
            },
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<PostOrdersPlaceResponse>(json, Component(BittradeEndpointIds.PostOrdersPlace)));

    public Task<Call<PostOrdersSubmitCancelByOrderIdRequest, PostOrdersSubmitCancelByOrderIdResponse>> PostOrdersSubmitCancelByOrderIdCallAsync(
        PostOrdersSubmitCancelByOrderIdRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BittradeEndpointIds.PostOrdersSubmitCancelByOrderId),
            BittradePrivateEndpoints.PostOrdersSubmitCancelByOrderId(request.OrderId.Value),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<PostOrdersSubmitCancelByOrderIdResponse>(json, Component(BittradeEndpointIds.PostOrdersSubmitCancelByOrderId)));

    public Task<Call<PostOrdersBatchCancelRequest, PostOrdersBatchCancelResponse>> PostOrdersBatchCancelCallAsync(
        PostOrdersBatchCancelRequest request,
        CancellationToken cancellationToken = default) =>
        TryBuildSpec(
            request,
            Component(BittradeEndpointIds.PostOrdersBatchCancel),
            () =>
            {
                if (!BittradeRawJson.TrySerialize(request.Body, out var body, out var error))
                {
                    return (Spec: (WireCallSpec?)null, Error: error);
                }

                return (Spec: BittradePrivateEndpoints.PostOrdersBatchCancel(body!), Error: (Exception?)null);
            },
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<PostOrdersBatchCancelResponse>(
                json,
                Component(BittradeEndpointIds.PostOrdersBatchCancel)));

    public Task<Call<PostOrdersBatchCancelOpenOrdersRequest, PostOrdersBatchCancelOpenOrdersResponse>> PostOrdersBatchCancelOpenOrdersCallAsync(
        PostOrdersBatchCancelOpenOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        TryBuildSpec(
            request,
            Component(BittradeEndpointIds.PostOrdersBatchCancelOpenOrders),
            () =>
            {
                if (!BittradeRawJson.TrySerialize(request.Body, out var body, out var error))
                {
                    return (Spec: (WireCallSpec?)null, Error: error);
                }

                return (Spec: BittradePrivateEndpoints.PostOrdersBatchCancelOpenOrders(body!), Error: (Exception?)null);
            },
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<PostOrdersBatchCancelOpenOrdersResponse>(
                json,
                Component(BittradeEndpointIds.PostOrdersBatchCancelOpenOrders)));

    public Task<Call<PostWithdrawApiCreateRequest, PostWithdrawApiCreateResponse>> PostWithdrawApiCreateCallAsync(
        PostWithdrawApiCreateRequest request,
        CancellationToken cancellationToken = default) =>
        TryBuildSpec(
            request,
            Component(BittradeEndpointIds.PostWithdrawApiCreate),
            () =>
            {
                if (!BittradeRawJson.TrySerialize(request.Body, out var body, out var error))
                {
                    return (Spec: (WireCallSpec?)null, Error: error);
                }

                return (Spec: BittradePrivateEndpoints.PostWithdrawApiCreate(body!), Error: (Exception?)null);
            },
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<PostWithdrawApiCreateResponse>(
                json,
                Component(BittradeEndpointIds.PostWithdrawApiCreate)));

    public Task<Call<PostWithdrawVirtualByAddressIdCreateRequest, PostWithdrawVirtualByAddressIdCreateResponse>> PostWithdrawVirtualByAddressIdCreateCallAsync(
        PostWithdrawVirtualByAddressIdCreateRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bittrade.PostWithdrawApiCreateByAddressId",
            BittradePrivateEndpoints.PostWithdrawVirtualByAddressIdCreate(request.AddressId.Value),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<PostWithdrawVirtualByAddressIdCreateResponse>(
                json,
                "Bittrade.PostWithdrawApiCreateByAddressId"));

    public Task<Call<PostWithdrawVirtualByWithdrawIdCancelRequest, PostWithdrawVirtualByWithdrawIdCancelResponse>> PostWithdrawVirtualByWithdrawIdCancelCallAsync(
        PostWithdrawVirtualByWithdrawIdCancelRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BittradeEndpointIds.PostWithdrawVirtualByWithdrawIdCancel),
            BittradePrivateEndpoints.PostWithdrawVirtualByWithdrawIdCancel(request.WithdrawId.Value),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<PostWithdrawVirtualByWithdrawIdCancelResponse>(
                json,
                Component(BittradeEndpointIds.PostWithdrawVirtualByWithdrawIdCancel)));

    public Task<Call<PostWithdrawVirtualByWithdrawIdPlaceRequest, PostWithdrawVirtualByWithdrawIdPlaceResponse>> PostWithdrawVirtualByWithdrawIdPlaceCallAsync(
        PostWithdrawVirtualByWithdrawIdPlaceRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BittradeEndpointIds.PostWithdrawVirtualByWithdrawIdPlace),
            BittradePrivateEndpoints.PostWithdrawVirtualByWithdrawIdPlace(request.WithdrawId.Value),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<PostWithdrawVirtualByWithdrawIdPlaceResponse>(
                json,
                Component(BittradeEndpointIds.PostWithdrawVirtualByWithdrawIdPlace)));

    public Task<Call<PostRetailOrderPlaceRequest, PostRetailOrderPlaceResponse>> PostRetailOrderPlaceCallAsync(
        PostRetailOrderPlaceRequest request,
        CancellationToken cancellationToken = default) =>
        TryBuildSpec(
            request,
            Component(BittradeEndpointIds.PostRetailOrderPlace),
            () =>
            {
                if (!BittradeRawJson.TrySerialize(request.Body, out var body, out var error))
                {
                    return (Spec: (WireCallSpec?)null, Error: error);
                }

                return (Spec: BittradePrivateEndpoints.PostRetailOrderPlace(body!), Error: (Exception?)null);
            },
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<PostRetailOrderPlaceResponse>(
                json,
                Component(BittradeEndpointIds.PostRetailOrderPlace)));

    public Task<Call<PostRetailOrderCancelByOrderIdRequest, PostRetailOrderCancelByOrderIdResponse>> PostRetailOrderCancelByOrderIdCallAsync(
        PostRetailOrderCancelByOrderIdRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BittradeEndpointIds.PostRetailOrderCancelByOrderId),
            BittradePrivateEndpoints.PostRetailOrderCancelByOrderId(request.OrderId.Value),
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<PostRetailOrderCancelByOrderIdResponse>(
                json,
                Component(BittradeEndpointIds.PostRetailOrderCancelByOrderId)));

    public Task<Call<PostRetailOrderHistoryRequest, PostRetailOrderHistoryResponse>> PostRetailOrderHistoryCallAsync(
        PostRetailOrderHistoryRequest request,
        CancellationToken cancellationToken = default) =>
        TryBuildSpec(
            request,
            Component(BittradeEndpointIds.PostRetailOrderHistory),
            () =>
            {
                if (!BittradeRawJson.TrySerialize(request.Body, out var body, out var error))
                {
                    return (Spec: (WireCallSpec?)null, Error: error);
                }

                return (Spec: BittradePrivateEndpoints.PostRetailOrderHistory(body!), Error: (Exception?)null);
            },
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<PostRetailOrderHistoryResponse>(
                json,
                Component(BittradeEndpointIds.PostRetailOrderHistory)));

    public Task<Call<PostRetailOrderDetailRequest, PostRetailOrderDetailResponse>> PostRetailOrderDetailCallAsync(
        PostRetailOrderDetailRequest request,
        CancellationToken cancellationToken = default) =>
        TryBuildSpec(
            request,
            Component(BittradeEndpointIds.PostRetailOrderDetail),
            () =>
            {
                if (!BittradeRawJson.TrySerialize(request.Body, out var body, out var error))
                {
                    return (Spec: (WireCallSpec?)null, Error: error);
                }

                return (Spec: BittradePrivateEndpoints.PostRetailOrderDetail(body!), Error: (Exception?)null);
            },
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<PostRetailOrderDetailResponse>(
                json,
                Component(BittradeEndpointIds.PostRetailOrderDetail)));

    public Task<Call<PostRetailOrderCreateRequest, PostRetailOrderCreateResponse>> PostRetailOrderCreateCallAsync(
        PostRetailOrderCreateRequest request,
        CancellationToken cancellationToken = default) =>
        TryBuildSpec(
            request,
            Component(BittradeEndpointIds.PostRetailOrderCreate),
            () =>
            {
                if (!BittradeRawJson.TrySerialize(request.Body, out var body, out var error))
                {
                    return (Spec: (WireCallSpec?)null, Error: error);
                }

                return (Spec: BittradePrivateEndpoints.PostRetailOrderCreate(body!), Error: (Exception?)null);
            },
            cancellationToken,
            json => BittradeRawJson.DeserializeOrThrow<PostRetailOrderCreateResponse>(
                json,
                Component(BittradeEndpointIds.PostRetailOrderCreate)));

    private async Task<Call<TReq, TRes>> SendAndParse<TReq, TRes>(
        TReq request,
        string component,
        WireCallSpec spec,
        CancellationToken cancellationToken,
        Func<string, TRes> parse)
    {
        var wireCall = await _wire.SendAsync(spec, cancellationToken).ConfigureAwait(false);
        return _executor.Parse(request, component, wireCall, parse);
    }

    private Task<Call<TReq, TRes>> TryBuildSpec<TReq, TRes>(
        TReq request,
        string component,
        Func<(WireCallSpec? Spec, Exception? Error)> buildSpec,
        CancellationToken cancellationToken,
        Func<string, TRes> parse)
    {
        var (spec, error) = buildSpec();
        if (spec is null)
        {
            return Task.FromResult(CreateSerializeErrorCall<TReq, TRes>(request, component, error));
        }

        return SendAndParse(request, component, spec, cancellationToken, parse);
    }

    private static Call<TReq, TRes> CreateSerializeErrorCall<TReq, TRes>(
        TReq request,
        string component,
        Exception? error)
    {
        var callError = new CallError(
            CallErrorKind.Codec,
            $"{component} failed to serialize request.",
            error);
        var meta = CallMeta.CreateInternal("Raw", component);
        var now = DateTimeOffset.UtcNow;

        return new Call<TReq, TRes>(
            Id: CallId.New(),
            StartedAt: now,
            Duration: TimeSpan.Zero,
            Request: request,
            Result: new CallResult<TRes>.Err(callError),
            Meta: meta);
    }

    private static string Component(string endpointId) => $"Bittrade.{endpointId}";
}
