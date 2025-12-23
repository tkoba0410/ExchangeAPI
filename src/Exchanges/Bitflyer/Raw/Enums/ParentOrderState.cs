using System.Runtime.Serialization;
using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bitflyer.Raw;

/// <summary>親注文状態 (parent_order_state)。</summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ParentOrderStatusState
{
    [EnumMember(Value = "ACTIVE")] Active,
    [EnumMember(Value = "COMPLETED")] Completed,
    [EnumMember(Value = "CANCELED")] Canceled,
    [EnumMember(Value = "EXPIRED")] Expired,
    [EnumMember(Value = "REJECTED")] Rejected,
}
