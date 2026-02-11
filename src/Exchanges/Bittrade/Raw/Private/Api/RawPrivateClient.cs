using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Common.Raw.Api;
using ExchangeApi.Exchanges.Bittrade.Raw.Private.Dtos;
using ExchangeApi.Exchanges.Bittrade.Raw.Private.Requests;
using ExchangeApi.Exchanges.Bittrade.Wire.Constants;
using ExchangeApi.Exchanges.Bittrade.Wire.Internal;
using ExchangeApi.Exchanges.Bittrade.Wire.Private.Endpoints;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Private.Api;

/// <summary>
/// Bittrade Private REST API の Raw 実装。
/// </summary>
internal sealed class RawPrivateClient
{
    private readonly IBittradeWireCallExecutor _wire;
    private readonly RawCallExecutor _executor;

    public RawPrivateClient(IBittradeWireCallExecutor wire, RawCallExecutor executor)
    {
        _wire = wire ?? throw new ArgumentNullException(nameof(wire));
        _executor = executor ?? throw new ArgumentNullException(nameof(executor));
    }

    public Task<Call<GetAccountsRequest, GetAccountsResponse>> GetAccountsCallAsync(
        GetAccountsRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(EndpointIds.GetAccounts),
            PrivateEndpoints.GetAccounts(),
            cancellationToken,
            json => RawJson.DeserializeOrThrow<GetAccountsResponse>(json, Component(EndpointIds.GetAccounts)));

    public Task<Call<GetAccountsBalanceByAccountIdRequest, GetAccountsBalanceByAccountIdResponse>> GetAccountsBalanceByAccountIdCallAsync(
        GetAccountsBalanceByAccountIdRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(EndpointIds.GetAccountsBalanceByAccountId),
            PrivateEndpoints.GetAccountsBalanceByAccountId(request.AccountId.Value),
            cancellationToken,
            json => RawJson.DeserializeOrThrow<GetAccountsBalanceByAccountIdResponse>(
                json,
                Component(EndpointIds.GetAccountsBalanceByAccountId)));

    public Task<Call<GetOpenOrdersRequest, GetOpenOrdersResponse>> GetOpenOrdersCallAsync(
        GetOpenOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(EndpointIds.GetOpenOrders),
            PrivateEndpoints.GetOpenOrders(request.Symbol.Value, request.AccountId.Value),
            cancellationToken,
            json => RawJson.DeserializeOrThrow<GetOpenOrdersResponse>(
                json,
                Component(EndpointIds.GetOpenOrders)));

    public Task<Call<GetOrdersRequest, GetOrdersResponse>> GetOrdersCallAsync(
        GetOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(EndpointIds.GetOrders),
            PrivateEndpoints.GetOrders(),
            cancellationToken,
            json => RawJson.DeserializeOrThrow<GetOrdersResponse>(json, Component(EndpointIds.GetOrders)));

    public Task<Call<GetOrdersByOrderIdRequest, GetOrdersByOrderIdResponse>> GetOrdersByOrderIdCallAsync(
        GetOrdersByOrderIdRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(EndpointIds.GetOrdersByOrderId),
            PrivateEndpoints.GetOrdersByOrderId(request.OrderId.Value),
            cancellationToken,
            json => RawJson.DeserializeOrThrow<GetOrdersByOrderIdResponse>(json, Component(EndpointIds.GetOrdersByOrderId)));

    public Task<Call<GetOrdersMatchResultsByOrderIdRequest, GetOrdersMatchResultsByOrderIdResponse>> GetOrdersMatchResultsByOrderIdCallAsync(
        GetOrdersMatchResultsByOrderIdRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(EndpointIds.GetOrdersMatchResultsByOrderId),
            PrivateEndpoints.GetOrdersMatchResultsByOrderId(request.OrderId.Value),
            cancellationToken,
            json => RawJson.DeserializeOrThrow<GetOrdersMatchResultsByOrderIdResponse>(
                json,
                Component(EndpointIds.GetOrdersMatchResultsByOrderId)));

    public Task<Call<GetMatchResultsRequest, GetMatchResultsResponse>> GetMatchResultsCallAsync(
        GetMatchResultsRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(EndpointIds.GetMatchResults),
            PrivateEndpoints.GetMatchResults(
                request.Symbol?.Value,
                request.Types?.Value,
                request.StartDate?.Value,
                request.EndDate?.Value,
                request.From?.ToString(CultureInfo.InvariantCulture),
                request.Direct?.Value,
                request.Size?.ToString(CultureInfo.InvariantCulture)),
            cancellationToken,
            json => RawJson.DeserializeOrThrow<GetMatchResultsResponse>(
                json,
                Component(EndpointIds.GetMatchResults)));

