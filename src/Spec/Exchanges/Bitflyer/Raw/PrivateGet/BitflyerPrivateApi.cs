using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using WireAccountApi = ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire.Private.IBitflyerWireAccountApi;
using ExchangeApi.Exchanges.Bitflyer.Raw.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.PrivateGet;

/// <summary>
/// bitFlyer Private REST API（情報系）の実装。
/// </summary>
internal sealed class BitflyerPrivateApi : IBitflyerPrivateApi
{
    private readonly WireAccountApi _wire;

    public BitflyerPrivateApi(WireAccountApi wire)
    {
        _wire = wire ?? throw new ArgumentNullException(nameof(wire));
    }

    public async Task<IReadOnlyList<string>> GetPermissionsAsync(
        CancellationToken cancellationToken = default)
    {
        var response = await _wire.GetPermissionsAsync(cancellationToken).ConfigureAwait(false);
        if (response.StatusCode is >= 200 and < 300)
        {
            return BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<string>>(
                response.Json,
                "Bitflyer.GetPermissions");
        }

        throw BitflyerRawJson.CreateStatusException("Bitflyer.GetPermissions", response.StatusCode, response.Json);
    }

    public async Task<IReadOnlyList<BalanceResponse>> GetBalancesAsync(
        CancellationToken cancellationToken = default)
    {
        var response = await _wire.GetBalancesAsync(cancellationToken).ConfigureAwait(false);
        if (response.StatusCode is >= 200 and < 300)
        {
            return BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<BalanceResponse>>(
                response.Json,
                "Bitflyer.GetBalances");
        }

        throw BitflyerRawJson.CreateStatusException("Bitflyer.GetBalances", response.StatusCode, response.Json);
    }

    public async Task<IReadOnlyList<PositionResponse>> GetPositionsAsync(
        RawProductCode productCode,
        CancellationToken cancellationToken = default)
    {
        var response = await _wire.GetPositionsAsync(productCode, cancellationToken).ConfigureAwait(false);
        if (response.StatusCode is >= 200 and < 300)
        {
            return BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<PositionResponse>>(
                response.Json,
                "Bitflyer.GetPositions");
        }

        throw BitflyerRawJson.CreateStatusException("Bitflyer.GetPositions", response.StatusCode, response.Json);
    }

    public async Task<IReadOnlyList<ExecutionPrivateResponse>> GetExecutionsAsync(
        RawProductCode productCode,
        string? childOrderId = null,
        string? childOrderAcceptanceId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default)
    {
        var response = await _wire
            .GetExecutionsAsync(
                productCode,
                childOrderId,
                childOrderAcceptanceId,
                count,
                before,
                after,
                cancellationToken)
            .ConfigureAwait(false);
        if (response.StatusCode is >= 200 and < 300)
        {
            return BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<ExecutionPrivateResponse>>(
                response.Json,
                "Bitflyer.GetExecutions");
        }

        throw BitflyerRawJson.CreateStatusException("Bitflyer.GetExecutions", response.StatusCode, response.Json);
    }

    public async Task<CollateralResponse> GetCollateralAsync(
        CancellationToken cancellationToken = default)
    {
        var response = await _wire.GetCollateralAsync(cancellationToken).ConfigureAwait(false);
        if (response.StatusCode is >= 200 and < 300)
        {
            return BitflyerRawJson.DeserializeOrThrow<CollateralResponse>(
                response.Json,
                "Bitflyer.GetCollateral");
        }

        throw BitflyerRawJson.CreateStatusException("Bitflyer.GetCollateral", response.StatusCode, response.Json);
    }

    public async Task<IReadOnlyList<CollateralAccount>> GetCollateralAccountsAsync(
        CancellationToken cancellationToken = default)
    {
        var response = await _wire.GetCollateralAccountsAsync(cancellationToken).ConfigureAwait(false);
        if (response.StatusCode is >= 200 and < 300)
        {
            return BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<CollateralAccount>>(
                response.Json,
                "Bitflyer.GetCollateralAccounts");
        }

        throw BitflyerRawJson.CreateStatusException(
            "Bitflyer.GetCollateralAccounts",
            response.StatusCode,
            response.Json);
    }

    public async Task<IReadOnlyList<ChildOrderResponse>> GetChildOrdersAsync(
        RawProductCode productCode,
        string? childOrderStatusState = null,
        string? childOrderAcceptanceId = null,
        string? childOrderId = null,
        string? parentOrderId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default)
    {
        var response = await _wire
            .GetChildOrdersAsync(
                productCode,
                childOrderStatusState,
                childOrderAcceptanceId,
                childOrderId,
                parentOrderId,
                count,
                before,
                after,
                cancellationToken)
            .ConfigureAwait(false);
        if (response.StatusCode is >= 200 and < 300)
        {
            return BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<ChildOrderResponse>>(
                response.Json,
                "Bitflyer.GetChildOrders");
        }

        throw BitflyerRawJson.CreateStatusException("Bitflyer.GetChildOrders", response.StatusCode, response.Json);
    }

    public async Task<IReadOnlyList<ParentOrderResponse>> GetParentOrdersAsync(
        RawProductCode productCode,
        int? count = null,
        long? before = null,
        long? after = null,
        string? parentOrderStatusState = null,
        CancellationToken cancellationToken = default)
    {
        var response = await _wire
            .GetParentOrdersAsync(
                productCode,
                count,
                before,
                after,
                parentOrderStatusState,
                cancellationToken)
            .ConfigureAwait(false);
        if (response.StatusCode is >= 200 and < 300)
        {
            return BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<ParentOrderResponse>>(
                response.Json,
                "Bitflyer.GetParentOrders");
        }

        throw BitflyerRawJson.CreateStatusException("Bitflyer.GetParentOrders", response.StatusCode, response.Json);
    }

