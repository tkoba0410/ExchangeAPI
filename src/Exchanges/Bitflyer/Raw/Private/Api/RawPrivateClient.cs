using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal;
using ExchangeApi.Exchanges.Common.Raw.Api;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private.Requests;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Exchanges.Bitflyer.Wire.Internal;
using ExchangeApi.Exchanges.Bitflyer.Wire.Private.Endpoints;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Private.Api;

internal sealed class RawPrivateClient
{
    private readonly IWireCallExecutor _wire;
    private readonly RawCallExecutor _executor;

    public RawPrivateClient(IWireCallExecutor wire, RawCallExecutor executor)
    {
        _wire = wire ?? throw new System.ArgumentNullException(nameof(wire));
        _executor = executor ?? throw new System.ArgumentNullException(nameof(executor));
    }

    public Task<Call<GetPermissionsRequest, GetPermissionsResponse>> GetPermissionsCallAsync(
        GetPermissionsRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(EndpointIds.GetPermissions),
            PrivateEndpoints.GetPermissions(),
            cancellationToken,
            json => RawJson.DeserializeOrThrow<GetPermissionsResponse>(
                json,
                Component(EndpointIds.GetPermissions)));

    public Task<Call<GetBalanceRequest, GetBalanceResponse>> GetBalanceCallAsync(
        GetBalanceRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(EndpointIds.GetBalance),
            PrivateEndpoints.GetBalance(),
            cancellationToken,
            json => RawJson.DeserializeOrThrow<GetBalanceResponse>(
                json,
                Component(EndpointIds.GetBalance)));

    public Task<Call<GetPositionsRequest, GetPositionsResponse>> GetPositionsCallAsync(
        GetPositionsRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(EndpointIds.GetPositions),
            PrivateEndpoints.GetPositions(request.ProductCode.Value),
            cancellationToken,
            json => RawJson.DeserializeOrThrow<GetPositionsResponse>(
                json,
                Component(EndpointIds.GetPositions)));

    public Task<Call<GetExecutionsPrivateRequest, GetExecutionsPrivateResponse>> GetExecutionsPrivateCallAsync(
        GetExecutionsPrivateRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(EndpointIds.GetExecutionsPrivate),
            PrivateEndpoints.GetExecutionsPrivate(
                request.ProductCode.Value,
                request.ChildOrderId?.Value,
                request.ChildOrderAcceptanceId?.Value,
                request.Count?.ToString(CultureInfo.InvariantCulture),
                request.Before?.ToString(CultureInfo.InvariantCulture),
                request.After?.ToString(CultureInfo.InvariantCulture)),
            cancellationToken,
            json => RawJson.DeserializeOrThrow<GetExecutionsPrivateResponse>(
                json,
                Component(EndpointIds.GetExecutionsPrivate)));

    public Task<Call<GetCollateralRequest, GetCollateralResponse>> GetCollateralCallAsync(
        GetCollateralRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(EndpointIds.GetCollateral),
            PrivateEndpoints.GetCollateral(),
            cancellationToken,
            json => RawJson.DeserializeOrThrow<GetCollateralResponse>(
                json,
                Component(EndpointIds.GetCollateral)));

    public Task<Call<GetCollateralAccountsRequest, GetCollateralAccountsResponse>> GetCollateralAccountsCallAsync(
        GetCollateralAccountsRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(EndpointIds.GetCollateralAccounts),
            PrivateEndpoints.GetCollateralAccounts(),
            cancellationToken,
            json => RawJson.DeserializeOrThrow<GetCollateralAccountsResponse>(
                json,
                Component(EndpointIds.GetCollateralAccounts)));

    public Task<Call<GetBalanceHistoryRequest, GetBalanceHistoryResponse>> GetBalanceHistoryCallAsync(
        GetBalanceHistoryRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(EndpointIds.GetBalanceHistory),
            PrivateEndpoints.GetBalanceHistory(
                request.CurrencyCode.HasValue ? CurrencyCodeConverter.ToCurrencyString(request.CurrencyCode.Value) : null,
                request.Count?.ToString(CultureInfo.InvariantCulture),
                request.Before?.ToString(CultureInfo.InvariantCulture),
                request.After?.ToString(CultureInfo.InvariantCulture)),
            cancellationToken,
            json => new GetBalanceHistoryResponse(json));

    public Task<Call<GetTradingCommissionRequest, GetTradingCommissionResponse>> GetTradingCommissionCallAsync(
        GetTradingCommissionRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(EndpointIds.GetTradingCommission),
            PrivateEndpoints.GetTradingCommission(request.ProductCode.Value),
            cancellationToken,
            json => new GetTradingCommissionResponse(json));

