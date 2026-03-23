using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Stage10.Bitflyer.Protocol.Public.Api;

public interface IBitflyerPublicProtocolApi
{
    Task<Call<WireCallSpec, WireResponse>> GetTickerCallAsync(
        string? productCode = null,
        CancellationToken cancellationToken = default);
}
