using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private.Requests;
using ExchangeApi.Exchanges.Bitflyer.Wire.Private.Endpoints;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Private.Api;

internal sealed class BitflyerRawPrivateClient
{
    private readonly BitflyerRawCallExecutor _executor;

    public BitflyerRawPrivateClient(BitflyerRawCallExecutor executor)
    {
        _executor = executor;
    }

    public Task<Call<GetPermissionsRequest, IReadOnlyList<string>>> GetPermissionsCallAsync(
        GetPermissionsRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bitflyer.GetPermissions",
            BitflyerPrivateEndpoints.GetPermissions(),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<string>>(
                json,
                "Bitflyer.GetPermissions"));

    public Task<Call<GetBalancesRequest, IReadOnlyList<BalanceResponse>>> GetBalanceCallAsync(
        GetBalancesRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bitflyer.GetBalance",
            BitflyerPrivateEndpoints.GetBalance(),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<BalanceResponse>>(
                json,
                "Bitflyer.GetBalance"));

    public Task<Call<GetPositionsRequest, IReadOnlyList<PositionResponse>>> GetPositionsCallAsync(
        GetPositionsRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bitflyer.GetPositions",
            BitflyerPrivateEndpoints.GetPositions(request.ProductCode),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<PositionResponse>>(
                json,
                "Bitflyer.GetPositions"));

