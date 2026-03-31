using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bitflyer.Vocabulary;

[JsonConverter(typeof(ApiStringEnumJsonConverter<BitflyerHealthStatus>))]
public enum BitflyerHealthStatus
{
    [ApiStringEnumValue("NORMAL")]
    Normal = 1,
    [ApiStringEnumValue("BUSY")]
    Busy = 2,
    [ApiStringEnumValue("VERY BUSY")]
    VeryBusy = 3,
    [ApiStringEnumValue("SUPER BUSY")]
    SuperBusy = 4,
    [ApiStringEnumValue("NO ORDER")]
    NoOrder = 5,
    [ApiStringEnumValue("STOP")]
    Stop = 6,
}

[JsonConverter(typeof(ApiStringEnumJsonConverter<BitflyerTradingState>))]
public enum BitflyerTradingState
{
    [ApiStringEnumValue("RUNNING")]
    Running = 1,
    [ApiStringEnumValue("CLOSED")]
    Closed = 2,
    [ApiStringEnumValue("STARTING")]
    Starting = 3,
    [ApiStringEnumValue("PREOPEN")]
    Preopen = 4,
    [ApiStringEnumValue("CIRCUIT BREAK")]
    CircuitBreak = 5,
    [ApiStringEnumValue("MATURED")]
    Matured = 6,
}

[JsonConverter(typeof(ApiStringEnumJsonConverter<BitflyerTransferStatus>))]
public enum BitflyerTransferStatus
{
    [ApiStringEnumValue("PENDING")]
    Pending = 1,
    [ApiStringEnumValue("COMPLETED")]
    Completed = 2,
}

[JsonConverter(typeof(ApiStringEnumJsonConverter<BitflyerMarketType>))]
public enum BitflyerMarketType
{
    [ApiStringEnumValue("Spot")]
    Spot = 1,
    [ApiStringEnumValue("FX")]
    Fx = 2,
}

[JsonConverter(typeof(ApiStringEnumJsonConverter<BitflyerTradeType>))]
public enum BitflyerTradeType
{
    [ApiStringEnumValue("BUY")]
    Buy = 1,
    [ApiStringEnumValue("SELL")]
    Sell = 2,
    [ApiStringEnumValue("DEPOSIT")]
    Deposit = 3,
    [ApiStringEnumValue("WITHDRAW")]
    Withdraw = 4,
    [ApiStringEnumValue("FEE")]
    Fee = 5,
    [ApiStringEnumValue("POST_COLL")]
    PostCollateral = 6,
    [ApiStringEnumValue("CANCEL_COLL")]
    CancelCollateral = 7,
    [ApiStringEnumValue("PAYMENT")]
    Payment = 8,
    [ApiStringEnumValue("TRANSFER")]
    Transfer = 9,
    [ApiStringEnumValue("RECEIVE")]
    Receive = 10,
}

[JsonConverter(typeof(ApiStringEnumJsonConverter<BitflyerAddressType>))]
public enum BitflyerAddressType
{
    [ApiStringEnumValue("UNKNOWN")]
    Unknown = 1,
    [ApiStringEnumValue("NORMAL")]
    Normal = 2,
}

[JsonConverter(typeof(ApiStringEnumJsonConverter<BitflyerParentOrderType>))]
public enum BitflyerParentOrderType
{
    [ApiStringEnumValue("UNKNOWN")]
    Unknown = 1,
    [ApiStringEnumValue("LIMIT")]
    Limit = 2,
    [ApiStringEnumValue("MARKET")]
    Market = 3,
    [ApiStringEnumValue("STOP")]
    Stop = 4,
    [ApiStringEnumValue("STOP_LIMIT")]
    StopLimit = 5,
    [ApiStringEnumValue("TRAIL")]
    Trail = 6,
    [ApiStringEnumValue("IFD")]
    Ifd = 7,
    [ApiStringEnumValue("OCO")]
    Oco = 8,
    [ApiStringEnumValue("IFDOCO")]
    IfdOco = 9,
}

public static class HealthStatuses
{
    public const BitflyerHealthStatus Normal = BitflyerHealthStatus.Normal;
    public const BitflyerHealthStatus Busy = BitflyerHealthStatus.Busy;
    public const BitflyerHealthStatus VeryBusy = BitflyerHealthStatus.VeryBusy;
    public const BitflyerHealthStatus SuperBusy = BitflyerHealthStatus.SuperBusy;
    public const BitflyerHealthStatus NoOrder = BitflyerHealthStatus.NoOrder;
    public const BitflyerHealthStatus Stop = BitflyerHealthStatus.Stop;
}

public static class TradingStates
{
    public const BitflyerTradingState Running = BitflyerTradingState.Running;
    public const BitflyerTradingState Closed = BitflyerTradingState.Closed;
    public const BitflyerTradingState Starting = BitflyerTradingState.Starting;
    public const BitflyerTradingState Preopen = BitflyerTradingState.Preopen;
    public const BitflyerTradingState CircuitBreak = BitflyerTradingState.CircuitBreak;
    public const BitflyerTradingState Matured = BitflyerTradingState.Matured;
}

public static class TransferStatuses
{
    public const BitflyerTransferStatus Pending = BitflyerTransferStatus.Pending;
    public const BitflyerTransferStatus Completed = BitflyerTransferStatus.Completed;
}

public static class MarketTypes
{
    public const BitflyerMarketType Spot = BitflyerMarketType.Spot;
    public const BitflyerMarketType Fx = BitflyerMarketType.Fx;
}

public static class TradeTypes
{
    public const BitflyerTradeType Buy = BitflyerTradeType.Buy;
    public const BitflyerTradeType Sell = BitflyerTradeType.Sell;
    public const BitflyerTradeType Deposit = BitflyerTradeType.Deposit;
    public const BitflyerTradeType Withdraw = BitflyerTradeType.Withdraw;
    public const BitflyerTradeType Fee = BitflyerTradeType.Fee;
    public const BitflyerTradeType PostCollateral = BitflyerTradeType.PostCollateral;
    public const BitflyerTradeType CancelCollateral = BitflyerTradeType.CancelCollateral;
    public const BitflyerTradeType Payment = BitflyerTradeType.Payment;
    public const BitflyerTradeType Transfer = BitflyerTradeType.Transfer;
    public const BitflyerTradeType Receive = BitflyerTradeType.Receive;
}

public static class AddressTypes
{
    public const BitflyerAddressType Unknown = BitflyerAddressType.Unknown;
    public const BitflyerAddressType Normal = BitflyerAddressType.Normal;
}

public static class ParentOrderTypes
{
    public const BitflyerParentOrderType Unknown = BitflyerParentOrderType.Unknown;
    public const BitflyerParentOrderType Limit = BitflyerParentOrderType.Limit;
    public const BitflyerParentOrderType Market = BitflyerParentOrderType.Market;
    public const BitflyerParentOrderType Stop = BitflyerParentOrderType.Stop;
    public const BitflyerParentOrderType StopLimit = BitflyerParentOrderType.StopLimit;
    public const BitflyerParentOrderType Trail = BitflyerParentOrderType.Trail;
    public const BitflyerParentOrderType Ifd = BitflyerParentOrderType.Ifd;
    public const BitflyerParentOrderType Oco = BitflyerParentOrderType.Oco;
    public const BitflyerParentOrderType IfdOco = BitflyerParentOrderType.IfdOco;
}
