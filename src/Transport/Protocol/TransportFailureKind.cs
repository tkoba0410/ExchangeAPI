namespace ExchangeApi.Transport.Protocol;

/// <summary>
/// Transport 例外の失敗種別。
/// </summary>
public enum TransportFailureKind
{
    Unknown,
    Timeout,
    Canceled
}
