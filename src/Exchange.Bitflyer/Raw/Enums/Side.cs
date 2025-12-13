using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Exchange.Bitflyer.Raw;

/// <summary>bitFlyer サイド (BUY/SELL)。</summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Side
{
    [EnumMember(Value = "BUY")]
    Buy,
    [EnumMember(Value = "SELL")]
    Sell,
}
