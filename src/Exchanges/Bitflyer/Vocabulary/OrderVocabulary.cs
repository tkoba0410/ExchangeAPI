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
