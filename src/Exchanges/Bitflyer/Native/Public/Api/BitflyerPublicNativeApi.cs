using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetTicker;
using ExchangeApi.Primitives.Calls;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Public.Api;

public sealed class BitflyerPublicNativeApi : IBitflyerPublicNativeApi
{
    private readonly IGetTickerNativeEndpoint _getTicker;

    public BitflyerPublicNativeApi(IGetTickerNativeEndpoint getTicker)
    {
        _getTicker = getTicker;
    }

    public Task<Call<GetTickerRequest, GetTickerResponse>> GetTickerCallAsync(
        GetTickerRequest request,
        CancellationToken cancellationToken = default)
    {
        return _getTicker.CallAsync(request, cancellationToken);
    }
}
