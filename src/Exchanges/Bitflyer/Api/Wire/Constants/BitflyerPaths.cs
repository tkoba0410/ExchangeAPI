// Exchanges/Bitflyer/Wire/Constants/BitflyerPaths.cs
namespace ExchangeApi.Exchanges.Bitflyer.Api.Wire.Constants;

internal static class BitflyerPaths
{
    public const string GetMarketsPath = "/v1/getmarkets";
    public const string GetBoardPath = "/v1/getboard";
    public const string GetTickerPath = "/v1/getticker";
    public const string GetExecutionsPublicPath = "/v1/getexecutions";
    public const string GetBoardStatePath = "/v1/getboardstate";
    public const string GetHealthPath = "/v1/gethealth";
    public const string GetFundingRatePath = "/v1/getfundingrate";
    public const string GetCorporateLeveragePath = "/v1/getcorporateleverage";
    public const string GetChatsPath = "/v1/getchats";

    public const string GetPermissionsPath = "/v1/me/getpermissions";
    public const string GetBalancePath = "/v1/me/getbalance";
    public const string GetCollateralPath = "/v1/me/getcollateral";
    public const string GetCollateralAccountsPath = "/v1/me/getcollateralaccounts";
    public const string GetAddressesPath = "/v1/me/getaddresses";
    public const string GetCoinInsPath = "/v1/me/getcoinins";
    public const string GetCoinOutsPath = "/v1/me/getcoinouts";
    public const string GetBankAccountsPath = "/v1/me/getbankaccounts";
    public const string GetDepositsPath = "/v1/me/getdeposits";
    public const string WithdrawPath = "/v1/me/withdraw";
    public const string GetWithdrawalsPath = "/v1/me/getwithdrawals";

    public const string SendChildOrderPath = "/v1/me/sendchildorder";
    public const string SendParentOrderPath = "/v1/me/sendparentorder";
    public const string CancelChildOrderPath = "/v1/me/cancelchildorder";
    public const string CancelParentOrderPath = "/v1/me/cancelparentorder";
    public const string CancelAllChildOrdersPath = "/v1/me/cancelallchildorders";
    public const string GetChildOrdersPath = "/v1/me/getchildorders";
    public const string GetParentOrdersPath = "/v1/me/getparentorders";
    public const string GetParentOrderPath = "/v1/me/getparentorder";
    public const string GetExecutionsPrivatePath = "/v1/me/getexecutions";
    public const string GetBalanceHistoryPath = "/v1/me/getbalancehistory";
    public const string GetPositionsPath = "/v1/me/getpositions";
    public const string GetCollateralHistoryPath = "/v1/me/getcollateralhistory";
    public const string GetTradingCommissionPath = "/v1/me/gettradingcommission";
}
