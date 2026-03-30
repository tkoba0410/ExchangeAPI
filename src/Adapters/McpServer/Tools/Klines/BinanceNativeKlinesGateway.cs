using ExchangeApi.Exchanges.Binance.Native.Public.Api;
using BinanceGetKlinesRequest = ExchangeApi.Exchanges.Binance.Native.Public.Endpoints.GetKlines.GetKlinesRequest;
using BinanceGetKlines = ExchangeApi.Exchanges.Binance.Native.Public.Endpoints.GetKlines.GetKlines;
using ExchangeApi.Primitives.Calls;

namespace ExchangeApi.Adapters.McpServer.Tools.Klines;

public sealed class BinanceNativeKlinesGateway : IBinanceKlinesGateway
{
    private readonly IBinancePublicNativeApi _publicApi;

    public BinanceNativeKlinesGateway(IBinancePublicNativeApi publicApi)
    {
        _publicApi = publicApi;
    }

    public Task<Call<BinanceGetKlinesRequest, IReadOnlyList<BinanceGetKlines.Item>>> GetKlinesCallAsync(
        BinanceGetKlinesRequest request,
        CancellationToken cancellationToken = default)
    {
        return _publicApi.GetKlinesCallAsync(request, cancellationToken);
    }
}
