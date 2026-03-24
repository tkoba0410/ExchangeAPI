using ExchangeApi.Exchanges.Bitflyer.Composition.Factory;
using ExchangeApi.Exchanges.Bitflyer.Composition.Options;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Api;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.CancelChildOrder;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.CancelAllChildOrders;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.CancelParentOrder;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetAddresses;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetBalance;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetBalanceHistory;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetBankAccounts;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCollateral;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCollateralHistory;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCollateralAccounts;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetChildOrders;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCoinIns;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCoinOuts;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetDeposits;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetExecutions;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetParentOrder;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetParentOrders;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetPermissions;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetPositions;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetTradingCommission;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetWithdrawals;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.SendParentOrder;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.SendChildOrder;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.Withdraw;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Api;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetBoard;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetBoardState;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetChats;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetCorporateLeverage;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetExecutionsPublic;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetFundingRate;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetHealth;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetMarkets;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetTicker;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Runtime;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Api;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.CancelChildOrder;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.CancelAllChildOrders;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.CancelParentOrder;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetAddresses;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetBalance;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetBalanceHistory;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetBankAccounts;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetCollateral;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetCollateralHistory;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetCollateralAccounts;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetChildOrders;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetCoinIns;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetCoinOuts;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetDeposits;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetExecutions;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetParentOrder;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetParentOrders;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetPermissions;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetPositions;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetTradingCommission;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetWithdrawals;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.SendParentOrder;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.SendChildOrder;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.Withdraw;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Api;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetBoard;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetBoardState;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetChats;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetCorporateLeverage;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetExecutionsPublic;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetFundingRate;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetHealth;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetMarkets;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetTicker;

namespace ExchangeApi.Exchanges.Bitflyer.Composition.Bootstrap;

internal static class BitflyerBootstrap
{
    internal static BitflyerProtocolBundle CreateProtocolBundle(BitflyerClientOptions? options)
    {
        var normalizedOptions = options ?? new BitflyerClientOptions();
        var transport = CreateTransport(normalizedOptions);

        var getMarkets = new GetMarketsProtocolEndpoint(transport);
        var getBoard = new GetBoardProtocolEndpoint(transport);
        var getBoardState = new GetBoardStateProtocolEndpoint(transport);
        var getHealth = new GetHealthProtocolEndpoint(transport);
        var getFundingRate = new GetFundingRateProtocolEndpoint(transport);
        var getCorporateLeverage = new GetCorporateLeverageProtocolEndpoint(transport);
        var getChats = new GetChatsProtocolEndpoint(transport);
        var getExecutionsPublic = new GetExecutionsPublicProtocolEndpoint(transport);
        var getTicker = new GetTickerProtocolEndpoint(transport, normalizedOptions.UseTickerAliasPath);
        var publicApi = new BitflyerPublicProtocolApi(
            getMarkets,
            getBoard,
            getBoardState,
            getHealth,
            getFundingRate,
            getCorporateLeverage,
            getChats,
            getExecutionsPublic,
            getTicker);

        if (normalizedOptions.Credentials is null)
        {
            return new BitflyerProtocolBundle
            {
                Public = publicApi,
                Private = null,
            };
        }

        var getBalance = new GetBalanceProtocolEndpoint(transport);
        var getPermissions = new GetPermissionsProtocolEndpoint(transport);
        var getAddresses = new GetAddressesProtocolEndpoint(transport);
        var getCoinIns = new GetCoinInsProtocolEndpoint(transport);
        var getCoinOuts = new GetCoinOutsProtocolEndpoint(transport);
        var getBankAccounts = new GetBankAccountsProtocolEndpoint(transport);
        var getDeposits = new GetDepositsProtocolEndpoint(transport);
        var withdraw = new WithdrawProtocolEndpoint(transport);
        var getWithdrawals = new GetWithdrawalsProtocolEndpoint(transport);
        var getParentOrders = new GetParentOrdersProtocolEndpoint(transport);
        var getParentOrder = new GetParentOrderProtocolEndpoint(transport);
        var getCollateral = new GetCollateralProtocolEndpoint(transport);
        var getCollateralAccounts = new GetCollateralAccountsProtocolEndpoint(transport);
        var getCollateralHistory = new GetCollateralHistoryProtocolEndpoint(transport);
        var getChildOrders = new GetChildOrdersProtocolEndpoint(transport);
        var getExecutions = new GetExecutionsProtocolEndpoint(transport);
        var getBalanceHistory = new GetBalanceHistoryProtocolEndpoint(transport);
        var getPositions = new GetPositionsProtocolEndpoint(transport);
        var getTradingCommission = new GetTradingCommissionProtocolEndpoint(transport);
        var sendChildOrder = new SendChildOrderProtocolEndpoint(transport);
        var sendParentOrder = new SendParentOrderProtocolEndpoint(transport);
        var cancelChildOrder = new CancelChildOrderProtocolEndpoint(transport);
        var cancelAllChildOrders = new CancelAllChildOrdersProtocolEndpoint(transport);
        var cancelParentOrder = new CancelParentOrderProtocolEndpoint(transport);
        var privateApi = new BitflyerPrivateProtocolApi(
            getPermissions,
            getAddresses,
            getCoinIns,
            getCoinOuts,
            getBankAccounts,
            getDeposits,
            withdraw,
            getWithdrawals,
            getBalance,
            getParentOrders,
            getParentOrder,
            getCollateral,
            getCollateralAccounts,
            getCollateralHistory,
            getChildOrders,
            getExecutions,
            getBalanceHistory,
            getPositions,
            getTradingCommission,
            sendChildOrder,
            sendParentOrder,
            cancelChildOrder,
            cancelAllChildOrders,
            cancelParentOrder);

        return new BitflyerProtocolBundle
        {
            Public = publicApi,
            Private = privateApi,
        };
    }

