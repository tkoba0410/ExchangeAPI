using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Api;

public interface IBitflyerPublicProtocolApi
{
    Task<Call<ProtocolRequest, ProtocolResponse>> GetTickerCallAsync(
        string? productCode,
        CancellationToken cancellationToken = default);
}
