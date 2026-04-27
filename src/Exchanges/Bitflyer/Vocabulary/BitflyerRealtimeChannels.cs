namespace ExchangeApi.Exchanges.Bitflyer.Vocabulary;

public static class BitflyerRealtimeChannels
{
    public static string Ticker(string productCode)
    {
        return Build("lightning_ticker", productCode);
    }

    public static string Executions(string productCode)
    {
        return Build("lightning_executions", productCode);
    }

    public static string BoardSnapshot(string productCode)
    {
        return Build("lightning_board_snapshot", productCode);
    }

    public static string Board(string productCode)
    {
        return Build("lightning_board", productCode);
    }

    public static string ChildOrderEvents()
    {
        return "child_order_events";
    }

    public static string ParentOrderEvents()
    {
        return "parent_order_events";
    }

    private static string Build(string prefix, string productCode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(productCode);

        return $"{prefix}_{productCode}";
    }
}
