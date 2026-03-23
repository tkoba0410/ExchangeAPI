using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Stage10.Bitflyer.Native.Public.Dtos;
using ExchangeApi.Stage10.Bitflyer.Native.Public.Endpoints.GetTicker;
using ExchangeApi.Stage10.Bitflyer.Native.Public.Requests;

namespace ExchangeApi.Stage10.Bitflyer.Native.Public.Api;

public sealed class BitflyerPublicNativeApi : IBitflyerPublicNativeApi
{
    private readonly IGetTickerNativeEndpoint _getTicker;

    public BitflyerPublicNativeApi(IGetTickerNativeEndpoint getTicker)
    {
        _getTicker = getTicker ?? throw new ArgumentNullException(nameof(getTicker));
    }

    public Task<Call<GetTickerRequest, GetTickerResponse>> GetTickerCallAsync(
        GetTickerRequest request,
        CancellationToken cancellationToken = default) =>
        _getTicker.CallAsync(request, cancellationToken);
}
