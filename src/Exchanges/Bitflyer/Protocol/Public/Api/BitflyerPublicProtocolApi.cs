using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetTicker;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Api;

public sealed class BitflyerPublicProtocolApi : IBitflyerPublicProtocolApi
{
    private readonly IGetTickerProtocolEndpoint _getTicker;

    public BitflyerPublicProtocolApi(IGetTickerProtocolEndpoint getTicker)
    {
        _getTicker = getTicker;
    }

    public Task<Call<ProtocolRequest, ProtocolResponse>> GetTickerCallAsync(
        string? productCode,
        CancellationToken cancellationToken = default)
    {
        return _getTicker.SendAsync(productCode, cancellationToken);
    }
}
