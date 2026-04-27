using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bitflyer.Protocol.Realtime;

public sealed class BitflyerRealtimeJsonRpcRequest
{
    [JsonPropertyName("method")]
    public required string Method { get; init; }

    [JsonPropertyName("params")]
    public required BitflyerRealtimeJsonRpcRequestParams Params { get; init; }
}

public sealed class BitflyerRealtimeJsonRpcRequestParams
{
    [JsonPropertyName("channel")]
    public required string Channel { get; init; }
}
