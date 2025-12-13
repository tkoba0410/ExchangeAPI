using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Adapter.Bitflyer.Models;

namespace ExchangeApi.Adapter.Bitflyer;

/// <summary>
/// bitFlyer Private Trading API のインターフェース。
/// </summary>
public interface IBitflyerPrivateTradingApi
{
    Task<BitflyerSendChildOrderResponse> SendChildOrderAsync(
        BitflyerSendChildOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<BitflyerEmptyResponse> CancelChildOrderAsync(
        BitflyerCancelChildOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<BitflyerEmptyResponse> CancelAllChildOrdersAsync(
        BitflyerCancelAllChildOrdersRequest request,
        CancellationToken cancellationToken = default);

    Task<JsonElement> SendParentOrderAsync(
        Dictionary<string, object?> body,
        CancellationToken cancellationToken = default);

    Task<BitflyerEmptyResponse> CancelParentOrderAsync(
        Dictionary<string, object?> body,
        CancellationToken cancellationToken = default);

    Task<JsonElement> WithdrawAsync(
        Dictionary<string, object?> body,
        CancellationToken cancellationToken = default);
}
