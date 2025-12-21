using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bittrade.Raw;

[JsonConverter(typeof(BittradeOrderSideJsonConverter))]
public enum BittradeOrderSide
{
    [EnumMember(Value = "buy")]
    Buy,
    [EnumMember(Value = "sell")]
    Sell
}
