namespace ExchangeApi.Abstractions.Dtos;

/// <summary>
/// 注文種別を表す列挙。
/// Stage3 では Market のみ使用する。
/// </summary>
public enum OrderType
{
    Market,
    Limit,
    Stop,
}
