using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace Exchange.Bittrade.Raw;

public sealed record BittradeAccountsResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("data")] IReadOnlyList<BittradeAccount>? Data);
