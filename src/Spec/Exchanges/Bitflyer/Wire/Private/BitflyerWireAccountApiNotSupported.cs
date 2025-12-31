using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Raw.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire;
using ExchangeApi.Exchanges.Bitflyer.Raw.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire.Private;

internal sealed class BitflyerWireAccountApiNotSupported : IBitflyerWireAccountApi
{
    private static NotSupportedException NotSupported() =>
        new("Bitflyer wire account is not supported.");

    public Task<WireResponse<IReadOnlyList<BalanceResponse>>> GetBalancesAsync(
        CancellationToken cancellationToken = default) =>
        throw NotSupported();

    public Task<WireResponse<IReadOnlyList<ExecutionPrivateResponse>>> GetExecutionsAsync(
        RawProductCode productCode,
        string? childOrderId = null,
        string? childOrderAcceptanceId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default) =>
        throw NotSupported();

    public Task<WireResponse<IReadOnlyList<PositionResponse>>> GetPositionsAsync(
        RawProductCode productCode,
        CancellationToken cancellationToken = default) =>
        throw NotSupported();

    public Task<WireResponse<CollateralResponse>> GetCollateralAsync(
        CancellationToken cancellationToken = default) =>
        throw NotSupported();

    public Task<WireResponse<IReadOnlyList<ChildOrderResponse>>> GetChildOrdersAsync(
        RawProductCode productCode,
        string? childOrderStatusState = null,
        string? childOrderAcceptanceId = null,
        string? childOrderId = null,
        string? parentOrderId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default) =>
        throw NotSupported();

    public Task<WireResponse<JsonElement>> GetTradingCommissionAsync(
        RawProductCode productCode,
        CancellationToken cancellationToken = default) =>
        throw NotSupported();
}
