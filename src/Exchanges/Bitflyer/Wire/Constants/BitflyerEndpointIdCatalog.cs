using System;
using System.Collections.Generic;

namespace ExchangeApi.Exchanges.Bitflyer.Wire.Constants;

public static class BitflyerEndpointIdCatalog
{
    private static readonly string[] Endpoints =
    {
        BitflyerEndpointIds.GetMarkets,
        BitflyerEndpointIds.GetBoard,
        BitflyerEndpointIds.GetTicker,
        BitflyerEndpointIds.GetExecutionsPublic,
        BitflyerEndpointIds.GetBoardState,
        BitflyerEndpointIds.GetHealth,
        BitflyerEndpointIds.GetFundingRate,
        BitflyerEndpointIds.GetCorporateLeverage,
        BitflyerEndpointIds.GetChats,
        BitflyerEndpointIds.GetPermissions,
        BitflyerEndpointIds.GetBalance,
        BitflyerEndpointIds.GetCollateral,
        BitflyerEndpointIds.GetCollateralAccounts,
        BitflyerEndpointIds.GetAddresses,
        BitflyerEndpointIds.GetCoinIns,
        BitflyerEndpointIds.GetCoinOuts,
        BitflyerEndpointIds.GetBankAccounts,
        BitflyerEndpointIds.GetDeposits,
        BitflyerEndpointIds.Withdraw,
        BitflyerEndpointIds.GetWithdrawals,
        BitflyerEndpointIds.SendChildOrder,
        BitflyerEndpointIds.SendParentOrder,
        BitflyerEndpointIds.CancelChildOrder,
        BitflyerEndpointIds.CancelParentOrder,
        BitflyerEndpointIds.CancelAllChildOrders,
        BitflyerEndpointIds.GetChildOrders,
        BitflyerEndpointIds.GetParentOrders,
        BitflyerEndpointIds.GetParentOrder,
        BitflyerEndpointIds.GetExecutionsPrivate,
        BitflyerEndpointIds.GetBalanceHistory,
        BitflyerEndpointIds.GetPositions,
        BitflyerEndpointIds.GetCollateralHistory,
        BitflyerEndpointIds.GetTradingCommission,
    };

    private static readonly string[] NotImplemented = Array.Empty<string>();

    public static IReadOnlyCollection<string> GetAllEndpointIds() =>
        Endpoints;

    public static IReadOnlyCollection<string> GetNotImplementedEndpointIds() => NotImplemented;
}