    public Task<Call<GetDepositWithdrawRequest, GetDepositWithdrawResponse>> GetDepositWithdrawCallAsync(
        GetDepositWithdrawRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(EndpointIds.GetDepositWithdraw),
            PrivateEndpoints.GetDepositWithdraw(
                request.Type.Value,
                request.Currency?.Value,
                request.From?.ToString(CultureInfo.InvariantCulture),
                request.Size?.ToString(CultureInfo.InvariantCulture),
                request.Direct?.Value),
            cancellationToken,
            json => RawJson.DeserializeOrThrow<GetDepositWithdrawResponse>(
                json,
                Component(EndpointIds.GetDepositWithdraw)));

    public Task<Call<GetWithdrawVirtualAddressesRequest, GetWithdrawVirtualAddressesResponse>> GetWithdrawVirtualAddressesCallAsync(
        GetWithdrawVirtualAddressesRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(EndpointIds.GetWithdrawVirtualAddresses),
            PrivateEndpoints.GetWithdrawVirtualAddresses(),
            cancellationToken,
            json => RawJson.DeserializeOrThrow<GetWithdrawVirtualAddressesResponse>(
                json,
                Component(EndpointIds.GetWithdrawVirtualAddresses)));

    public Task<Call<GetRetailOrderListRequest, GetRetailOrderListResponse>> GetRetailOrderListCallAsync(
        GetRetailOrderListRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(EndpointIds.GetRetailOrderList),
            PrivateEndpoints.GetRetailOrderList(
                request.Direct.ToString(CultureInfo.InvariantCulture),
                request.Status?.ToString(CultureInfo.InvariantCulture),
                request.StartTime?.ToUnixTimeMilliseconds().ToString(CultureInfo.InvariantCulture),
                request.EndTime?.ToUnixTimeMilliseconds().ToString(CultureInfo.InvariantCulture)),
            cancellationToken,
            json => RawJson.DeserializeOrThrow<GetRetailOrderListResponse>(
                json,
                Component(EndpointIds.GetRetailOrderList)));

    public Task<Call<GetRetailOrderDetailByOrderIdRequest, GetRetailOrderDetailByOrderIdResponse>> GetRetailOrderDetailByOrderIdCallAsync(
        GetRetailOrderDetailByOrderIdRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(EndpointIds.PostRetailOrderDetail),
            PrivateEndpoints.GetRetailOrderDetailByOrderId(request.OrderId.Value),
            cancellationToken,
            json => RawJson.DeserializeOrThrow<GetRetailOrderDetailByOrderIdResponse>(
                json,
                Component(EndpointIds.PostRetailOrderDetail)));

    public Task<Call<GetRetailAccountBalanceRequest, GetRetailAccountBalanceResponse>> GetRetailAccountBalanceCallAsync(
        GetRetailAccountBalanceRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(EndpointIds.GetRetailAccountBalance),
            PrivateEndpoints.GetRetailAccountBalance(),
            cancellationToken,
            json => RawJson.DeserializeOrThrow<GetRetailAccountBalanceResponse>(
                json,
                Component(EndpointIds.GetRetailAccountBalance)));

    public Task<Call<PostOrdersPlaceRequest, PostOrdersPlaceResponse>> PostOrdersPlaceCallAsync(
        PostOrdersPlaceRequest request,
        CancellationToken cancellationToken = default) =>
        TryBuildSpec(
            request,
            Component(EndpointIds.PostOrdersPlace),
            () =>
            {
                if (!RawJson.TrySerialize(request.Body, out var body, out var error))
                {
                    return (Spec: (WireCallSpec?)null, Error: error);
                }

                return (Spec: PrivateEndpoints.PostOrdersPlace(body!), Error: (Exception?)null);
            },
            cancellationToken,
            json => RawJson.DeserializeOrThrow<PostOrdersPlaceResponse>(json, Component(EndpointIds.PostOrdersPlace)));

    public Task<Call<PostOrdersSubmitCancelByOrderIdRequest, PostOrdersSubmitCancelByOrderIdResponse>> PostOrdersSubmitCancelByOrderIdCallAsync(
        PostOrdersSubmitCancelByOrderIdRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(EndpointIds.PostOrdersSubmitCancelByOrderId),
            PrivateEndpoints.PostOrdersSubmitCancelByOrderId(request.OrderId.Value),
            cancellationToken,
            json => RawJson.DeserializeOrThrow<PostOrdersSubmitCancelByOrderIdResponse>(json, Component(EndpointIds.PostOrdersSubmitCancelByOrderId)));

