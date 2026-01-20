using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private;
using ExchangeApi.Exchanges.Bitflyer.Raw.Requests;
using ExchangeApi.Exchanges.Bitflyer.Wire.Endpoints;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Call;

/// <summary>
/// bitFlyer Private REST API（情報系）の実装。
/// </summary>
public sealed class BitflyerPrivateApi : IBitflyerPrivateApi
{
    private readonly IWireTransport _wire;

    public BitflyerPrivateApi(IWireTransport wire)
    {
        _wire = wire ?? throw new ArgumentNullException(nameof(wire));
    }

    public Task<Call<GetPermissionsRequest, IReadOnlyList<string>>> GetPermissionsAsync(
        GetPermissionsRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bitflyer.GetPermissions",
            BitflyerEndpoints.GetPermissions(),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<string>>(
                json,
                "Bitflyer.GetPermissions"));

    public Task<Call<GetBalancesRequest, IReadOnlyList<BalanceResponse>>> GetBalancesAsync(
        GetBalancesRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bitflyer.GetBalance",
            BitflyerEndpoints.GetBalances(),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<BalanceResponse>>(
                json,
                "Bitflyer.GetBalance"));

    public Task<Call<GetPositionsRequest, IReadOnlyList<PositionResponse>>> GetPositionsAsync(
        GetPositionsRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bitflyer.GetPositions",
            BitflyerEndpoints.GetPositions(request.ProductCode),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<PositionResponse>>(
                json,
                "Bitflyer.GetPositions"));

    public Task<Call<GetAccountExecutionsRequest, IReadOnlyList<ExecutionPrivateResponse>>> GetExecutionsAsync(
        GetAccountExecutionsRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bitflyer.GetExecutions",
            BitflyerEndpoints.GetExecutions(
                request.ProductCode,
                request.ChildOrderId,
                request.ChildOrderAcceptanceId,
                request.Count?.ToString(CultureInfo.InvariantCulture),
                request.Before?.ToString(CultureInfo.InvariantCulture),
                request.After?.ToString(CultureInfo.InvariantCulture)),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<ExecutionPrivateResponse>>(
                json,
                "Bitflyer.GetExecutions"));

    public Task<Call<GetCollateralRequest, CollateralResponse>> GetCollateralAsync(
        GetCollateralRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bitflyer.GetCollateral",
            BitflyerEndpoints.GetCollateral(),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<CollateralResponse>(
                json,
                "Bitflyer.GetCollateral"));

    public Task<Call<GetCollateralAccountsRequest, IReadOnlyList<CollateralAccount>>> GetCollateralAccountsAsync(
        GetCollateralAccountsRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bitflyer.GetCollateralAccounts",
            BitflyerEndpoints.GetCollateralAccounts(),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<CollateralAccount>>(
                json,
                "Bitflyer.GetCollateralAccounts"));

    public Task<Call<GetChildOrdersRequest, IReadOnlyList<ChildOrderResponse>>> GetChildOrdersAsync(
        GetChildOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bitflyer.GetChildOrders",
            BitflyerEndpoints.GetChildOrders(
                request.ProductCode,
                request.ChildOrderStatusState,
                request.ChildOrderAcceptanceId,
                request.ChildOrderId,
                request.Count?.ToString(CultureInfo.InvariantCulture),
                request.Before?.ToString(CultureInfo.InvariantCulture),
                request.After?.ToString(CultureInfo.InvariantCulture),
                request.ParentOrderId),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<ChildOrderResponse>>(
                json,
                "Bitflyer.GetChildOrders"));

    public Task<Call<GetParentOrdersRequest, IReadOnlyList<ParentOrderResponse>>> GetParentOrdersAsync(
        GetParentOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bitflyer.GetParentOrders",
            BitflyerEndpoints.GetParentOrders(
                request.ProductCode,
                request.ParentOrderState,
                request.Count?.ToString(CultureInfo.InvariantCulture),
                request.Before?.ToString(CultureInfo.InvariantCulture),
                request.After?.ToString(CultureInfo.InvariantCulture)),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<ParentOrderResponse>>(
                json,
                "Bitflyer.GetParentOrders"));

