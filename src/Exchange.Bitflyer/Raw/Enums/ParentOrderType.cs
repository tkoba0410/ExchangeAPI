using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Exchange.Bitflyer.Raw;

/// <summary>親注文種別（一覧レスポンスの parent_order_type）。</summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ParentOrderType
{
    [EnumMember(Value = "SIMPLE")] Simple,
    [EnumMember(Value = "IFD")] Ifd,
    [EnumMember(Value = "OCO")] Oco,
    [EnumMember(Value = "IFDOCO")] Ifdoco,
    [EnumMember(Value = "STOP")] Stop,
}
