using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bitflyer.Api.Raw.Public.Dtos;

public sealed class ExecutionPublicResponse
{
    [JsonPropertyName("id")] public long Id { get; init; }
    [JsonPropertyName("product_code")] public string ProductCode { get; init; } = string.Empty;
    [JsonPropertyName("side")] public string Side { get; init; } = string.Empty;
    [JsonPropertyName("price")] public decimal Price { get; init; }
    [JsonPropertyName("size")] public decimal Size { get; init; }
    [JsonPropertyName("exec_date")] public DateTimeOffset ExecDate { get; init; }
    [JsonPropertyName("child_order_acceptance_id")] public string? ChildOrderAcceptanceId { get; init; }
}