    public Task<Call<GetParentOrderRequest, ParentOrderDetailResponse>> GetParentOrderAsync(
        GetParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bitflyer.GetParentOrder",
            BitflyerEndpoints.GetParentOrder(
                request.ParentOrderId,
                request.ParentOrderAcceptanceId),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<ParentOrderDetailResponse>(
                json,
                "Bitflyer.GetParentOrder"));

    public Task<Call<GetBalanceHistoryRequest, RawJsonResponse>> GetBalanceHistoryAsync(
        GetBalanceHistoryRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bitflyer.GetBalanceHistory",
            BitflyerEndpoints.GetBalanceHistory(
                request.CurrencyCode,
                request.Count?.ToString(CultureInfo.InvariantCulture),
                request.Before?.ToString(CultureInfo.InvariantCulture),
                request.After?.ToString(CultureInfo.InvariantCulture)),
            cancellationToken,
            json => new RawJsonResponse(json));

    public Task<Call<GetTradingCommissionRequest, RawJsonResponse>> GetTradingCommissionAsync(
        GetTradingCommissionRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bitflyer.GetTradingCommission",
            BitflyerEndpoints.GetTradingCommission(request.ProductCode),
            cancellationToken,
            json => new RawJsonResponse(json));

    public Task<Call<GetCollateralHistoryRequest, RawJsonResponse>> GetCollateralHistoryAsync(
        GetCollateralHistoryRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bitflyer.GetCollateralHistory",
            BitflyerEndpoints.GetCollateralHistory(
                request.Count?.ToString(CultureInfo.InvariantCulture),
                request.Before?.ToString(CultureInfo.InvariantCulture),
                request.After?.ToString(CultureInfo.InvariantCulture)),
            cancellationToken,
            json => new RawJsonResponse(json));

    public Task<Call<GetAddressesRequest, RawJsonResponse>> GetAddressesAsync(
        GetAddressesRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bitflyer.GetAddresses",
            BitflyerEndpoints.GetAddresses(),
            cancellationToken,
            json => new RawJsonResponse(json));

    public Task<Call<GetCoinInsRequest, RawJsonResponse>> GetCoinInsAsync(
        GetCoinInsRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bitflyer.GetCoinIns",
            BitflyerEndpoints.GetCoinIns(
                request.Count?.ToString(CultureInfo.InvariantCulture),
                request.Before?.ToString(CultureInfo.InvariantCulture),
                request.After?.ToString(CultureInfo.InvariantCulture)),
            cancellationToken,
            json => new RawJsonResponse(json));

    public Task<Call<GetCoinOutsRequest, RawJsonResponse>> GetCoinOutsAsync(
        GetCoinOutsRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bitflyer.GetCoinOuts",
            BitflyerEndpoints.GetCoinOuts(
                request.MessageId,
                request.Count?.ToString(CultureInfo.InvariantCulture),
                request.Before?.ToString(CultureInfo.InvariantCulture),
                request.After?.ToString(CultureInfo.InvariantCulture)),
            cancellationToken,
            json => new RawJsonResponse(json));

    public Task<Call<GetDepositsRequest, RawJsonResponse>> GetDepositsAsync(
        GetDepositsRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bitflyer.GetDeposits",
            BitflyerEndpoints.GetDeposits(
                request.Count?.ToString(CultureInfo.InvariantCulture),
                request.Before?.ToString(CultureInfo.InvariantCulture),
                request.After?.ToString(CultureInfo.InvariantCulture)),
            cancellationToken,
            json => new RawJsonResponse(json));

    public Task<Call<GetWithdrawalsRequest, RawJsonResponse>> GetWithdrawalsAsync(
        GetWithdrawalsRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bitflyer.GetWithdrawals",
            BitflyerEndpoints.GetWithdrawals(
                messageId: null,
                count: request.Count?.ToString(CultureInfo.InvariantCulture),
                before: request.Before?.ToString(CultureInfo.InvariantCulture),
                after: request.After?.ToString(CultureInfo.InvariantCulture)),
            cancellationToken,
            json => new RawJsonResponse(json));

