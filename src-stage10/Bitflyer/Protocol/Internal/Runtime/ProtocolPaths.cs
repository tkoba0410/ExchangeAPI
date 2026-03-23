namespace ExchangeApi.Stage10.Bitflyer.Protocol.Internal.Runtime;

internal static class ProtocolPaths
{
    public const string GetTicker = "/v1/getticker";
    public const string GetTickerAlias = "/v1/ticker";
    public const string GetBalance = "/v1/me/getbalance";
    public const string SendChildOrder = "/v1/me/sendchildorder";
    public const string CancelChildOrder = "/v1/me/cancelchildorder";
}
