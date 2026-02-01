namespace ExchangeApi.Composition.Dtos;

using System;

/// <summary>
/// API キーとシークレットを保持する DTO。
/// </summary>
/// <remarks>
/// KeyType には "read", "trade" など権限種別を入れられる。ExpiresAt は失効時刻で、設定されない場合は無期限を想定する。
/// </remarks>
public sealed record ApiCredentials(
    string ApiKey,
    string ApiSecret,
    string? KeyType = null,
    DateTimeOffset? ExpiresAt = null);
