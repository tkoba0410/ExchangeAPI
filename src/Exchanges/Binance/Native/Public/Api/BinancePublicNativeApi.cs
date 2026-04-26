using ExchangeApi.Exchanges.Binance.Native.Public.Endpoints.GetKlines;
using ExchangeApi.Primitives.Calls;

namespace ExchangeApi.Exchanges.Binance.Native.Public.Api;

public sealed class BinancePublicNativeApi : IBinancePublicNativeApi
{
    private readonly IGetKlinesNativeEndpoint _getKlines;

    public BinancePublicNativeApi(IGetKlinesNativeEndpoint getKlines)
    {
        _getKlines = getKlines;
    }

    public Task<CallResult<GetKlinesRequest, IReadOnlyList<GetKlines.Item>>> GetKlinesAsync(
        GetKlinesRequest request,
        CancellationToken cancellationToken = default)
    {
        return _getKlines.CallAsync(request, cancellationToken);
    }
}
