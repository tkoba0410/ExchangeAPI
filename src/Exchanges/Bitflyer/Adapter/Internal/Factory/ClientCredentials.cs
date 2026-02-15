namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Internal.Factory;

/// <summary>
/// bitFlyer 署名に必要な最小資格情報。
/// </summary>
public sealed record ClientCredentials(
    string ApiKey,
    string ApiSecret);
