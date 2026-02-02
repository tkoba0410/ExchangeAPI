using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Api.Raw.Internal;
using ExchangeApi.Exchanges.Bitflyer.Api.Raw.Private.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Api.Raw.Private.Requests;
using ExchangeApi.Exchanges.Bitflyer.Api.Wire.Constants;
using ExchangeApi.Exchanges.Bitflyer.Api.Wire.Internal;
using ExchangeApi.Exchanges.Bitflyer.Api.Wire.Private.Endpoints;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Exchanges.Bitflyer.Api.Raw.Private.Api;

internal sealed class BitflyerRawPrivateClient
{
    private readonly IBitflyerWireCallExecutor _wire;
    private readonly BitflyerRawCallExecutor _executor;

    public BitflyerRawPrivateClient(IBitflyerWireCallExecutor wire, BitflyerRawCallExecutor executor)
    {
        _wire = wire ?? throw new System.ArgumentNullException(nameof(wire));
        _executor = executor ?? throw new System.ArgumentNullException(nameof(executor));
    }

    public Task<Call<GetPermissionsRequest, IReadOnlyList<string>>> GetPermissionsCallAsync(
        GetPermissionsRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BitflyerEndpointIds.GetPermissions),
            BitflyerPrivateEndpoints.GetPermissions(),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<string>>(
                json,
                Component(BitflyerEndpointIds.GetPermissions)));

    public Task<Call<GetBalancesRequest, IReadOnlyList<BalanceResponse>>> GetBalanceCallAsync(
        GetBalancesRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BitflyerEndpointIds.GetBalance),
            BitflyerPrivateEndpoints.GetBalance(),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<BalanceResponse>>(
                json,
                Component(BitflyerEndpointIds.GetBalance)));

    public Task<Call<GetPositionsRequest, IReadOnlyList<PositionResponse>>> GetPositionsCallAsync(
        GetPositionsRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BitflyerEndpointIds.GetPositions),
            BitflyerPrivateEndpoints.GetPositions(request.ProductCode),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<PositionResponse>>(
                json,
                Component(BitflyerEndpointIds.GetPositions)));

    public Task<Call<GetAccountExecutionsRequest, IReadOnlyList<ExecutionPrivateResponse>>> GetExecutionsPrivateCallAsync(
        GetAccountExecutionsRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BitflyerEndpointIds.GetExecutionsPrivate),
            BitflyerPrivateEndpoints.GetExecutionsPrivate(
                request.ProductCode,
                request.ChildOrderId,
                request.ChildOrderAcceptanceId,
                request.Count?.ToString(CultureInfo.InvariantCulture),
                request.Before?.ToString(CultureInfo.InvariantCulture),
                request.After?.ToString(CultureInfo.InvariantCulture)),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<ExecutionPrivateResponse>>(
                json,
                Component(BitflyerEndpointIds.GetExecutionsPrivate)));

    public Task<Call<GetCollateralRequest, CollateralResponse>> GetCollateralCallAsync(
        GetCollateralRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BitflyerEndpointIds.GetCollateral),
            BitflyerPrivateEndpoints.GetCollateral(),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<CollateralResponse>(
                json,
                Component(BitflyerEndpointIds.GetCollateral)));

    public Task<Call<GetCollateralAccountsRequest, IReadOnlyList<CollateralAccount>>> GetCollateralAccountsCallAsync(
        GetCollateralAccountsRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BitflyerEndpointIds.GetCollateralAccounts),
            BitflyerPrivateEndpoints.GetCollateralAccounts(),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<CollateralAccount>>(
                json,
                Component(BitflyerEndpointIds.GetCollateralAccounts)));

    public Task<Call<GetBalanceHistoryRequest, RawJsonResponse>> GetBalanceHistoryCallAsync(
        GetBalanceHistoryRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BitflyerEndpointIds.GetBalanceHistory),
            BitflyerPrivateEndpoints.GetBalanceHistory(
                request.CurrencyCode,
                request.Count?.ToString(CultureInfo.InvariantCulture),
                request.Before?.ToString(CultureInfo.InvariantCulture),
                request.After?.ToString(CultureInfo.InvariantCulture)),
            cancellationToken,
            json => new RawJsonResponse(json));

    public Task<Call<GetTradingCommissionRequest, RawJsonResponse>> GetTradingCommissionCallAsync(
        GetTradingCommissionRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BitflyerEndpointIds.GetTradingCommission),
            BitflyerPrivateEndpoints.GetTradingCommission(request.ProductCode),
            cancellationToken,
            json => new RawJsonResponse(json));

    public Task<Call<GetCollateralHistoryRequest, RawJsonResponse>> GetCollateralHistoryCallAsync(
        GetCollateralHistoryRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BitflyerEndpointIds.GetCollateralHistory),
            BitflyerPrivateEndpoints.GetCollateralHistory(
                request.Count?.ToString(CultureInfo.InvariantCulture),
                request.Before?.ToString(CultureInfo.InvariantCulture),
                request.After?.ToString(CultureInfo.InvariantCulture)),
            cancellationToken,
            json => new RawJsonResponse(json));

    public Task<Call<GetAddressesRequest, RawJsonResponse>> GetAddressesCallAsync(
        GetAddressesRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BitflyerEndpointIds.GetAddresses),
            BitflyerPrivateEndpoints.GetAddresses(),
            cancellationToken,
            json => new RawJsonResponse(json));

    public Task<Call<GetCoinInsRequest, RawJsonResponse>> GetCoinInsCallAsync(
        GetCoinInsRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BitflyerEndpointIds.GetCoinIns),
            BitflyerPrivateEndpoints.GetCoinIns(
                request.Count?.ToString(CultureInfo.InvariantCulture),
                request.Before?.ToString(CultureInfo.InvariantCulture),
                request.After?.ToString(CultureInfo.InvariantCulture)),
            cancellationToken,
            json => new RawJsonResponse(json));

    public Task<Call<GetCoinOutsRequest, RawJsonResponse>> GetCoinOutsCallAsync(
        GetCoinOutsRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BitflyerEndpointIds.GetCoinOuts),
            BitflyerPrivateEndpoints.GetCoinOuts(
                request.MessageId,
                request.Count?.ToString(CultureInfo.InvariantCulture),
                request.Before?.ToString(CultureInfo.InvariantCulture),
                request.After?.ToString(CultureInfo.InvariantCulture)),
            cancellationToken,
            json => new RawJsonResponse(json));

    public Task<Call<GetDepositsRequest, RawJsonResponse>> GetDepositsCallAsync(
        GetDepositsRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BitflyerEndpointIds.GetDeposits),
            BitflyerPrivateEndpoints.GetDeposits(
                request.Count?.ToString(CultureInfo.InvariantCulture),
                request.Before?.ToString(CultureInfo.InvariantCulture),
                request.After?.ToString(CultureInfo.InvariantCulture)),
            cancellationToken,
            json => new RawJsonResponse(json));

    public Task<Call<GetWithdrawalsRequest, RawJsonResponse>> GetWithdrawalsCallAsync(
        GetWithdrawalsRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BitflyerEndpointIds.GetWithdrawals),
            BitflyerPrivateEndpoints.GetWithdrawals(
                messageId: null,
                count: request.Count?.ToString(CultureInfo.InvariantCulture),
                before: request.Before?.ToString(CultureInfo.InvariantCulture),
                after: request.After?.ToString(CultureInfo.InvariantCulture)),
            cancellationToken,
            json => new RawJsonResponse(json));

    public Task<Call<GetBankAccountsRequest, RawJsonResponse>> GetBankAccountsCallAsync(
        GetBankAccountsRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BitflyerEndpointIds.GetBankAccounts),
            BitflyerPrivateEndpoints.GetBankAccounts(),
            cancellationToken,
            json => new RawJsonResponse(json));

    public Task<Call<CreateWithdrawalRequest, CreateWithdrawalResponse>> WithdrawCallAsync(
        CreateWithdrawalRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BitflyerEndpointIds.Withdraw),
            BitflyerPrivateEndpoints.Withdraw(
                BitflyerRawJson.SerializeOrThrow(request, Component(BitflyerEndpointIds.Withdraw))),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<CreateWithdrawalResponse>(
                json,
                Component(BitflyerEndpointIds.Withdraw)));

    public Task<Call<CreateChildOrderRequest, RawSendChildOrderResponse>> SendChildOrderCallAsync(
        CreateChildOrderRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BitflyerEndpointIds.SendChildOrder),
            BitflyerPrivateEndpoints.SendChildOrder(
                BitflyerRawJson.SerializeOrThrow(
                    BitflyerRawMappers.MapSendChildOrderRequest(request),
                    Component(BitflyerEndpointIds.SendChildOrder))),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<RawSendChildOrderResponse>(
                json,
                Component(BitflyerEndpointIds.SendChildOrder)));

    public Task<Call<CreateParentOrderRequest, RawSendParentOrderResponse>> SendParentOrderCallAsync(
        CreateParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BitflyerEndpointIds.SendParentOrder),
            BitflyerPrivateEndpoints.SendParentOrder(
                BitflyerRawJson.SerializeOrThrow(
                    BitflyerRawMappers.MapSendParentOrderRequest(request),
                    Component(BitflyerEndpointIds.SendParentOrder))),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<RawSendParentOrderResponse>(
                json,
                Component(BitflyerEndpointIds.SendParentOrder)));

    public Task<Call<CancelChildOrderRequest, RawCancelChildOrderResponse>> CancelChildOrderCallAsync(
        CancelChildOrderRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BitflyerEndpointIds.CancelChildOrder),
            BitflyerPrivateEndpoints.CancelChildOrder(
                BitflyerRawJson.SerializeOrThrow(request, Component(BitflyerEndpointIds.CancelChildOrder))),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<RawCancelChildOrderResponse>(
                json,
                Component(BitflyerEndpointIds.CancelChildOrder)));

    public Task<Call<CancelParentOrderRequest, RawCancelParentOrderResponse>> CancelParentOrderCallAsync(
        CancelParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BitflyerEndpointIds.CancelParentOrder),
            BitflyerPrivateEndpoints.CancelParentOrder(
                BitflyerRawJson.SerializeOrThrow(request, Component(BitflyerEndpointIds.CancelParentOrder))),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<RawCancelParentOrderResponse>(
                json,
                Component(BitflyerEndpointIds.CancelParentOrder)));

    public Task<Call<CancelAllChildOrdersRequest, RawCancelAllChildOrdersResponse>> CancelAllChildOrdersCallAsync(
        CancelAllChildOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BitflyerEndpointIds.CancelAllChildOrders),
            BitflyerPrivateEndpoints.CancelAllChildOrders(
                BitflyerRawJson.SerializeOrThrow(request, Component(BitflyerEndpointIds.CancelAllChildOrders))),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<RawCancelAllChildOrdersResponse>(
                json,
                Component(BitflyerEndpointIds.CancelAllChildOrders)));

    public Task<Call<GetChildOrdersRequest, IReadOnlyList<RawGetChildOrdersResponse>>> GetChildOrdersCallAsync(
        GetChildOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BitflyerEndpointIds.GetChildOrders),
            BitflyerPrivateEndpoints.GetChildOrders(
                request.ProductCode,
                request.ChildOrderStatusState,
                request.ChildOrderAcceptanceId,
                request.ChildOrderId,
                request.Count?.ToString(CultureInfo.InvariantCulture),
                request.Before?.ToString(CultureInfo.InvariantCulture),
                request.After?.ToString(CultureInfo.InvariantCulture),
                request.ParentOrderId),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<RawGetChildOrdersResponse>>(
                json,
                Component(BitflyerEndpointIds.GetChildOrders)));

    public Task<Call<GetParentOrdersRequest, IReadOnlyList<RawGetParentOrdersResponse>>> GetParentOrdersCallAsync(
        GetParentOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BitflyerEndpointIds.GetParentOrders),
            BitflyerPrivateEndpoints.GetParentOrders(
                request.ProductCode,
                request.ParentOrderState,
                request.Count?.ToString(CultureInfo.InvariantCulture),
                request.Before?.ToString(CultureInfo.InvariantCulture),
                request.After?.ToString(CultureInfo.InvariantCulture)),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<RawGetParentOrdersResponse>>(
                json,
                Component(BitflyerEndpointIds.GetParentOrders)));

    public Task<Call<GetParentOrderRequest, RawGetParentOrderResponse>> GetParentOrderCallAsync(
        GetParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(BitflyerEndpointIds.GetParentOrder),
            BitflyerPrivateEndpoints.GetParentOrder(
                request.ParentOrderId,
                request.ParentOrderAcceptanceId),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<RawGetParentOrderResponse>(
                json,
                Component(BitflyerEndpointIds.GetParentOrder)));

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

    private static string Component(string endpointId) => $"Bitflyer.{endpointId}";
}
