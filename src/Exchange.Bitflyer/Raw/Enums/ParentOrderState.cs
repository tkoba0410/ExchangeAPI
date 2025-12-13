using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Exchange.Bitflyer.Raw;

/// <summary>親注文状態 (parent_order_state)。</summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ParentOrderState
{
    [EnumMember(Value = "ACTIVE")] Active,
    [EnumMember(Value = "COMPLETED")] Completed,
    [EnumMember(Value = "CANCELED")] Canceled,
    [EnumMember(Value = "EXPIRED")] Expired,
    [EnumMember(Value = "REJECTED")] Rejected,
}
