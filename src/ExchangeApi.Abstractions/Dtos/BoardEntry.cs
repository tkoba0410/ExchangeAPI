namespace ExchangeApi.Abstractions.Dtos;

/// <summary>
/// 板の1行（価格とサイズ）。
/// </summary>
public sealed record BoardEntry(decimal Price, decimal Size);