    public Task<Call<GetBankAccountsRequest, RawJsonResponse>> GetBankAccountsAsync(
        GetBankAccountsRequest request,
        CancellationToken cancellationToken = default) =>
        SendAndParse(
            request,
            "Bitflyer.GetBankAccounts",
            BitflyerEndpoints.GetBankAccounts(),
            cancellationToken,
            json => new RawJsonResponse(json));

    private async Task<Call<TReq, TRes>> SendAndParse<TReq, TRes>(
        TReq request,
        string component,
        WireCallSpec spec,
        CancellationToken cancellationToken,
        Func<string, TRes> parse)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        if (parse is null) throw new ArgumentNullException(nameof(parse));

        var wireCall = await _wire.SendAsync(ExchangeCode.Bitflyer, spec, cancellationToken).ConfigureAwait(false);
        return CreateCall(request, component, wireCall, parse);
    }

    private static Call<TReq, TRes> CreateCall<TReq, TRes>(
        TReq request,
        string component,
        Call<WireCallSpec, WireResponse> wireCall,
        Func<string, TRes> parse)
    {
        return wireCall.Result switch
        {
            CallResult<WireResponse>.Err err => new Call<TReq, TRes>(
                Id: CallId.New(),
                StartedAt: wireCall.StartedAt,
                Duration: wireCall.Duration,
                Request: request,
                Result: new CallResult<TRes>.Err(err.Error),
                Meta: wireCall.Meta),
            CallResult<WireResponse>.Ok ok => CreateOkCall(request, component, ok.Response, wireCall, parse),
            _ => new Call<TReq, TRes>(
                Id: CallId.New(),
                StartedAt: wireCall.StartedAt,
                Duration: wireCall.Duration,
                Request: request,
                Result: new CallResult<TRes>.Err(new CallError(CallErrorKind.Unknown, "Wire call returned unknown result.")),
                Meta: wireCall.Meta)
        };
    }

    private static Call<TReq, TRes> CreateOkCall<TReq, TRes>(
        TReq request,
        string component,
        WireResponse response,
        Call<WireCallSpec, WireResponse> wireCall,
        Func<string, TRes> parse)
    {
        if (response.StatusCode is < 200 or >= 300)
        {
            var error = new CallError(
                CallErrorKind.Http,
                $"{component} failed with status {response.StatusCode}.",
                HttpStatus: response.StatusCode,
                BodySnippet: Snip(response.Json));
            return new Call<TReq, TRes>(
                Id: CallId.New(),
                StartedAt: wireCall.StartedAt,
                Duration: wireCall.Duration,
                Request: request,
                Result: new CallResult<TRes>.Err(error),
                Meta: wireCall.Meta);
        }

        try
        {
            var parsed = parse(response.Json);
            return new Call<TReq, TRes>(
                Id: CallId.New(),
                StartedAt: wireCall.StartedAt,
                Duration: wireCall.Duration,
                Request: request,
                Result: new CallResult<TRes>.Ok(parsed),
                Meta: wireCall.Meta);
        }
        catch (Exception ex) when (ex is JsonException or NotSupportedException)
        {
            var error = new CallError(
                CallErrorKind.Codec,
                $"{component} failed to parse response.",
                ex,
                response.StatusCode,
                Snip(response.Json));
            return new Call<TReq, TRes>(
                Id: CallId.New(),
                StartedAt: wireCall.StartedAt,
                Duration: wireCall.Duration,
                Request: request,
                Result: new CallResult<TRes>.Err(error),
                Meta: wireCall.Meta);
        }
    }

    private static string? Snip(string? json)
    {
        if (string.IsNullOrEmpty(json)) return json;
        return json.Length <= 512 ? json : json[..512];
    }
}
