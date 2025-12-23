using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Wire.Private.Models;
using ExchangeApi.Exchanges.Bittrade.Wire.Private.Requests;

namespace ExchangeApi.Exchanges.Bittrade.Wire.Private;

public interface IBittradeWireTradingApi
{
    Task<BittradeWireOrder> PlaceOrderAsync(BittradeWireCreateOrderRequest request, CancellationToken ct = default);

    Task CancelOrderAsync(string orderId, CancellationToken ct = default);

    Task<IReadOnlyList<BittradeWireOpenOrder>> GetOpenOrdersAsync(string symbol, CancellationToken ct = default);

    Task<BittradeWireOrder> GetOrderAsync(string orderId, CancellationToken ct = default);
}
