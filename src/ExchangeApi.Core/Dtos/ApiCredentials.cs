namespace ExchangeApi.Core.Dtos;

/// <summary>
/// API キーとシークレットを保持する DTO。
/// </summary>
public sealed record ApiCredentials(
    string ApiKey,
    string ApiSecret);
