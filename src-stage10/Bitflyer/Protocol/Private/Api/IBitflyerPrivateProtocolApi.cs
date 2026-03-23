using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Stage10.Bitflyer.Protocol.Private.Api;

public interface IBitflyerPrivateProtocolApi
{
    Task<Call<WireCallSpec, WireResponse>> GetBalanceCallAsync(
        CancellationToken cancellationToken = default);

    Task<Call<WireCallSpec, WireResponse>> SendChildOrderCallAsync(
        string bodyJson,
        CancellationToken cancellationToken = default);

    Task<Call<WireCallSpec, WireResponse>> CancelChildOrderCallAsync(
        string bodyJson,
        CancellationToken cancellationToken = default);
}
