using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bittrade.Raw;

[JsonConverter(typeof(BittradeOrderTypeJsonConverter))]
public enum BittradeOrderType
{
    [EnumMember(Value = "buy-limit")]
    BuyLimit,
    [EnumMember(Value = "sell-limit")]
    SellLimit,
    [EnumMember(Value = "buy-market")]
    BuyMarket,
    [EnumMember(Value = "sell-market")]
    SellMarket,
    [EnumMember(Value = "buy-limit-maker")]
    BuyLimitMaker,
    [EnumMember(Value = "sell-limit-maker")]
    SellLimitMaker,
    [EnumMember(Value = "buy-ioc")]
    BuyIoc,
    [EnumMember(Value = "sell-ioc")]
    SellIoc
}
