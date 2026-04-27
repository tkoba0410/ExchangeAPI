using ExchangeApi.Exchanges.Bitflyer.Protocol.Realtime;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Realtime.Internal;

internal sealed class BitflyerRealtimeResilienceException : BitflyerRealtimeException
{
    internal BitflyerRealtimeResilienceException(BitflyerRealtimeErrorKind kind, string message)
        : base(kind, message)
    {
    }

    internal BitflyerRealtimeResilienceException(BitflyerRealtimeErrorKind kind, string message, Exception innerException)
        : base(kind, message, innerException)
    {
    }
}
