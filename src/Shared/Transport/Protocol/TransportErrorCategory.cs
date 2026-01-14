namespace ExchangeApi.Shared.Transport.Protocol;

/// <summary>
/// Transport レイヤーで扱うエラー分類。
/// </summary>
public enum TransportErrorCategory
{
    Unknown,
    Request,
    Auth,
    Balance,
    RateLimit,
    Network,
    Server,
}
