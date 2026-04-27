namespace ExchangeApi.Exchanges.Bitflyer.Protocol.Realtime;

public enum BitflyerRealtimeErrorKind
{
    Unknown = 0,
    ConnectionFailed,
    MessageInvalid,
    MessageDecodeFailed,
    AuthenticationFailed,
    ReconnectExhausted,
    ResubscribeFailed,
    TransportFailed,
}

public class BitflyerRealtimeException : Exception
{
    public BitflyerRealtimeException(string message)
        : this(BitflyerRealtimeErrorKind.Unknown, message)
    {
    }

    public BitflyerRealtimeException(BitflyerRealtimeErrorKind kind, string message)
        : base(message)
    {
        Kind = kind;
    }

    public BitflyerRealtimeException(string message, Exception innerException)
        : this(BitflyerRealtimeErrorKind.Unknown, message, innerException)
    {
    }

    public BitflyerRealtimeException(BitflyerRealtimeErrorKind kind, string message, Exception innerException)
        : base(message, innerException)
    {
        Kind = kind;
    }

    public BitflyerRealtimeErrorKind Kind { get; }
}

public sealed class BitflyerRealtimeConnectionException : BitflyerRealtimeException
{
    public BitflyerRealtimeConnectionException(string message)
        : base(BitflyerRealtimeErrorKind.ConnectionFailed, message)
    {
    }

    public BitflyerRealtimeConnectionException(string message, Exception innerException)
        : base(BitflyerRealtimeErrorKind.ConnectionFailed, message, innerException)
    {
    }
}

public sealed class BitflyerRealtimeMessageException : BitflyerRealtimeException
{
    public BitflyerRealtimeMessageException(string message)
        : base(BitflyerRealtimeErrorKind.MessageInvalid, message)
    {
    }

    public BitflyerRealtimeMessageException(string message, Exception innerException)
        : base(BitflyerRealtimeErrorKind.MessageInvalid, message, innerException)
    {
    }
}
