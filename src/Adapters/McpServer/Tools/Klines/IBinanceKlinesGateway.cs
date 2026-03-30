using BinanceGetKlinesRequest = ExchangeApi.Exchanges.Binance.Native.Public.Endpoints.GetKlines.GetKlinesRequest;
using BinanceGetKlines = ExchangeApi.Exchanges.Binance.Native.Public.Endpoints.GetKlines.GetKlines;
using ExchangeApi.Primitives.Calls;

namespace ExchangeApi.Adapters.McpServer.Tools.Klines;

public interface IBinanceKlinesGateway
{
    Task<Call<BinanceGetKlinesRequest, IReadOnlyList<BinanceGetKlines.Item>>> GetKlinesCallAsync(
        BinanceGetKlinesRequest request,
        CancellationToken cancellationToken = default);
}
