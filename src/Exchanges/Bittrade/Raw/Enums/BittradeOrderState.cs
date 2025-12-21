using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bittrade.Raw;

[JsonConverter(typeof(BittradeOrderStateJsonConverter))]
public enum BittradeOrderState
{
    [EnumMember(Value = "submitted")]
    Submitted,
    [EnumMember(Value = "partial-filled")]
    PartialFilled,
    [EnumMember(Value = "partial-canceled")]
    PartialCanceled,
    [EnumMember(Value = "filled")]
    Filled,
    [EnumMember(Value = "canceled")]
    Canceled
}
