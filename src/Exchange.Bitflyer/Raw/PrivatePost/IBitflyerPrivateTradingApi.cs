using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Exchange.Bitflyer.Raw;

namespace Exchange.Bitflyer.Raw;

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

    Task<BitflyerSendParentOrderResponse> SendParentOrderAsync(
        BitflyerSendParentOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<BitflyerEmptyResponse> CancelParentOrderAsync(
        BitflyerCancelParentOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<BitflyerWithdrawResponse> WithdrawAsync(
        BitflyerWithdrawRequest request,
        CancellationToken cancellationToken = default);
}
