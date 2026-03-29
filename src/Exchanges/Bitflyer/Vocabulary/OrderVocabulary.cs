using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bitflyer.Vocabulary;

[JsonConverter(typeof(ApiStringEnumJsonConverter<BitflyerChildOrderType>))]
public enum BitflyerChildOrderType
{
    [ApiStringEnumValue("LIMIT")]
    Limit = 1,
    [ApiStringEnumValue("MARKET")]
    Market = 2,
}

[JsonConverter(typeof(ApiStringEnumJsonConverter<BitflyerOrderSide>))]
public enum BitflyerOrderSide
{
    [ApiStringEnumValue("BUY")]
    Buy = 1,
    [ApiStringEnumValue("SELL")]
    Sell = 2,
}

[JsonConverter(typeof(ApiStringEnumJsonConverter<BitflyerTimeInForce>))]
public enum BitflyerTimeInForce
{
    [ApiStringEnumValue("GTC")]
    Gtc = 1,
    [ApiStringEnumValue("IOC")]
    Ioc = 2,
    [ApiStringEnumValue("FOK")]
    Fok = 3,
}

[JsonConverter(typeof(ApiStringEnumJsonConverter<BitflyerOrderState>))]
public enum BitflyerOrderState
{
    [ApiStringEnumValue("ACTIVE")]
    Active = 1,
    [ApiStringEnumValue("COMPLETED")]
    Completed = 2,
    [ApiStringEnumValue("CANCELED")]
    Canceled = 3,
    [ApiStringEnumValue("EXPIRED")]
    Expired = 4,
    [ApiStringEnumValue("REJECTED")]
    Rejected = 5,
}

[JsonConverter(typeof(ApiStringEnumJsonConverter<BitflyerOrderMethod>))]
public enum BitflyerOrderMethod
{
    [ApiStringEnumValue("SIMPLE")]
    Simple = 1,
    [ApiStringEnumValue("IFD")]
    Ifd = 2,
    [ApiStringEnumValue("OCO")]
    Oco = 3,
    [ApiStringEnumValue("IFDOCO")]
    IfdOco = 4,
}

[JsonConverter(typeof(ApiStringEnumJsonConverter<BitflyerConditionType>))]
public enum BitflyerConditionType
{
    [ApiStringEnumValue("LIMIT")]
    Limit = 1,
    [ApiStringEnumValue("MARKET")]
    Market = 2,
    [ApiStringEnumValue("STOP")]
    Stop = 3,
    [ApiStringEnumValue("STOP_LIMIT")]
    StopLimit = 4,
    [ApiStringEnumValue("TRAIL")]
    Trail = 5,
}

public static class ChildOrderTypes
{
    public const BitflyerChildOrderType Limit = BitflyerChildOrderType.Limit;
    public const BitflyerChildOrderType Market = BitflyerChildOrderType.Market;
}

public static class OrderSides
{
    public const BitflyerOrderSide Buy = BitflyerOrderSide.Buy;
    public const BitflyerOrderSide Sell = BitflyerOrderSide.Sell;
}

public static class TimeInForces
{
    public const BitflyerTimeInForce Gtc = BitflyerTimeInForce.Gtc;
    public const BitflyerTimeInForce Ioc = BitflyerTimeInForce.Ioc;
    public const BitflyerTimeInForce Fok = BitflyerTimeInForce.Fok;
}

public static class ChildOrderStates
{
    public const BitflyerOrderState Active = BitflyerOrderState.Active;
    public const BitflyerOrderState Completed = BitflyerOrderState.Completed;
    public const BitflyerOrderState Canceled = BitflyerOrderState.Canceled;
    public const BitflyerOrderState Expired = BitflyerOrderState.Expired;
    public const BitflyerOrderState Rejected = BitflyerOrderState.Rejected;
}

public static class ParentOrderStates
{
    public const BitflyerOrderState Active = BitflyerOrderState.Active;
    public const BitflyerOrderState Completed = BitflyerOrderState.Completed;
    public const BitflyerOrderState Canceled = BitflyerOrderState.Canceled;
    public const BitflyerOrderState Expired = BitflyerOrderState.Expired;
    public const BitflyerOrderState Rejected = BitflyerOrderState.Rejected;
}

public static class ParentOrderMethods
{
    public const BitflyerOrderMethod Simple = BitflyerOrderMethod.Simple;
    public const BitflyerOrderMethod Ifd = BitflyerOrderMethod.Ifd;
    public const BitflyerOrderMethod Oco = BitflyerOrderMethod.Oco;
    public const BitflyerOrderMethod IfdOco = BitflyerOrderMethod.IfdOco;
}

public static class ParentOrderConditionTypes
{
    public const BitflyerConditionType Limit = BitflyerConditionType.Limit;
    public const BitflyerConditionType Market = BitflyerConditionType.Market;
    public const BitflyerConditionType Stop = BitflyerConditionType.Stop;
    public const BitflyerConditionType StopLimit = BitflyerConditionType.StopLimit;
    public const BitflyerConditionType Trail = BitflyerConditionType.Trail;
}