    internal static BitflyerNativeBundle CreateNativeBundle(BitflyerClientOptions? options)
    {
        var normalizedOptions = options ?? new BitflyerClientOptions();
        var transport = CreateTransport(normalizedOptions);

        var getMarketsProtocol = new GetMarketsProtocolEndpoint(transport);
        var getBoardProtocol = new GetBoardProtocolEndpoint(transport);
        var getBoardStateProtocol = new GetBoardStateProtocolEndpoint(transport);
        var getHealthProtocol = new GetHealthProtocolEndpoint(transport);
        var getFundingRateProtocol = new GetFundingRateProtocolEndpoint(transport);
        var getCorporateLeverageProtocol = new GetCorporateLeverageProtocolEndpoint(transport);
        var getChatsProtocol = new GetChatsProtocolEndpoint(transport);
        var getExecutionsPublicProtocol = new GetExecutionsPublicProtocolEndpoint(transport);
        var getTickerProtocol = new GetTickerProtocolEndpoint(transport, normalizedOptions.UseTickerAliasPath);
        var publicProtocolApi = new BitflyerPublicProtocolApi(
            getMarketsProtocol,
            getBoardProtocol,
            getBoardStateProtocol,
            getHealthProtocol,
            getFundingRateProtocol,
            getCorporateLeverageProtocol,
            getChatsProtocol,
            getExecutionsPublicProtocol,
            getTickerProtocol);
        var getMarketsNative = new GetMarketsNativeEndpoint(getMarketsProtocol);
        var getBoardNative = new GetBoardNativeEndpoint(getBoardProtocol);
        var getBoardStateNative = new GetBoardStateNativeEndpoint(getBoardStateProtocol);
        var getHealthNative = new GetHealthNativeEndpoint(getHealthProtocol);
        var getFundingRateNative = new GetFundingRateNativeEndpoint(getFundingRateProtocol);
        var getCorporateLeverageNative = new GetCorporateLeverageNativeEndpoint(getCorporateLeverageProtocol);
        var getChatsNative = new GetChatsNativeEndpoint(getChatsProtocol);
        var getExecutionsPublicNative = new GetExecutionsPublicNativeEndpoint(getExecutionsPublicProtocol);
        var getTickerNative = new GetTickerNativeEndpoint(getTickerProtocol);
        var publicApi = new BitflyerPublicNativeApi(
            getMarketsNative,
            getBoardNative,
            getBoardStateNative,
            getHealthNative,
            getFundingRateNative,
            getCorporateLeverageNative,
            getChatsNative,
            getExecutionsPublicNative,
            getTickerNative);

        if (normalizedOptions.Credentials is null)
        {
            return new BitflyerNativeBundle
            {
                Public = publicApi,
                Private = null,
                Protocol = new BitflyerProtocolBundle
                {
                    Public = publicProtocolApi,
                    Private = null,
                },
            };
        }

        var getBalanceProtocol = new GetBalanceProtocolEndpoint(transport);
        var getPermissionsProtocol = new GetPermissionsProtocolEndpoint(transport);
        var getAddressesProtocol = new GetAddressesProtocolEndpoint(transport);
        var getCoinInsProtocol = new GetCoinInsProtocolEndpoint(transport);
        var getCoinOutsProtocol = new GetCoinOutsProtocolEndpoint(transport);
        var getBankAccountsProtocol = new GetBankAccountsProtocolEndpoint(transport);
        var getDepositsProtocol = new GetDepositsProtocolEndpoint(transport);
        var withdrawProtocol = new WithdrawProtocolEndpoint(transport);
        var getWithdrawalsProtocol = new GetWithdrawalsProtocolEndpoint(transport);
        var getParentOrdersProtocol = new GetParentOrdersProtocolEndpoint(transport);
        var getParentOrderProtocol = new GetParentOrderProtocolEndpoint(transport);
        var getCollateralProtocol = new GetCollateralProtocolEndpoint(transport);
        var getCollateralAccountsProtocol = new GetCollateralAccountsProtocolEndpoint(transport);
        var getCollateralHistoryProtocol = new GetCollateralHistoryProtocolEndpoint(transport);
        var getChildOrdersProtocol = new GetChildOrdersProtocolEndpoint(transport);
        var getExecutionsProtocol = new GetExecutionsProtocolEndpoint(transport);
        var getBalanceHistoryProtocol = new GetBalanceHistoryProtocolEndpoint(transport);
        var getPositionsProtocol = new GetPositionsProtocolEndpoint(transport);
        var getTradingCommissionProtocol = new GetTradingCommissionProtocolEndpoint(transport);
        var sendChildOrderProtocol = new SendChildOrderProtocolEndpoint(transport);
        var sendParentOrderProtocol = new SendParentOrderProtocolEndpoint(transport);
        var cancelChildOrderProtocol = new CancelChildOrderProtocolEndpoint(transport);
        var cancelAllChildOrdersProtocol = new CancelAllChildOrdersProtocolEndpoint(transport);
        var cancelParentOrderProtocol = new CancelParentOrderProtocolEndpoint(transport);

        var privateProtocolApi = new BitflyerPrivateProtocolApi(
            getPermissionsProtocol,
            getAddressesProtocol,
            getCoinInsProtocol,
            getCoinOutsProtocol,
            getBankAccountsProtocol,
            getDepositsProtocol,
            withdrawProtocol,
            getWithdrawalsProtocol,
            getBalanceProtocol,
            getParentOrdersProtocol,
            getParentOrderProtocol,
            getCollateralProtocol,
            getCollateralAccountsProtocol,
            getCollateralHistoryProtocol,
            getChildOrdersProtocol,
            getExecutionsProtocol,
            getBalanceHistoryProtocol,
            getPositionsProtocol,
            getTradingCommissionProtocol,
            sendChildOrderProtocol,
            sendParentOrderProtocol,
            cancelChildOrderProtocol,
            cancelAllChildOrdersProtocol,
            cancelParentOrderProtocol);

        var getPermissions = new GetPermissionsNativeEndpoint(getPermissionsProtocol);
        var getAddresses = new GetAddressesNativeEndpoint(getAddressesProtocol);
        var getCoinIns = new GetCoinInsNativeEndpoint(getCoinInsProtocol);
        var getCoinOuts = new GetCoinOutsNativeEndpoint(getCoinOutsProtocol);
        var getBankAccounts = new GetBankAccountsNativeEndpoint(getBankAccountsProtocol);
        var getDeposits = new GetDepositsNativeEndpoint(getDepositsProtocol);
        var withdraw = new WithdrawNativeEndpoint(withdrawProtocol);
        var getWithdrawals = new GetWithdrawalsNativeEndpoint(getWithdrawalsProtocol);
        var getBalance = new GetBalanceNativeEndpoint(getBalanceProtocol);
        var getParentOrders = new GetParentOrdersNativeEndpoint(getParentOrdersProtocol);
        var getParentOrder = new GetParentOrderNativeEndpoint(getParentOrderProtocol);
        var getCollateral = new GetCollateralNativeEndpoint(getCollateralProtocol);
        var getCollateralAccounts = new GetCollateralAccountsNativeEndpoint(getCollateralAccountsProtocol);
        var getCollateralHistory = new GetCollateralHistoryNativeEndpoint(getCollateralHistoryProtocol);
        var getChildOrders = new GetChildOrdersNativeEndpoint(getChildOrdersProtocol);
        var getExecutions = new GetExecutionsNativeEndpoint(getExecutionsProtocol);
        var getBalanceHistory = new GetBalanceHistoryNativeEndpoint(getBalanceHistoryProtocol);
        var getPositions = new GetPositionsNativeEndpoint(getPositionsProtocol);
        var getTradingCommission = new GetTradingCommissionNativeEndpoint(getTradingCommissionProtocol);
        var sendChildOrder = new SendChildOrderNativeEndpoint(sendChildOrderProtocol);
        var sendParentOrder = new SendParentOrderNativeEndpoint(sendParentOrderProtocol);
        var cancelChildOrder = new CancelChildOrderNativeEndpoint(cancelChildOrderProtocol);
        var cancelAllChildOrders = new CancelAllChildOrdersNativeEndpoint(cancelAllChildOrdersProtocol);
        var cancelParentOrder = new CancelParentOrderNativeEndpoint(cancelParentOrderProtocol);
        var privateApi = new BitflyerPrivateNativeApi(
            getPermissions,
            getAddresses,
            getCoinIns,
            getCoinOuts,
            getBankAccounts,
            getDeposits,
            withdraw,
            getWithdrawals,
            getBalance,
            getParentOrders,
            getParentOrder,
            getCollateral,
            getCollateralAccounts,
            getCollateralHistory,
            getChildOrders,
            getExecutions,
            getBalanceHistory,
            getPositions,
            getTradingCommission,
            sendChildOrder,
            sendParentOrder,
            cancelChildOrder,
            cancelAllChildOrders,
            cancelParentOrder);

        return new BitflyerNativeBundle
        {
            Public = publicApi,
            Private = privateApi,
            Protocol = new BitflyerProtocolBundle
            {
                Public = publicProtocolApi,
                Private = privateProtocolApi,
            },
        };
    }

    private static IProtocolTransport CreateTransport(BitflyerClientOptions normalizedOptions)
    {
        var httpClient = new HttpClient
        {
            BaseAddress = normalizedOptions.BaseUri,
        };

        IProtocolDebugLogger debugLogger = normalizedOptions.EnableProtocolDebugLogging
            ? new FileProtocolDebugLogger(normalizedOptions.ProtocolDebugLogDirectory)
            : new NoOpProtocolDebugLogger();

        return new BitflyerProtocolTransport(
            httpClient,
            debugLogger,
            normalizedOptions.Credentials?.ApiKey,
            normalizedOptions.Credentials?.ApiSecret);
    }
}
