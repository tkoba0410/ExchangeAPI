using ExchangeApi.Exchanges.Binance.Native.Public.Api;
using ExchangeApi.Primitives.Calls;
using BinanceGetKlines = ExchangeApi.Exchanges.Binance.Native.Public.Endpoints.GetKlines.GetKlines;
using BinanceGetKlinesRequest = ExchangeApi.Exchanges.Binance.Native.Public.Endpoints.GetKlines.GetKlinesRequest;

namespace ExchangeApi.Adapters.McpServer.Tools.Klines;

public sealed class BinanceNativeKlinesGateway : IBinanceKlinesGateway
{
    private readonly IBinancePublicNativeApi _publicApi;

    public BinanceNativeKlinesGateway(IBinancePublicNativeApi publicApi)
    {
        _publicApi = publicApi;
    }

    public Task<CallResult<BinanceGetKlinesRequest, IReadOnlyList<BinanceGetKlines.Item>>> GetKlinesAsync(
        BinanceGetKlinesRequest request,
        CancellationToken cancellationToken = default)
    {
        return _publicApi.GetKlinesAsync(request, cancellationToken);
    }
}
