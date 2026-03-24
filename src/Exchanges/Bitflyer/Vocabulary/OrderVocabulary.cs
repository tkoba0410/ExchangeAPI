namespace ExchangeApi.Exchanges.Bitflyer.Vocabulary;

public static class ChildOrderTypes
{
    public const string Limit = "LIMIT";
    public const string Market = "MARKET";
}

public static class OrderSides
{
    public const string Buy = "BUY";
    public const string Sell = "SELL";
}

public static class TimeInForces
{
    public const string Gtc = "GTC";
    public const string Ioc = "IOC";
    public const string Fok = "FOK";
}

public static class ChildOrderStates
{
    public const string Active = "ACTIVE";
    public const string Completed = "COMPLETED";
    public const string Canceled = "CANCELED";
    public const string Expired = "EXPIRED";
    public const string Rejected = "REJECTED";
}

public static class ParentOrderStates
{
    public const string Active = "ACTIVE";
    public const string Completed = "COMPLETED";
    public const string Canceled = "CANCELED";
    public const string Expired = "EXPIRED";
    public const string Rejected = "REJECTED";
}

public static class ParentOrderMethods
{
    public const string Simple = "SIMPLE";
    public const string Ifd = "IFD";
    public const string Oco = "OCO";
    public const string IfdOco = "IFDOCO";
}

public static class ParentOrderConditionTypes
{
    public const string Limit = "LIMIT";
    public const string Market = "MARKET";
    public const string Stop = "STOP";
    public const string StopLimit = "STOP_LIMIT";
    public const string Trail = "TRAIL";
}
