using ExchangeApi.Primitives.Credentials;

namespace ExchangeApi.Exchanges.Bitflyer.Protocol.Realtime;

public interface IBitflyerPrivateRealtimeProtocolClient : IBitflyerRealtimeProtocolClient
{
    ValueTask AuthenticateAsync(
        IApiCredentialSession credentialSession,
        CancellationToken cancellationToken = default);
}
