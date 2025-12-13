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
    Task<BitflyerSendChildOrderResponse> PlaceChildOrderAsync(
        BitflyerSendChildOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<BitflyerEmptyResponse> CancelChildOrderAsync(
        BitflyerCancelChildOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<BitflyerEmptyResponse> CancelAllOrdersAsync(
        BitflyerCancelAllChildOrdersRequest request,
        CancellationToken cancellationToken = default);

    Task<BitflyerSendParentOrderResponse> SendParentOrderAsync(
        BitflyerSendParentOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<BitflyerEmptyResponse> CancelParentOrderAsync(
        BitflyerCancelParentOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<BitflyerWithdrawResponse> RequestWithdrawalAsync(
        BitflyerWithdrawRequest request,
        CancellationToken cancellationToken = default);
}
