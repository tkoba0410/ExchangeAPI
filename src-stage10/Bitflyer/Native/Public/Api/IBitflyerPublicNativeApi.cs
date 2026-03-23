using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Stage10.Bitflyer.Native.Public.Dtos;
using ExchangeApi.Stage10.Bitflyer.Native.Public.Requests;

namespace ExchangeApi.Stage10.Bitflyer.Native.Public.Api;

public interface IBitflyerPublicNativeApi
{
    Task<Call<GetTickerRequest, GetTickerResponse>> GetTickerCallAsync(
        GetTickerRequest request,
        CancellationToken cancellationToken = default);
}