    public Task<Call<GetCollateralHistoryRequest, GetCollateralHistoryResponse>> GetCollateralHistoryCallAsync(
        GetCollateralHistoryRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(EndpointIds.GetCollateralHistory),
            PrivateEndpoints.GetCollateralHistory(
                request.Count?.ToString(CultureInfo.InvariantCulture),
                request.Before?.ToString(CultureInfo.InvariantCulture),
                request.After?.ToString(CultureInfo.InvariantCulture)),
            cancellationToken,
            json => new GetCollateralHistoryResponse(json));

    public Task<Call<GetAddressesRequest, GetAddressesResponse>> GetAddressesCallAsync(
        GetAddressesRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(EndpointIds.GetAddresses),
            PrivateEndpoints.GetAddresses(),
            cancellationToken,
            json => new GetAddressesResponse(json));

    public Task<Call<GetCoinInsRequest, GetCoinInsResponse>> GetCoinInsCallAsync(
        GetCoinInsRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(EndpointIds.GetCoinIns),
            PrivateEndpoints.GetCoinIns(
                request.Count?.ToString(CultureInfo.InvariantCulture),
                request.Before?.ToString(CultureInfo.InvariantCulture),
                request.After?.ToString(CultureInfo.InvariantCulture)),
            cancellationToken,
            json => new GetCoinInsResponse(json));

    public Task<Call<GetCoinOutsRequest, GetCoinOutsResponse>> GetCoinOutsCallAsync(
        GetCoinOutsRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(EndpointIds.GetCoinOuts),
            PrivateEndpoints.GetCoinOuts(
                request.MessageId?.Value,
                request.Count?.ToString(CultureInfo.InvariantCulture),
                request.Before?.ToString(CultureInfo.InvariantCulture),
                request.After?.ToString(CultureInfo.InvariantCulture)),
            cancellationToken,
            json => new GetCoinOutsResponse(json));

    public Task<Call<GetDepositsRequest, GetDepositsResponse>> GetDepositsCallAsync(
        GetDepositsRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(EndpointIds.GetDeposits),
            PrivateEndpoints.GetDeposits(
                request.Count?.ToString(CultureInfo.InvariantCulture),
                request.Before?.ToString(CultureInfo.InvariantCulture),
                request.After?.ToString(CultureInfo.InvariantCulture)),
            cancellationToken,
            json => new GetDepositsResponse(json));

    public Task<Call<GetWithdrawalsRequest, GetWithdrawalsResponse>> GetWithdrawalsCallAsync(
        GetWithdrawalsRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(EndpointIds.GetWithdrawals),
            PrivateEndpoints.GetWithdrawals(
                messageId: null,
                count: request.Count?.ToString(CultureInfo.InvariantCulture),
                before: request.Before?.ToString(CultureInfo.InvariantCulture),
                after: request.After?.ToString(CultureInfo.InvariantCulture)),
            cancellationToken,
            json => new GetWithdrawalsResponse(json));

    public Task<Call<GetBankAccountsRequest, GetBankAccountsResponse>> GetBankAccountsCallAsync(
        GetBankAccountsRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(EndpointIds.GetBankAccounts),
            PrivateEndpoints.GetBankAccounts(),
            cancellationToken,
            json => new GetBankAccountsResponse(json));

    public Task<Call<WithdrawRequest, WithdrawResponse>> WithdrawCallAsync(
        WithdrawRequest request,
        CancellationToken cancellationToken = default) =>
        TryBuildSpec(
            request,
            Component(EndpointIds.Withdraw),
            () =>
            {
                if (!RawJson.TrySerialize(request, out var body, out var error))
                {
                    return (Spec: (WireCallSpec?)null, Error: error);
                }

                return (Spec: PrivateEndpoints.Withdraw(body!), Error: (Exception?)null);
            },
            cancellationToken,
            json => RawJson.DeserializeOrThrow<WithdrawResponse>(
                json,
                Component(EndpointIds.Withdraw)));

    public Task<Call<SendChildOrderRequest, SendChildOrderResponse>> SendChildOrderCallAsync(
        SendChildOrderRequest request,
        CancellationToken cancellationToken = default) =>
        TryBuildSpec(
            request,
            Component(EndpointIds.SendChildOrder),
            () =>
            {
                var bodyModel = RawMappers.MapSendChildOrderRequest(request);
                if (!RawJson.TrySerialize(bodyModel, out var body, out var error))
                {
                    return (Spec: (WireCallSpec?)null, Error: error);
                }

                return (Spec: PrivateEndpoints.SendChildOrder(body!), Error: (Exception?)null);
            },
            cancellationToken,
            json => RawJson.DeserializeOrThrow<SendChildOrderResponse>(
                json,
                Component(EndpointIds.SendChildOrder)));

