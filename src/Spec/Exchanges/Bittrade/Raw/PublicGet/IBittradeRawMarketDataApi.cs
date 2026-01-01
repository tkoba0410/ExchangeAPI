using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ExchangeApi.Exchanges.Bittrade.Raw;

public interface IBittradeRawMarketDataApi
{
    Task<RawMergedResponse> GetTickerAsync(RawSymbol symbol, CancellationToken cancellationToken = default);
    Task<RawDepthResponse> GetOrderBookAsync(RawSymbol symbol, string? type = null, CancellationToken cancellationToken = default);
    Task<RawTradeResponse> GetTradesAsync(RawSymbol symbol, CancellationToken cancellationToken = default);

    Task<BittradeRawCall<RawMergedResponse, JsonElement>> GetTickerCallAsync(
        RawSymbol symbol,
        CancellationToken cancellationToken = default);

    Task<BittradeRawCall<RawDepthResponse, JsonElement>> GetOrderBookCallAsync(
        RawSymbol symbol,
        string? type = null,
        CancellationToken cancellationToken = default);

    Task<BittradeRawCall<RawTradeResponse, JsonElement>> GetTradesCallAsync(
        RawSymbol symbol,
        CancellationToken cancellationToken = default);
}
