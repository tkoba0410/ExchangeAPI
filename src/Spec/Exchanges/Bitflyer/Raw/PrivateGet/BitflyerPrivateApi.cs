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
        return BitflyerRawJson.ParseOrThrow<IReadOnlyList<string>>(response);
    }

    public async Task<IReadOnlyList<BalanceResponse>> GetBalancesAsync(
        CancellationToken cancellationToken = default)
    {
        var response = await _wire.GetBalancesAsync(cancellationToken).ConfigureAwait(false);
        return BitflyerRawJson.ParseOrThrow<IReadOnlyList<BalanceResponse>>(response);
    }

    public async Task<IReadOnlyList<PositionResponse>> GetPositionsAsync(
        RawProductCode productCode,
        CancellationToken cancellationToken = default)
    {
        var response = await _wire.GetPositionsAsync(productCode, cancellationToken).ConfigureAwait(false);
        return BitflyerRawJson.ParseOrThrow<IReadOnlyList<PositionResponse>>(response);
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
        return BitflyerRawJson.ParseOrThrow<IReadOnlyList<ExecutionPrivateResponse>>(response);
    }

    public async Task<CollateralResponse> GetCollateralAsync(
        CancellationToken cancellationToken = default)
    {
        var response = await _wire.GetCollateralAsync(cancellationToken).ConfigureAwait(false);
        return BitflyerRawJson.ParseOrThrow<CollateralResponse>(response);
    }

    public async Task<IReadOnlyList<CollateralAccount>> GetCollateralAccountsAsync(
        CancellationToken cancellationToken = default)
    {
        var response = await _wire.GetCollateralAccountsAsync(cancellationToken).ConfigureAwait(false);
        return BitflyerRawJson.ParseOrThrow<IReadOnlyList<CollateralAccount>>(response);
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
        return BitflyerRawJson.ParseOrThrow<IReadOnlyList<ChildOrderResponse>>(response);
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
        return BitflyerRawJson.ParseOrThrow<IReadOnlyList<ParentOrderResponse>>(response);
    }

    public async Task<ParentOrderDetailResponse> GetParentOrderAsync(
        string? parentOrderId = null,
        string? parentOrderAcceptanceId = null,
        CancellationToken cancellationToken = default)
    {
        var response = await _wire
            .GetParentOrderAsync(parentOrderId, parentOrderAcceptanceId, cancellationToken)
            .ConfigureAwait(false);
        return BitflyerRawJson.ParseOrThrow<ParentOrderDetailResponse>(response);
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
        return BitflyerRawJson.ParseOrThrow<IReadOnlyList<JsonElement>>(response);
    }

    public async Task<JsonElement> GetTradingCommissionAsync(
        RawProductCode productCode,
        CancellationToken cancellationToken = default)
    {
        var response = await _wire.GetTradingCommissionAsync(productCode, cancellationToken).ConfigureAwait(false);
        return BitflyerRawJson.ParseOrThrow<JsonElement>(response);
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
        return BitflyerRawJson.ParseOrThrow<IReadOnlyList<JsonElement>>(response);
    }

    public async Task<IReadOnlyList<JsonElement>> GetAddressesAsync(
        CancellationToken cancellationToken = default)
    {
        var response = await _wire.GetAddressesAsync(cancellationToken).ConfigureAwait(false);
        return BitflyerRawJson.ParseOrThrow<IReadOnlyList<JsonElement>>(response);
    }

    public async Task<IReadOnlyList<JsonElement>> GetCoinInsAsync(
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default)
    {
        var response = await _wire.GetCoinInsAsync(count, before, after, cancellationToken).ConfigureAwait(false);
        return BitflyerRawJson.ParseOrThrow<IReadOnlyList<JsonElement>>(response);
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
        return BitflyerRawJson.ParseOrThrow<IReadOnlyList<JsonElement>>(response);
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
        return BitflyerRawJson.ParseOrThrow<IReadOnlyList<JsonElement>>(response);
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
        return BitflyerRawJson.ParseOrThrow<IReadOnlyList<JsonElement>>(response);
    }

    public async Task<IReadOnlyList<JsonElement>> GetBankAccountsAsync(
        CancellationToken cancellationToken = default)
    {
        var response = await _wire.GetBankAccountsAsync(cancellationToken).ConfigureAwait(false);
        return BitflyerRawJson.ParseOrThrow<IReadOnlyList<JsonElement>>(response);
    }
}
