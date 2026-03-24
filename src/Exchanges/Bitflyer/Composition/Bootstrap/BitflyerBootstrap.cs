using ExchangeApi.Exchanges.Bitflyer.Composition.Factory;
using ExchangeApi.Exchanges.Bitflyer.Composition.Options;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Api;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.CancelChildOrder;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.CancelAllChildOrders;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetBalance;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCollateral;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCollateralHistory;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCollateralAccounts;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetChildOrders;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetExecutions;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetPositions;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetTradingCommission;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.SendChildOrder;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Api;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetBoard;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetExecutionsPublic;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetMarkets;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetTicker;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Runtime;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Api;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.CancelChildOrder;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.CancelAllChildOrders;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetBalance;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetCollateral;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetCollateralHistory;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetCollateralAccounts;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetChildOrders;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetExecutions;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetPositions;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetTradingCommission;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.SendChildOrder;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Api;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetBoard;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetExecutionsPublic;
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
        var getExecutionsPublic = new GetExecutionsPublicProtocolEndpoint(transport);
        var getTicker = new GetTickerProtocolEndpoint(transport, normalizedOptions.UseTickerAliasPath);
        var publicApi = new BitflyerPublicProtocolApi(getMarkets, getBoard, getExecutionsPublic, getTicker);

        if (normalizedOptions.Credentials is null)
        {
            return new BitflyerProtocolBundle
            {
                Public = publicApi,
                Private = null,
            };
        }

        var getBalance = new GetBalanceProtocolEndpoint(transport);
        var getCollateral = new GetCollateralProtocolEndpoint(transport);
        var getCollateralAccounts = new GetCollateralAccountsProtocolEndpoint(transport);
        var getCollateralHistory = new GetCollateralHistoryProtocolEndpoint(transport);
        var getChildOrders = new GetChildOrdersProtocolEndpoint(transport);
        var getExecutions = new GetExecutionsProtocolEndpoint(transport);
        var getPositions = new GetPositionsProtocolEndpoint(transport);
        var getTradingCommission = new GetTradingCommissionProtocolEndpoint(transport);
        var sendChildOrder = new SendChildOrderProtocolEndpoint(transport);
        var cancelChildOrder = new CancelChildOrderProtocolEndpoint(transport);
        var cancelAllChildOrders = new CancelAllChildOrdersProtocolEndpoint(transport);
        var privateApi = new BitflyerPrivateProtocolApi(
            getBalance,
            getCollateral,
            getCollateralAccounts,
            getCollateralHistory,
            getChildOrders,
            getExecutions,
            getPositions,
            getTradingCommission,
            sendChildOrder,
            cancelChildOrder,
            cancelAllChildOrders);

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
        var getExecutionsPublicProtocol = new GetExecutionsPublicProtocolEndpoint(transport);
        var getTickerProtocol = new GetTickerProtocolEndpoint(transport, normalizedOptions.UseTickerAliasPath);
        var publicProtocolApi = new BitflyerPublicProtocolApi(getMarketsProtocol, getBoardProtocol, getExecutionsPublicProtocol, getTickerProtocol);
        var getMarketsNative = new GetMarketsNativeEndpoint(getMarketsProtocol);
        var getBoardNative = new GetBoardNativeEndpoint(getBoardProtocol);
        var getExecutionsPublicNative = new GetExecutionsPublicNativeEndpoint(getExecutionsPublicProtocol);
        var getTickerNative = new GetTickerNativeEndpoint(getTickerProtocol);
        var publicApi = new BitflyerPublicNativeApi(getMarketsNative, getBoardNative, getExecutionsPublicNative, getTickerNative);

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
        var getCollateralProtocol = new GetCollateralProtocolEndpoint(transport);
        var getCollateralAccountsProtocol = new GetCollateralAccountsProtocolEndpoint(transport);
        var getCollateralHistoryProtocol = new GetCollateralHistoryProtocolEndpoint(transport);
        var getChildOrdersProtocol = new GetChildOrdersProtocolEndpoint(transport);
        var getExecutionsProtocol = new GetExecutionsProtocolEndpoint(transport);
        var getPositionsProtocol = new GetPositionsProtocolEndpoint(transport);
        var getTradingCommissionProtocol = new GetTradingCommissionProtocolEndpoint(transport);
        var sendChildOrderProtocol = new SendChildOrderProtocolEndpoint(transport);
        var cancelChildOrderProtocol = new CancelChildOrderProtocolEndpoint(transport);
        var cancelAllChildOrdersProtocol = new CancelAllChildOrdersProtocolEndpoint(transport);

        var privateProtocolApi = new BitflyerPrivateProtocolApi(
            getBalanceProtocol,
            getCollateralProtocol,
            getCollateralAccountsProtocol,
            getCollateralHistoryProtocol,
            getChildOrdersProtocol,
            getExecutionsProtocol,
            getPositionsProtocol,
            getTradingCommissionProtocol,
            sendChildOrderProtocol,
            cancelChildOrderProtocol,
            cancelAllChildOrdersProtocol);

        var getBalance = new GetBalanceNativeEndpoint(getBalanceProtocol);
        var getCollateral = new GetCollateralNativeEndpoint(getCollateralProtocol);
        var getCollateralAccounts = new GetCollateralAccountsNativeEndpoint(getCollateralAccountsProtocol);
        var getCollateralHistory = new GetCollateralHistoryNativeEndpoint(getCollateralHistoryProtocol);
        var getChildOrders = new GetChildOrdersNativeEndpoint(getChildOrdersProtocol);
        var getExecutions = new GetExecutionsNativeEndpoint(getExecutionsProtocol);
        var getPositions = new GetPositionsNativeEndpoint(getPositionsProtocol);
        var getTradingCommission = new GetTradingCommissionNativeEndpoint(getTradingCommissionProtocol);
        var sendChildOrder = new SendChildOrderNativeEndpoint(sendChildOrderProtocol);
        var cancelChildOrder = new CancelChildOrderNativeEndpoint(cancelChildOrderProtocol);
        var cancelAllChildOrders = new CancelAllChildOrdersNativeEndpoint(cancelAllChildOrdersProtocol);
        var privateApi = new BitflyerPrivateNativeApi(
            getBalance,
            getCollateral,
            getCollateralAccounts,
            getCollateralHistory,
            getChildOrders,
            getExecutions,
            getPositions,
            getTradingCommission,
            sendChildOrder,
            cancelChildOrder,
            cancelAllChildOrders);

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
