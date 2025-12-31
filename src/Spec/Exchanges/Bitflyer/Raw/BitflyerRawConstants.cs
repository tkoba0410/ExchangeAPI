namespace ExchangeApi.Exchanges.Bitflyer.Raw;

/// <summary>bitFlyer 固有の定数（エンドポイントやキー名）。</summary>
internal static class BitflyerRawConstants
{
    internal static class Paths
    {
        public const string GetTicker = "/v1/getticker";
        public const string Ticker = "/v1/ticker";
        public const string GetBoard = "/v1/getboard";
        public const string Board = "/v1/board";
        public const string GetExecutions = "/v1/getexecutions";
        public const string Executions = "/v1/executions";
        public const string GetMarkets = "/v1/getmarkets";
        public const string Markets = "/v1/markets";
        public const string GetChats = "/v1/getchats";
        public const string GetHealth = "/v1/gethealth";
        public const string GetBoardState = "/v1/getboardstate";
        public const string GetCorporateLeverage = "/v1/getcorporateleverage";
        public const string GetFundingRate = "/v1/getfundingrate";
    }

    internal static class QueryKeys
    {
        public const string ProductCode = "product_code";
        public const string Count = "count";
        public const string Before = "before";
        public const string After = "after";
        public const string FromDate = "from_date";
    }
}
