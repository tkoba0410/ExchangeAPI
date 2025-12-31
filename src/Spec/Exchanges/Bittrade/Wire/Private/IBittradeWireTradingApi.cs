using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire;
using ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire.Private.Models;
using ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire.Private.Requests;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire.Private;

internal interface IBittradeWireTradingApi
{
    Task<WireResponse<BittradeWireOrder>> PlaceOrderAsync(BittradeWireCreateOrderRequest request, CancellationToken ct = default);

    Task<WireResponse<bool>> CancelOrderAsync(string orderId, CancellationToken ct = default);

    Task<WireResponse<IReadOnlyList<BittradeWireOpenOrder>>> GetOpenOrdersAsync(string symbol, CancellationToken ct = default);

    Task<WireResponse<BittradeWireOrder>> GetOrderAsync(string orderId, CancellationToken ct = default);
}