    public Task<Call<PostOrdersBatchCancelRequest, PostOrdersBatchCancelResponse>> PostOrdersBatchCancelCallAsync(
        PostOrdersBatchCancelRequest request,
        CancellationToken cancellationToken = default) =>
        TryBuildSpec(
            request,
            Component(EndpointIds.PostOrdersBatchCancel),
            () =>
            {
                if (!RawJson.TrySerialize(request.Body, out var body, out var error))
                {
                    return (Spec: (WireCallSpec?)null, Error: error);
                }

                return (Spec: PrivateEndpoints.PostOrdersBatchCancel(body!), Error: (Exception?)null);
            },
            cancellationToken,
            json => RawJson.DeserializeOrThrow<PostOrdersBatchCancelResponse>(
                json,
                Component(EndpointIds.PostOrdersBatchCancel)));

    public Task<Call<PostOrdersBatchCancelOpenOrdersRequest, PostOrdersBatchCancelOpenOrdersResponse>> PostOrdersBatchCancelOpenOrdersCallAsync(
        PostOrdersBatchCancelOpenOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        TryBuildSpec(
            request,
            Component(EndpointIds.PostOrdersBatchCancelOpenOrders),
            () =>
            {
                if (!RawJson.TrySerialize(request.Body, out var body, out var error))
                {
                    return (Spec: (WireCallSpec?)null, Error: error);
                }

                return (Spec: PrivateEndpoints.PostOrdersBatchCancelOpenOrders(body!), Error: (Exception?)null);
            },
            cancellationToken,
            json => RawJson.DeserializeOrThrow<PostOrdersBatchCancelOpenOrdersResponse>(
                json,
                Component(EndpointIds.PostOrdersBatchCancelOpenOrders)));

    public Task<Call<PostWithdrawApiCreateRequest, PostWithdrawApiCreateResponse>> PostWithdrawApiCreateCallAsync(
        PostWithdrawApiCreateRequest request,
        CancellationToken cancellationToken = default) =>
        TryBuildSpec(
            request,
            Component(EndpointIds.PostWithdrawApiCreate),
            () =>
            {
                if (!RawJson.TrySerialize(request.Body, out var body, out var error))
                {
                    return (Spec: (WireCallSpec?)null, Error: error);
                }

                return (Spec: PrivateEndpoints.PostWithdrawApiCreate(body!), Error: (Exception?)null);
            },
            cancellationToken,
            json => RawJson.DeserializeOrThrow<PostWithdrawApiCreateResponse>(
                json,
                Component(EndpointIds.PostWithdrawApiCreate)));

    public Task<Call<PostWithdrawVirtualByAddressIdCreateRequest, PostWithdrawVirtualByAddressIdCreateResponse>> PostWithdrawVirtualByAddressIdCreateCallAsync(
        PostWithdrawVirtualByAddressIdCreateRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(EndpointIds.PostWithdrawVirtualByAddressIdCreate),
            PrivateEndpoints.PostWithdrawVirtualByAddressIdCreate(request.AddressId.Value),
            cancellationToken,
            json => RawJson.DeserializeOrThrow<PostWithdrawVirtualByAddressIdCreateResponse>(
                json,
                Component(EndpointIds.PostWithdrawVirtualByAddressIdCreate)));

    public Task<Call<PostWithdrawVirtualByWithdrawIdCancelRequest, PostWithdrawVirtualByWithdrawIdCancelResponse>> PostWithdrawVirtualByWithdrawIdCancelCallAsync(
        PostWithdrawVirtualByWithdrawIdCancelRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(EndpointIds.PostWithdrawVirtualByWithdrawIdCancel),
            PrivateEndpoints.PostWithdrawVirtualByWithdrawIdCancel(request.WithdrawId.Value),
            cancellationToken,
            json => RawJson.DeserializeOrThrow<PostWithdrawVirtualByWithdrawIdCancelResponse>(
                json,
                Component(EndpointIds.PostWithdrawVirtualByWithdrawIdCancel)));

    public Task<Call<PostWithdrawVirtualByWithdrawIdPlaceRequest, PostWithdrawVirtualByWithdrawIdPlaceResponse>> PostWithdrawVirtualByWithdrawIdPlaceCallAsync(
        PostWithdrawVirtualByWithdrawIdPlaceRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(EndpointIds.PostWithdrawVirtualByWithdrawIdPlace),
            PrivateEndpoints.PostWithdrawVirtualByWithdrawIdPlace(request.WithdrawId.Value),
            cancellationToken,
            json => RawJson.DeserializeOrThrow<PostWithdrawVirtualByWithdrawIdPlaceResponse>(
                json,
                Component(EndpointIds.PostWithdrawVirtualByWithdrawIdPlace)));

    public Task<Call<PostRetailOrderPlaceRequest, PostRetailOrderPlaceResponse>> PostRetailOrderPlaceCallAsync(
        PostRetailOrderPlaceRequest request,
        CancellationToken cancellationToken = default) =>
        TryBuildSpec(
            request,
            Component(EndpointIds.PostRetailOrderPlace),
            () =>
            {
                if (!RawJson.TrySerialize(request.Body, out var body, out var error))
                {
                    return (Spec: (WireCallSpec?)null, Error: error);
                }

                return (Spec: PrivateEndpoints.PostRetailOrderPlace(body!), Error: (Exception?)null);
            },
            cancellationToken,
            json => RawJson.DeserializeOrThrow<PostRetailOrderPlaceResponse>(
                json,
                Component(EndpointIds.PostRetailOrderPlace)));

    public Task<Call<PostRetailOrderCancelByOrderIdRequest, PostRetailOrderCancelByOrderIdResponse>> PostRetailOrderCancelByOrderIdCallAsync(
        PostRetailOrderCancelByOrderIdRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(EndpointIds.PostRetailOrderCancelByOrderId),
            PrivateEndpoints.PostRetailOrderCancelByOrderId(request.OrderId.Value),
            cancellationToken,
            json => RawJson.DeserializeOrThrow<PostRetailOrderCancelByOrderIdResponse>(
                json,
                Component(EndpointIds.PostRetailOrderCancelByOrderId)));

    public Task<Call<PostRetailOrderHistoryRequest, PostRetailOrderHistoryResponse>> PostRetailOrderHistoryCallAsync(
        PostRetailOrderHistoryRequest request,
        CancellationToken cancellationToken = default) =>
        TryBuildSpec(
            request,
            Component(EndpointIds.PostRetailOrderHistory),
            () =>
            {
                if (!RawJson.TrySerialize(request.Body, out var body, out var error))
                {
                    return (Spec: (WireCallSpec?)null, Error: error);
                }

                return (Spec: PrivateEndpoints.PostRetailOrderHistory(body!), Error: (Exception?)null);
            },
            cancellationToken,
            json => RawJson.DeserializeOrThrow<PostRetailOrderHistoryResponse>(
                json,
                Component(EndpointIds.PostRetailOrderHistory)));

    public Task<Call<PostRetailOrderDetailRequest, PostRetailOrderDetailResponse>> PostRetailOrderDetailCallAsync(
        PostRetailOrderDetailRequest request,
        CancellationToken cancellationToken = default) =>
        TryBuildSpec(
            request,
            Component(EndpointIds.PostRetailOrderDetail),
            () =>
            {
                if (!RawJson.TrySerialize(request.Body, out var body, out var error))
                {
                    return (Spec: (WireCallSpec?)null, Error: error);
                }

                return (Spec: PrivateEndpoints.PostRetailOrderDetail(body!), Error: (Exception?)null);
            },
            cancellationToken,
            json => RawJson.DeserializeOrThrow<PostRetailOrderDetailResponse>(
                json,
                Component(EndpointIds.PostRetailOrderDetail)));

    public Task<Call<PostRetailOrderCreateRequest, PostRetailOrderCreateResponse>> PostRetailOrderCreateCallAsync(
        PostRetailOrderCreateRequest request,
        CancellationToken cancellationToken = default) =>
        TryBuildSpec(
            request,
            Component(EndpointIds.PostRetailOrderCreate),
            () =>
            {
                if (!RawJson.TrySerialize(request.Body, out var body, out var error))
                {
                    return (Spec: (WireCallSpec?)null, Error: error);
                }

                return (Spec: PrivateEndpoints.PostRetailOrderCreate(body!), Error: (Exception?)null);
            },
            cancellationToken,
            json => RawJson.DeserializeOrThrow<PostRetailOrderCreateResponse>(
                json,
                Component(EndpointIds.PostRetailOrderCreate)));

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
