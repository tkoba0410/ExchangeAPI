namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Bootstrap;

/// <summary>
/// bitFlyer 署名に必要な最小資格情報。
/// </summary>
public sealed record ClientCredentials(
    string ApiKey,
    string ApiSecret);
