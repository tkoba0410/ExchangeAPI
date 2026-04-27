namespace ExchangeApi.Exchanges.Bitflyer.Protocol.Realtime;

public class BitflyerRealtimeException : Exception
{
    public BitflyerRealtimeException(string message)
        : base(message)
    {
    }

    public BitflyerRealtimeException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

public sealed class BitflyerRealtimeConnectionException : BitflyerRealtimeException
{
    public BitflyerRealtimeConnectionException(string message)
        : base(message)
    {
    }

    public BitflyerRealtimeConnectionException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

public sealed class BitflyerRealtimeMessageException : BitflyerRealtimeException
{
    public BitflyerRealtimeMessageException(string message)
        : base(message)
    {
    }

    public BitflyerRealtimeMessageException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
