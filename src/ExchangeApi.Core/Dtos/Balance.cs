namespace ExchangeApi.Core.Dtos;

/// <summary>
/// 口座残高情報。
/// Stage2 では通貨ごとの総残高と発注可能残高のみを扱う。
/// </summary>
public sealed record Balance(
    string Currency,
    decimal Amount,
    decimal Available);
