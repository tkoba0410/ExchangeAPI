using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetTradingCommission;

public sealed class GetTradingCommissionResponse
{
    [JsonPropertyName("commission_rate")]
    public required decimal CommissionRate { get; init; }
}
