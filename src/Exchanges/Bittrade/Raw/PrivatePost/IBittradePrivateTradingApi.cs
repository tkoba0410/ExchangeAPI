using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Exchange.Bittrade.Raw;
namespace Exchange.Bittrade.Raw;

/// <summary>
/// Bittrade Private REST API（取引系 POST）の Raw アクセス。
/// </summary>
public interface IBittradePrivateTradingApi
{
    Task<BittradePlaceOrderResponse> PlaceOrderAsync(Dictionary<string, object?> body, CancellationToken cancellationToken = default);

    Task<BittradeCancelOrderResponse> CancelOrderAsync(string orderId, CancellationToken cancellationToken = default);
}
