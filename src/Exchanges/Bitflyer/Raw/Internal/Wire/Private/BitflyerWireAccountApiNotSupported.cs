using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Raw.PrivateGet;
using ExchangeApi.Exchanges.Bitflyer.Raw.Types;

#pragma warning disable CS0618
namespace ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire.Private;

internal sealed class BitflyerWireAccountApiNotSupported : IBitflyerWireAccountApi
{
    private static NotSupportedException NotSupported() =>
        new("Bitflyer wire account is not supported.");

    public Task<IReadOnlyList<BalanceResponse>> GetBalancesAsync(
        CancellationToken cancellationToken = default) =>
        throw NotSupported();

    public Task<IReadOnlyList<ExecutionPrivateResponse>> GetExecutionsAsync(
        RawProductCode productCode,
        string? childOrderId = null,
        string? childOrderAcceptanceId = null,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default) =>
        throw NotSupported();

    public Task<IReadOnlyList<PositionResponse>> GetPositionsAsync(
        RawProductCode productCode,
        CancellationToken cancellationToken = default) =>
        throw NotSupported();

    public Task<CollateralResponse> GetCollateralAsync(
        CancellationToken cancellationToken = default) =>
        throw NotSupported();

    public Task<IReadOnlyList<ChildOrderResponse>> GetChildOrdersAsync(
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

    public Task<JsonElement> GetTradingCommissionAsync(
        RawProductCode productCode,
        CancellationToken cancellationToken = default) =>
        throw NotSupported();
}
#pragma warning restore CS0618