    public Task<Call<SendParentOrderRequest, SendParentOrderResponse>> SendParentOrderCallAsync(
        SendParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        TryBuildSpec(
            request,
            Component(EndpointIds.SendParentOrder),
            () =>
            {
                var bodyModel = RawMappers.MapSendParentOrderRequest(request);
                if (!RawJson.TrySerialize(bodyModel, out var body, out var error))
                {
                    return (Spec: (WireCallSpec?)null, Error: error);
                }

                return (Spec: PrivateEndpoints.SendParentOrder(body!), Error: (Exception?)null);
            },
            cancellationToken,
            json => RawJson.DeserializeOrThrow<SendParentOrderResponse>(
                json,
                Component(EndpointIds.SendParentOrder)));

    public Task<Call<CancelChildOrderRequest, CancelChildOrderResponse>> CancelChildOrderCallAsync(
        CancelChildOrderRequest request,
        CancellationToken cancellationToken = default) =>
        TryBuildSpec(
            request,
            Component(EndpointIds.CancelChildOrder),
            () =>
            {
                if (!RawJson.TrySerialize(request, out var body, out var error))
                {
                    return (Spec: (WireCallSpec?)null, Error: error);
                }

                return (Spec: PrivateEndpoints.CancelChildOrder(body!), Error: (Exception?)null);
            },
            cancellationToken,
            json => RawJson.DeserializeOrThrow<CancelChildOrderResponse>(
                json,
                Component(EndpointIds.CancelChildOrder)));

    public Task<Call<CancelParentOrderRequest, CancelParentOrderResponse>> CancelParentOrderCallAsync(
        CancelParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        TryBuildSpec(
            request,
            Component(EndpointIds.CancelParentOrder),
            () =>
            {
                if (!RawJson.TrySerialize(request, out var body, out var error))
                {
                    return (Spec: (WireCallSpec?)null, Error: error);
                }

                return (Spec: PrivateEndpoints.CancelParentOrder(body!), Error: (Exception?)null);
            },
            cancellationToken,
            json => RawJson.DeserializeOrThrow<CancelParentOrderResponse>(
                json,
                Component(EndpointIds.CancelParentOrder)));

    public Task<Call<CancelAllChildOrdersRequest, CancelAllChildOrdersResponse>> CancelAllChildOrdersCallAsync(
        CancelAllChildOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        TryBuildSpec(
            request,
            Component(EndpointIds.CancelAllChildOrders),
            () =>
            {
                if (!RawJson.TrySerialize(request, out var body, out var error))
                {
                    return (Spec: (WireCallSpec?)null, Error: error);
                }

                return (Spec: PrivateEndpoints.CancelAllChildOrders(body!), Error: (Exception?)null);
            },
            cancellationToken,
            json => RawJson.DeserializeOrThrow<CancelAllChildOrdersResponse>(
                json,
                Component(EndpointIds.CancelAllChildOrders)));

    public Task<Call<GetChildOrdersRequest, GetChildOrdersResponse>> GetChildOrdersCallAsync(
        GetChildOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(EndpointIds.GetChildOrders),
            PrivateEndpoints.GetChildOrders(
                request.ProductCode.Value,
                request.ChildOrderStatusState?.Value,
                request.ChildOrderAcceptanceId?.Value,
                request.ChildOrderId?.Value,
                request.Count?.ToString(CultureInfo.InvariantCulture),
                request.Before?.ToString(CultureInfo.InvariantCulture),
                request.After?.ToString(CultureInfo.InvariantCulture),
                request.ParentOrderId?.Value),
            cancellationToken,
            json => RawJson.DeserializeOrThrow<GetChildOrdersResponse>(
                json,
                Component(EndpointIds.GetChildOrders)));

    public Task<Call<GetParentOrdersRequest, GetParentOrdersResponse>> GetParentOrdersCallAsync(
        GetParentOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(EndpointIds.GetParentOrders),
            PrivateEndpoints.GetParentOrders(
                request.ProductCode.Value,
                request.ParentOrderState?.Value,
                request.Count?.ToString(CultureInfo.InvariantCulture),
                request.Before?.ToString(CultureInfo.InvariantCulture),
                request.After?.ToString(CultureInfo.InvariantCulture)),
            cancellationToken,
            json => RawJson.DeserializeOrThrow<GetParentOrdersResponse>(
                json,
                Component(EndpointIds.GetParentOrders)));

    public Task<Call<GetParentOrderRequest, GetParentOrderResponse>> GetParentOrderCallAsync(
        GetParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            Component(EndpointIds.GetParentOrder),
            PrivateEndpoints.GetParentOrder(
                request.ParentOrderId?.Value,
                request.ParentOrderAcceptanceId?.Value),
            cancellationToken,
            json => RawJson.DeserializeOrThrow<GetParentOrderResponse>(
                json,
                Component(EndpointIds.GetParentOrder)));

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
        var meta = CallMeta.CreateInternal(CallMetaVocabulary.Layer.Raw, component);
        var now = DateTimeOffset.UtcNow;

        return new Call<TReq, TRes>(
            Id: CallId.New(),
            StartedAt: now,
            Duration: TimeSpan.Zero,
            Request: request,
            Result: new CallResult<TRes>.Err(callError),
            Meta: meta);
    }

    private static string Component(string endpointId) => $"Bitflyer.{endpointId}";
}
