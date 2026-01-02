namespace ExchangeApi.Exchanges.Bitflyer.Wire.Constants;

/// <summary>bitFlyer 固有の定数（エンドポイントやキー名）。</summary>
internal static class BitflyerConstants
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

        // Private GET
        public const string GetPermissions = "/v1/me/getpermissions";
        public const string GetBalance = "/v1/me/getbalance";
        public const string GetCollateral = "/v1/me/getcollateral";
        public const string GetCollateralAccounts = "/v1/me/getcollateralaccounts";
        public const string GetChildOrders = "/v1/me/getchildorders";
        public const string GetParentOrders = "/v1/me/getparentorders";
        public const string GetParentOrder = "/v1/me/getparentorder";
        public const string GetPrivateExecutions = "/v1/me/getexecutions";
        public const string GetBalanceHistory = "/v1/me/getbalancehistory";
        public const string GetPositions = "/v1/me/getpositions";
        public const string GetCollateralHistory = "/v1/me/getcollateralhistory";
        public const string GetTradingCommission = "/v1/me/gettradingcommission";
        public const string GetAddresses = "/v1/me/getaddresses";
        public const string GetCoinIns = "/v1/me/getcoinins";
        public const string GetCoinOuts = "/v1/me/getcoinouts";
        public const string GetDeposits = "/v1/me/getdeposits";
        public const string GetWithdrawals = "/v1/me/getwithdrawals";
        public const string GetBankAccounts = "/v1/me/getbankaccounts";

        // Private POST
        public const string SendChildOrder = "/v1/me/sendchildorder";
        public const string SendParentOrder = "/v1/me/sendparentorder";
        public const string CancelChildOrder = "/v1/me/cancelchildorder";
        public const string CancelParentOrder = "/v1/me/cancelparentorder";
        public const string CancelAllChildOrders = "/v1/me/cancelallchildorders";
        public const string Withdraw = "/v1/me/withdraw";
    }

    internal static class QueryKeys
    {
        public const string ProductCode = "product_code";
        public const string Count = "count";
        public const string Before = "before";
        public const string After = "after";
        public const string ChildOrderStatusState = "child_order_state";
        public const string ChildOrderAcceptanceId = "child_order_acceptance_id";
        public const string ChildOrderId = "child_order_id";
        public const string ParentOrderId = "parent_order_id";
        public const string ParentOrderAcceptanceId = "parent_order_acceptance_id";
        public const string ParentOrderStatusState = "parent_order_state";
        public const string CurrencyCode = "currency_code";
        public const string MessageId = "message_id";
        public const string FromDate = "from_date";
    }

}
