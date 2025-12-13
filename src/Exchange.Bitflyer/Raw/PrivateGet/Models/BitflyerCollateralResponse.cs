using System.Text.Json.Serialization;

namespace Exchange.Bitflyer.Raw;

/// <summary>
/// /v1/me/getcollateral のレスポンス DTO。
/// </summary>
public sealed class BitflyerCollateralResponse
{
    [JsonPropertyName("collateral")] public decimal Collateral { get; init; }
    [JsonPropertyName("open_position_pnl")] public decimal OpenPositionPnl { get; init; }
    [JsonPropertyName("require_collateral")] public decimal RequireCollateral { get; init; }
    [JsonPropertyName("keep_rate")] public decimal KeepRate { get; init; }
}
