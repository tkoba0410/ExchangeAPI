using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Enums;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Exchanges.Bitflyer.Wire;

namespace ExchangeApi.Exchanges.Bitflyer.Wire.Private;

internal sealed class BitflyerWireAccountApiNotSupported : IBitflyerWireAccountApi
{
    private static ExchangeFeatureNotSupportedException NotSupported() =>
        new(ExchangeCode.Bitflyer, "WireAccount");

    public Task<IReadOnlyList<BalanceResponse>> GetBalancesAsync(
        CancellationToken cancellationToken = default) =>
        throw NotSupported();

    public Task<IReadOnlyList<ExecutionPrivateResponse>> GetExecutionsAsync(
        string productCode,
        string? childOrderId = null,
        string? childOrderAcceptanceId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default) =>
        throw NotSupported();

    public Task<IReadOnlyList<PositionResponse>> GetPositionsAsync(
        string productCode,
        CancellationToken cancellationToken = default) =>
        throw NotSupported();

    public Task<CollateralResponse> GetCollateralAsync(
        CancellationToken cancellationToken = default) =>
        throw NotSupported();

    public Task<IReadOnlyList<ChildOrderResponse>> GetChildOrdersAsync(
        string productCode,
        string? childOrderStatusState = null,
        string? childOrderAcceptanceId = null,
        string? childOrderId = null,
        string? parentOrderId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default) =>
        throw NotSupported();

    public Task<JsonElement> GetTradingCommissionAsync(
        string productCode,
        CancellationToken cancellationToken = default) =>
        throw NotSupported();
}
