using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Bitflyer.Models;

namespace ExchangeApi.Bitflyer;

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
}
