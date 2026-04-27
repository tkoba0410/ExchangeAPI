using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bitflyer.Protocol.Realtime;

public sealed class BitflyerRealtimeJsonRpcRequest
{
    [JsonPropertyName("jsonrpc")]
    public string JsonRpc { get; init; } = "2.0";

    [JsonPropertyName("id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Id { get; init; }

    [JsonPropertyName("method")]
    public required string Method { get; init; }

    [JsonPropertyName("params")]
    public required object Params { get; init; }
}

public sealed class BitflyerRealtimeJsonRpcRequestParams
{
    [JsonPropertyName("channel")]
    public required string Channel { get; init; }
}

public sealed class BitflyerRealtimeAuthenticationRequestParams
{
    [JsonPropertyName("api_key")]
    public required string ApiKey { get; init; }

    [JsonPropertyName("timestamp")]
    public required long Timestamp { get; init; }

    [JsonPropertyName("nonce")]
    public required string Nonce { get; init; }

    [JsonPropertyName("signature")]
    public required string Signature { get; init; }
}
