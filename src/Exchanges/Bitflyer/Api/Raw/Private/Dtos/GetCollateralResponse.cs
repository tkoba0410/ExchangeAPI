using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bitflyer.Api.Raw.Private.Dtos;

/// <summary>
/// /v1/me/getcollateral のレスポンス DTO。
/// </summary>
public sealed class GetCollateralResponse
{
    [JsonPropertyName("collateral")] public decimal Collateral { get; init; }
    [JsonPropertyName("open_position_pnl")] public decimal OpenPositionPnl { get; init; }
    [JsonPropertyName("require_collateral")] public decimal RequireCollateral { get; init; }
    [JsonPropertyName("keep_rate")] public decimal KeepRate { get; init; }
}
