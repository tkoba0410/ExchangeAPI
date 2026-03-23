using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Stage10.Bitflyer.Protocol.Public.Endpoints.GetTicker;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Stage10.Bitflyer.Protocol.Public.Api;

public sealed class BitflyerPublicProtocolApi : IBitflyerPublicProtocolApi
{
    private readonly IGetTickerProtocolEndpoint _getTicker;

    public BitflyerPublicProtocolApi(IGetTickerProtocolEndpoint getTicker)
    {
        _getTicker = getTicker ?? throw new ArgumentNullException(nameof(getTicker));
    }

    public Task<Call<WireCallSpec, WireResponse>> GetTickerCallAsync(
        string? productCode = null,
        CancellationToken cancellationToken = default) =>
        _getTicker.SendAsync(productCode, cancellationToken);
}