    public Task<Call<GetAccountExecutionsRequest, IReadOnlyList<ExecutionPrivateResponse>>> GetExecutionsPrivateCallAsync(
        GetAccountExecutionsRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bitflyer.GetExecutions",
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
                "Bitflyer.GetExecutions"));

    public Task<Call<GetCollateralRequest, CollateralResponse>> GetCollateralCallAsync(
        GetCollateralRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bitflyer.GetCollateral",
            BitflyerPrivateEndpoints.GetCollateral(),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<CollateralResponse>(
                json,
                "Bitflyer.GetCollateral"));

    public Task<Call<GetCollateralAccountsRequest, IReadOnlyList<CollateralAccount>>> GetCollateralAccountsCallAsync(
        GetCollateralAccountsRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bitflyer.GetCollateralAccounts",
            BitflyerPrivateEndpoints.GetCollateralAccounts(),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<CollateralAccount>>(
                json,
                "Bitflyer.GetCollateralAccounts"));

    public Task<Call<GetBalanceHistoryRequest, RawJsonResponse>> GetBalanceHistoryCallAsync(
        GetBalanceHistoryRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bitflyer.GetBalanceHistory",
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
        _executor.SendAndParse(
            request,
            "Bitflyer.GetTradingCommission",
            BitflyerPrivateEndpoints.GetTradingCommission(request.ProductCode),
            cancellationToken,
            json => new RawJsonResponse(json));

    public Task<Call<GetCollateralHistoryRequest, RawJsonResponse>> GetCollateralHistoryCallAsync(
        GetCollateralHistoryRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bitflyer.GetCollateralHistory",
            BitflyerPrivateEndpoints.GetCollateralHistory(
                request.Count?.ToString(CultureInfo.InvariantCulture),
                request.Before?.ToString(CultureInfo.InvariantCulture),
                request.After?.ToString(CultureInfo.InvariantCulture)),
            cancellationToken,
            json => new RawJsonResponse(json));

    public Task<Call<GetAddressesRequest, RawJsonResponse>> GetAddressesCallAsync(
        GetAddressesRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bitflyer.GetAddresses",
            BitflyerPrivateEndpoints.GetAddresses(),
            cancellationToken,
            json => new RawJsonResponse(json));

    public Task<Call<GetCoinInsRequest, RawJsonResponse>> GetCoinInsCallAsync(
        GetCoinInsRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bitflyer.GetCoinIns",
            BitflyerPrivateEndpoints.GetCoinIns(
                request.Count?.ToString(CultureInfo.InvariantCulture),
                request.Before?.ToString(CultureInfo.InvariantCulture),
                request.After?.ToString(CultureInfo.InvariantCulture)),
            cancellationToken,
            json => new RawJsonResponse(json));

    public Task<Call<GetCoinOutsRequest, RawJsonResponse>> GetCoinOutsCallAsync(
        GetCoinOutsRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bitflyer.GetCoinOuts",
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
        _executor.SendAndParse(
            request,
            "Bitflyer.GetDeposits",
            BitflyerPrivateEndpoints.GetDeposits(
                request.Count?.ToString(CultureInfo.InvariantCulture),
                request.Before?.ToString(CultureInfo.InvariantCulture),
                request.After?.ToString(CultureInfo.InvariantCulture)),
            cancellationToken,
            json => new RawJsonResponse(json));

    public Task<Call<GetWithdrawalsRequest, RawJsonResponse>> GetWithdrawalsCallAsync(
        GetWithdrawalsRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bitflyer.GetWithdrawals",
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
        _executor.SendAndParse(
            request,
            "Bitflyer.GetBankAccounts",
            BitflyerPrivateEndpoints.GetBankAccounts(),
            cancellationToken,
            json => new RawJsonResponse(json));

    public Task<Call<CreateWithdrawalRequest, CreateWithdrawalResponse>> WithdrawCallAsync(
        CreateWithdrawalRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bitflyer.Withdraw",
            BitflyerPrivateEndpoints.Withdraw(
                BitflyerRawJson.SerializeOrThrow(request, "Bitflyer.Withdraw")),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<CreateWithdrawalResponse>(
                json,
                "Bitflyer.Withdraw"));

    public Task<Call<string, RawSendChildOrderResponse>> SendChildOrderCallAsync(
        string bodyJson,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            bodyJson,
            "Bitflyer.SendChildOrder",
            BitflyerPrivateEndpoints.SendChildOrder(bodyJson),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<RawSendChildOrderResponse>(
                json,
                "Bitflyer.SendChildOrder"));

    public Task<Call<string, RawSendParentOrderResponse>> SendParentOrderCallAsync(
        string bodyJson,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            bodyJson,
            "Bitflyer.SendParentOrder",
            BitflyerPrivateEndpoints.SendParentOrder(bodyJson),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<RawSendParentOrderResponse>(
                json,
                "Bitflyer.SendParentOrder"));

    public Task<Call<CancelChildOrderRequest, RawCancelChildOrderResponse>> CancelChildOrderCallAsync(
        CancelChildOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bitflyer.CancelChildOrder",
            BitflyerPrivateEndpoints.CancelChildOrder(
                BitflyerRawJson.SerializeOrThrow(request, "Bitflyer.CancelChildOrder")),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<RawCancelChildOrderResponse>(
                json,
                "Bitflyer.CancelChildOrder"));

    public Task<Call<CancelParentOrderRequest, RawCancelParentOrderResponse>> CancelParentOrderCallAsync(
        CancelParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bitflyer.CancelParentOrder",
            BitflyerPrivateEndpoints.CancelParentOrder(
                BitflyerRawJson.SerializeOrThrow(request, "Bitflyer.CancelParentOrder")),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<RawCancelParentOrderResponse>(
                json,
                "Bitflyer.CancelParentOrder"));

    public Task<Call<CancelAllChildOrdersRequest, RawCancelAllChildOrdersResponse>> CancelAllChildOrdersCallAsync(
        CancelAllChildOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bitflyer.CancelAllChildOrders",
            BitflyerPrivateEndpoints.CancelAllChildOrders(
                BitflyerRawJson.SerializeOrThrow(request, "Bitflyer.CancelAllChildOrders")),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<RawCancelAllChildOrdersResponse>(
                json,
                "Bitflyer.CancelAllChildOrders"));

    public Task<Call<GetChildOrdersRequest, IReadOnlyList<RawGetChildOrdersResponse>>> GetChildOrdersCallAsync(
        GetChildOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bitflyer.GetChildOrders",
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
                "Bitflyer.GetChildOrders"));

    public Task<Call<GetParentOrdersRequest, IReadOnlyList<RawGetParentOrdersResponse>>> GetParentOrdersCallAsync(
        GetParentOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bitflyer.GetParentOrders",
            BitflyerPrivateEndpoints.GetParentOrders(
                request.ProductCode,
                request.ParentOrderState,
                request.Count?.ToString(CultureInfo.InvariantCulture),
                request.Before?.ToString(CultureInfo.InvariantCulture),
                request.After?.ToString(CultureInfo.InvariantCulture)),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<RawGetParentOrdersResponse>>(
                json,
                "Bitflyer.GetParentOrders"));

    public Task<Call<GetParentOrderRequest, RawGetParentOrderResponse>> GetParentOrderCallAsync(
        GetParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _executor.SendAndParse(
            request,
            "Bitflyer.GetParentOrder",
            BitflyerPrivateEndpoints.GetParentOrder(
                request.ParentOrderId,
                request.ParentOrderAcceptanceId),
            cancellationToken,
            json => BitflyerRawJson.DeserializeOrThrow<RawGetParentOrderResponse>(
                json,
                "Bitflyer.GetParentOrder"));
}
