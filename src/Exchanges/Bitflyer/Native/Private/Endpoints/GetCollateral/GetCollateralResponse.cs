using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCollateral;

public sealed class GetCollateralResponse
{
    [JsonPropertyName("collateral")]
    public required decimal Collateral { get; init; }

    [JsonPropertyName("open_position_pnl")]
    public required decimal OpenPositionPnl { get; init; }

    [JsonPropertyName("require_collateral")]
    public required decimal RequireCollateral { get; init; }

    [JsonPropertyName("keep_rate")]
    public required decimal KeepRate { get; init; }

    [JsonPropertyName("margin_call_amount")]
    public decimal? MarginCallAmount { get; init; }

    [JsonPropertyName("margin_call_due_date")]
    public DateTimeOffset? MarginCallDueDate { get; init; }
}
