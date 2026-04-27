namespace ExchangeApi.Exchanges.Bitflyer.Protocol.Realtime;

public sealed class BitflyerRealtimeAuthenticationException : BitflyerRealtimeException
{
    public BitflyerRealtimeAuthenticationException(string message)
        : base(BitflyerRealtimeErrorKind.AuthenticationFailed, message)
    {
    }
}
