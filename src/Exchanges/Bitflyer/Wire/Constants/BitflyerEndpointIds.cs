// Exchanges/Bitflyer/Wire/Constants/BitflyerEndpointIds.cs
namespace ExchangeApi.Exchanges.Bitflyer.Wire.Constants;

public static class BitflyerEndpointIds
{
    public const string GetMarkets = "GetMarkets";
    public const string GetBoard = "GetBoard";
    public const string GetTicker = "GetTicker";
    public const string GetExecutionsPublic = "GetExecutionsPublic";
    public const string GetBoardState = "GetBoardState";
    public const string GetHealth = "GetHealth";
    public const string GetFundingRate = "GetFundingRate";
    public const string GetCorporateLeverage = "GetCorporateLeverage";
    public const string GetChats = "GetChats";

    public const string GetPermissions = "GetPermissions";
    public const string GetBalance = "GetBalance";
    public const string GetCollateral = "GetCollateral";
    public const string GetCollateralAccounts = "GetCollateralAccounts";
    public const string GetAddresses = "GetAddresses";
    public const string GetCoinIns = "GetCoinIns";
    public const string GetCoinOuts = "GetCoinOuts";
    public const string GetBankAccounts = "GetBankAccounts";
    public const string GetDeposits = "GetDeposits";
    public const string Withdraw = "Withdraw";
    public const string GetWithdrawals = "GetWithdrawals";

    public const string SendChildOrder = "SendChildOrder";
    public const string SendParentOrder = "SendParentOrder";
    public const string CancelChildOrder = "CancelChildOrder";
    public const string CancelParentOrder = "CancelParentOrder";
    public const string CancelAllChildOrders = "CancelAllChildOrders";
    public const string GetChildOrders = "GetChildOrders";
    public const string GetParentOrders = "GetParentOrders";
    public const string GetParentOrder = "GetParentOrder";
    public const string GetExecutionsPrivate = "GetExecutionsPrivate";
    public const string GetBalanceHistory = "GetBalanceHistory";
    public const string GetPositions = "GetPositions";
    public const string GetCollateralHistory = "GetCollateralHistory";
    public const string GetTradingCommission = "GetTradingCommission";
}
