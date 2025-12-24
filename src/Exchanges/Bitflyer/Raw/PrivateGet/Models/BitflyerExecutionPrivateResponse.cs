using System;
using System.Text.Json.Serialization;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Exchanges.Bitflyer.Raw.Types;
namespace ExchangeApi.Exchanges.Bitflyer.Raw.PrivateGet;

/// <summary>
/// Private 約定履歴 (/v1/me/getexecutions) のレスポンス DTO。
/// </summary>
public sealed class ExecutionPrivateResponse
{
    [JsonPropertyName("id")] public long Id { get; init; }
    [JsonPropertyName("product_code")] public RawProductCode ProductCode { get; init; }
    [JsonPropertyName("side")] public Side Side { get; init; }
    [JsonPropertyName("price")] public decimal Price { get; init; }
    [JsonPropertyName("size")] public decimal Size { get; init; }
    [JsonPropertyName("exec_date")] public DateTimeOffset ExecDate { get; init; }
    [JsonPropertyName("child_order_acceptance_id")] public string? ChildOrderAcceptanceId { get; init; }
}