    public async Task<ParentOrderDetailResponse> GetParentOrderAsync(
        string? parentOrderId = null,
        string? parentOrderAcceptanceId = null,
        CancellationToken cancellationToken = default)
    {
        var response = await _wire
            .GetParentOrderAsync(parentOrderId, parentOrderAcceptanceId, cancellationToken)
            .ConfigureAwait(false);
        if (response.StatusCode is >= 200 and < 300)
        {
            return BitflyerRawJson.DeserializeOrThrow<ParentOrderDetailResponse>(
                response.Json,
                "Bitflyer.GetParentOrder");
        }

        throw BitflyerRawJson.CreateStatusException("Bitflyer.GetParentOrder", response.StatusCode, response.Json);
    }

    public async Task<IReadOnlyList<JsonElement>> GetBalanceHistoryAsync(
        string? currencyCode = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default)
    {
        var response = await _wire
            .GetBalanceHistoryAsync(currencyCode, count, before, after, cancellationToken)
            .ConfigureAwait(false);
        if (response.StatusCode is >= 200 and < 300)
        {
            return BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<JsonElement>>(
                response.Json,
                "Bitflyer.GetBalanceHistory");
        }

        throw BitflyerRawJson.CreateStatusException(
            "Bitflyer.GetBalanceHistory",
            response.StatusCode,
            response.Json);
    }

    public async Task<JsonElement> GetTradingCommissionAsync(
        RawProductCode productCode,
        CancellationToken cancellationToken = default)
    {
        var response = await _wire.GetTradingCommissionAsync(productCode, cancellationToken).ConfigureAwait(false);
        if (response.StatusCode is >= 200 and < 300)
        {
            return BitflyerRawJson.DeserializeOrThrow<JsonElement>(
                response.Json,
                "Bitflyer.GetTradingCommission");
        }

        throw BitflyerRawJson.CreateStatusException(
            "Bitflyer.GetTradingCommission",
            response.StatusCode,
            response.Json);
    }

    public async Task<IReadOnlyList<JsonElement>> GetCollateralHistoryAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default)
    {
        var response = await _wire
            .GetCollateralHistoryAsync(count, before, after, cancellationToken)
            .ConfigureAwait(false);
        if (response.StatusCode is >= 200 and < 300)
        {
            return BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<JsonElement>>(
                response.Json,
                "Bitflyer.GetCollateralHistory");
        }

        throw BitflyerRawJson.CreateStatusException(
            "Bitflyer.GetCollateralHistory",
            response.StatusCode,
            response.Json);
    }

    public async Task<IReadOnlyList<JsonElement>> GetAddressesAsync(
        CancellationToken cancellationToken = default)
    {
        var response = await _wire.GetAddressesAsync(cancellationToken).ConfigureAwait(false);
        if (response.StatusCode is >= 200 and < 300)
        {
            return BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<JsonElement>>(
                response.Json,
                "Bitflyer.GetAddresses");
        }

        throw BitflyerRawJson.CreateStatusException("Bitflyer.GetAddresses", response.StatusCode, response.Json);
    }

    public async Task<IReadOnlyList<JsonElement>> GetCoinInsAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default)
    {
        var response = await _wire.GetCoinInsAsync(count, before, after, cancellationToken).ConfigureAwait(false);
        if (response.StatusCode is >= 200 and < 300)
        {
            return BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<JsonElement>>(
                response.Json,
                "Bitflyer.GetCoinIns");
        }

        throw BitflyerRawJson.CreateStatusException("Bitflyer.GetCoinIns", response.StatusCode, response.Json);
    }

    public async Task<IReadOnlyList<JsonElement>> GetCoinOutsAsync(
        string? messageId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default)
    {
        var response = await _wire
            .GetCoinOutsAsync(messageId, count, before, after, cancellationToken)
            .ConfigureAwait(false);
        if (response.StatusCode is >= 200 and < 300)
        {
            return BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<JsonElement>>(
                response.Json,
                "Bitflyer.GetCoinOuts");
        }

        throw BitflyerRawJson.CreateStatusException("Bitflyer.GetCoinOuts", response.StatusCode, response.Json);
    }

    public async Task<IReadOnlyList<JsonElement>> GetDepositsAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default)
    {
        var response = await _wire
            .GetDepositsAsync(count, before, after, cancellationToken)
            .ConfigureAwait(false);
        if (response.StatusCode is >= 200 and < 300)
        {
            return BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<JsonElement>>(
                response.Json,
                "Bitflyer.GetDeposits");
        }

        throw BitflyerRawJson.CreateStatusException("Bitflyer.GetDeposits", response.StatusCode, response.Json);
    }

    public async Task<IReadOnlyList<JsonElement>> GetWithdrawalsAsync(
        string? messageId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default)
    {
        var response = await _wire
            .GetWithdrawalsAsync(messageId, count, before, after, cancellationToken)
            .ConfigureAwait(false);
        if (response.StatusCode is >= 200 and < 300)
        {
            return BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<JsonElement>>(
                response.Json,
                "Bitflyer.GetWithdrawals");
        }

        throw BitflyerRawJson.CreateStatusException("Bitflyer.GetWithdrawals", response.StatusCode, response.Json);
    }

    public async Task<IReadOnlyList<JsonElement>> GetBankAccountsAsync(
        CancellationToken cancellationToken = default)
    {
        var response = await _wire.GetBankAccountsAsync(cancellationToken).ConfigureAwait(false);
        if (response.StatusCode is >= 200 and < 300)
        {
            return BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<JsonElement>>(
                response.Json,
                "Bitflyer.GetBankAccounts");
        }

        throw BitflyerRawJson.CreateStatusException("Bitflyer.GetBankAccounts", response.StatusCode, response.Json);
    }
}
