using ExchangeApi.Primitives.Calls;
using BinanceGetKlines = ExchangeApi.Exchanges.Binance.Native.Public.Endpoints.GetKlines.GetKlines;
using BinanceGetKlinesRequest = ExchangeApi.Exchanges.Binance.Native.Public.Endpoints.GetKlines.GetKlinesRequest;

namespace ExchangeApi.Adapters.McpServer.Tools.Klines;

public interface IBinanceKlinesGateway
{
    Task<CallResult<BinanceGetKlinesRequest, IReadOnlyList<BinanceGetKlines.Item>>> GetKlinesAsync(
        BinanceGetKlinesRequest request,
        CancellationToken cancellationToken = default);
}
