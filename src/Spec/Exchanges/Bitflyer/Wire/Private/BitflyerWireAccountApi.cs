using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Enums;
using ExchangeApi.Exchanges.Bitflyer.Raw.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire;
using ExchangeApi.Exchanges.Bitflyer.Raw.PrivateGet;
using ExchangeApi.Exchanges.Bitflyer.Raw.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire.Private;

internal sealed class BitflyerWireAccountApi : IBitflyerWireAccountApi
{
    private const ExchangeCode Exchange = ExchangeCode.Bitflyer;
    private readonly IBitflyerRawAccountApi _raw;

    public BitflyerWireAccountApi(IBitflyerRawAccountApi raw)
    {
        _raw = raw ?? throw new ArgumentNullException(nameof(raw));
    }

    public async Task<WireResponse<IReadOnlyList<BalanceResponse>>> GetBalancesAsync(
        CancellationToken cancellationToken = default)
    {
        var response = await _raw.GetBalancesAsync(cancellationToken).ConfigureAwait(false);
        return new WireResponse<IReadOnlyList<BalanceResponse>>(Exchange, response);
    }

    public async Task<WireResponse<IReadOnlyList<ExecutionPrivateResponse>>> GetExecutionsAsync(
        RawProductCode productCode,
        string? childOrderId = null,
        string? childOrderAcceptanceId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default)
    {
        var response = await _raw.GetExecutionsAsync(
                productCode,
                childOrderId,
                childOrderAcceptanceId,
                count,
                before,
                after,
                cancellationToken)
            .ConfigureAwait(false);
        return new WireResponse<IReadOnlyList<ExecutionPrivateResponse>>(Exchange, response);
    }

    public async Task<WireResponse<IReadOnlyList<PositionResponse>>> GetPositionsAsync(
        RawProductCode productCode,
        CancellationToken cancellationToken = default)
    {
        var response = await _raw.GetPositionsAsync(productCode, cancellationToken).ConfigureAwait(false);
        return new WireResponse<IReadOnlyList<PositionResponse>>(Exchange, response);
    }

    public async Task<WireResponse<CollateralResponse>> GetCollateralAsync(
        CancellationToken cancellationToken = default)
    {
        var response = await _raw.GetCollateralAsync(cancellationToken).ConfigureAwait(false);
        return new WireResponse<CollateralResponse>(Exchange, response);
    }

    public async Task<WireResponse<IReadOnlyList<ChildOrderResponse>>> GetChildOrdersAsync(
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
        var response = await _raw.GetChildOrdersAsync(
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
        return new WireResponse<IReadOnlyList<ChildOrderResponse>>(Exchange, response);
    }

    public async Task<WireResponse<JsonElement>> GetTradingCommissionAsync(
        RawProductCode productCode,
        CancellationToken cancellationToken = default)
    {
        var response = await _raw.GetTradingCommissionAsync(productCode, cancellationToken).ConfigureAwait(false);
        return new WireResponse<JsonElement>(Exchange, response);
    }
}
