using System.Collections.Generic;

namespace ExchangeApi.Exchanges.Bitflyer.Wire.Constants;

public static class EndpointIdCatalog
{
    private static readonly string[] Endpoints =
    {
        EndpointIds.GetMarkets,
        EndpointIds.GetBoard,
        EndpointIds.GetTicker,
        EndpointIds.GetExecutionsPublic,
        EndpointIds.GetBoardState,
        EndpointIds.GetHealth,
        EndpointIds.GetFundingRate,
        EndpointIds.GetCorporateLeverage,
        EndpointIds.GetChats,
        EndpointIds.GetPermissions,
        EndpointIds.GetBalance,
        EndpointIds.GetCollateral,
        EndpointIds.GetCollateralAccounts,
        EndpointIds.GetAddresses,
        EndpointIds.GetCoinIns,
        EndpointIds.GetCoinOuts,
        EndpointIds.GetBankAccounts,
        EndpointIds.GetDeposits,
        EndpointIds.Withdraw,
        EndpointIds.GetWithdrawals,
        EndpointIds.SendChildOrder,
        EndpointIds.SendParentOrder,
        EndpointIds.CancelChildOrder,
        EndpointIds.CancelParentOrder,
        EndpointIds.CancelAllChildOrders,
        EndpointIds.GetChildOrders,
        EndpointIds.GetParentOrders,
        EndpointIds.GetParentOrder,
        EndpointIds.GetExecutionsPrivate,
        EndpointIds.GetBalanceHistory,
        EndpointIds.GetPositions,
        EndpointIds.GetCollateralHistory,
        EndpointIds.GetTradingCommission,
    };

    public static IReadOnlyCollection<string> GetAllEndpointIds() =>
        Endpoints;
}
