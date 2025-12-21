using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bittrade.Raw;

[JsonConverter(typeof(BittradeRetailOrderTypeJsonConverter))]
public enum BittradeRetailOrderType
{
    Buy = 1,
    Sell = 2
}
