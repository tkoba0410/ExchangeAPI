using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bitflyer.Protocol.Realtime;

public sealed class BitflyerRealtimeJsonRpcResponse
{
    [JsonPropertyName("jsonrpc")]
    public string? JsonRpc { get; init; }

    [JsonPropertyName("id")]
    public object? Id { get; init; }

    [JsonPropertyName("result")]
    public object? Result { get; init; }

    [JsonPropertyName("error")]
    public BitflyerRealtimeJsonRpcError? Error { get; init; }
}

public sealed class BitflyerRealtimeJsonRpcError
{
    [JsonPropertyName("code")]
    public int? Code { get; init; }

    [JsonPropertyName("message")]
    public string? Message { get; init; }
}
