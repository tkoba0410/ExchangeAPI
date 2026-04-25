using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Api;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.CancelAllChildOrders;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.CancelChildOrder;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.CancelParentOrder;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetAddresses;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetBalance;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetBalanceHistory;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetBankAccounts;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetChildOrders;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetCoinIns;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetCoinOuts;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetCollateral;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetCollateralAccounts;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetCollateralHistory;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetDeposits;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetExecutions;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetParentOrder;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetParentOrders;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetPermissions;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetPositions;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetTradingCommission;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetWithdrawals;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.SendChildOrder;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.SendParentOrder;
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
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Protocol.Tests;

public sealed class ProtocolFacadeTests
{
    [Fact]
    public async Task PublicFacade_ForwardsCalls()
    {
        var getMarkets = TestCall();
        var getBoard = TestCall();
        var getBoardState = TestCall();
        var getHealth = TestCall();
        var getFundingRate = TestCall();
        var getCorporateLeverage = TestCall();
        var getChats = TestCall();
        var getExecutions = TestCall();
        var getTicker = TestCall();
        var api = new BitflyerPublicProtocolApi(
            new FakeGetMarketsProtocolEndpoint(getMarkets),
            new FakeGetBoardProtocolEndpoint(getBoard),
            new FakeGetBoardStateProtocolEndpoint(getBoardState),
            new FakeGetHealthProtocolEndpoint(getHealth),
            new FakeGetFundingRateProtocolEndpoint(getFundingRate),
            new FakeGetCorporateLeverageProtocolEndpoint(getCorporateLeverage),
            new FakeGetChatsProtocolEndpoint(getChats),
            new FakeGetExecutionsPublicProtocolEndpoint(getExecutions),
            new FakeGetTickerProtocolEndpoint(getTicker));

        var actualMarkets = await api.GetMarketsAsync();
        var actualBoard = await api.GetBoardAsync("BTC_JPY");
        var actualBoardState = await api.GetBoardStateAsync("BTC_JPY");
        var actualHealth = await api.GetHealthAsync("BTC_JPY");
        var actualFundingRate = await api.GetFundingRateAsync("FX_BTC_JPY");
        var actualCorporateLeverage = await api.GetCorporateLeverageAsync();
        var actualChats = await api.GetChatsAsync(null);
        var actualExecutions = await api.GetExecutionsAsync("BTC_JPY", 10, null, null);
        var actualTicker = await api.GetTickerAsync("BTC_JPY");

        Assert.Same(getMarkets, actualMarkets);
        Assert.Same(getBoard, actualBoard);
        Assert.Same(getBoardState, actualBoardState);
        Assert.Same(getHealth, actualHealth);
        Assert.Same(getFundingRate, actualFundingRate);
        Assert.Same(getCorporateLeverage, actualCorporateLeverage);
        Assert.Same(getChats, actualChats);
        Assert.Same(getExecutions, actualExecutions);
        Assert.Same(getTicker, actualTicker);
    }

    [Fact]
    public async Task PrivateFacade_ForwardsCalls()
    {
        var getPermissions = TestCall();
        var getAddresses = TestCall();
        var getCoinIns = TestCall();
        var getCoinOuts = TestCall();
        var getBankAccounts = TestCall();
        var getDeposits = TestCall();
        var withdraw = TestCall();
        var getWithdrawals = TestCall();
        var getBalance = TestCall();
        var getParentOrders = TestCall();
        var getParentOrder = TestCall();
        var getCollateral = TestCall();
        var getCollateralAccounts = TestCall();
        var getCollateralHistory = TestCall();
        var getChildOrders = TestCall();
        var getExecutions = TestCall();
        var getBalanceHistory = TestCall();
        var getPositions = TestCall();
        var getTradingCommission = TestCall();
        var sendChildOrder = TestCall();
        var sendParentOrder = TestCall();
        var cancelChildOrder = TestCall();
        var cancelAllChildOrders = TestCall();
        var cancelParentOrder = TestCall();
        var api = new BitflyerPrivateProtocolApi(
            new FakeGetPermissionsProtocolEndpoint(getPermissions),
            new FakeGetAddressesProtocolEndpoint(getAddresses),
            new FakeGetCoinInsProtocolEndpoint(getCoinIns),
            new FakeGetCoinOutsProtocolEndpoint(getCoinOuts),
            new FakeGetBankAccountsProtocolEndpoint(getBankAccounts),
            new FakeGetDepositsProtocolEndpoint(getDeposits),
            new FakeWithdrawProtocolEndpoint(withdraw),
            new FakeGetWithdrawalsProtocolEndpoint(getWithdrawals),
            new FakeGetBalanceProtocolEndpoint(getBalance),
            new FakeGetParentOrdersProtocolEndpoint(getParentOrders),
            new FakeGetParentOrderProtocolEndpoint(getParentOrder),
            new FakeGetCollateralProtocolEndpoint(getCollateral),
            new FakeGetCollateralAccountsProtocolEndpoint(getCollateralAccounts),
            new FakeGetCollateralHistoryProtocolEndpoint(getCollateralHistory),
            new FakeGetChildOrdersProtocolEndpoint(getChildOrders),
            new FakeGetExecutionsProtocolEndpoint(getExecutions),
            new FakeGetBalanceHistoryProtocolEndpoint(getBalanceHistory),
            new FakeGetPositionsProtocolEndpoint(getPositions),
            new FakeGetTradingCommissionProtocolEndpoint(getTradingCommission),
            new FakeSendChildOrderProtocolEndpoint(sendChildOrder),
            new FakeSendParentOrderProtocolEndpoint(sendParentOrder),
            new FakeCancelChildOrderProtocolEndpoint(cancelChildOrder),
            new FakeCancelAllChildOrdersProtocolEndpoint(cancelAllChildOrders),
            new FakeCancelParentOrderProtocolEndpoint(cancelParentOrder));

        Assert.Same(getPermissions, await api.GetPermissionsAsync());
        Assert.Same(getAddresses, await api.GetAddressesAsync());
        Assert.Same(getCoinIns, await api.GetCoinInsAsync(10, null, null));
        Assert.Same(getCoinOuts, await api.GetCoinOutsAsync(10, null, null));
        Assert.Same(getBankAccounts, await api.GetBankAccountsAsync());
        Assert.Same(getDeposits, await api.GetDepositsAsync(10, null, null));
        Assert.Same(withdraw, await api.WithdrawAsync("{}"));
        Assert.Same(getWithdrawals, await api.GetWithdrawalsAsync(10, null, null, "MID1"));
        Assert.Same(getBalance, await api.GetBalanceAsync());
        Assert.Same(getParentOrders, await api.GetParentOrdersAsync("BTC_JPY", 10, null, null, "ACTIVE"));
        Assert.Same(getParentOrder, await api.GetParentOrderAsync("JCO1", null));
        Assert.Same(getCollateral, await api.GetCollateralAsync());
        Assert.Same(getCollateralAccounts, await api.GetCollateralAccountsAsync());
        Assert.Same(getCollateralHistory, await api.GetCollateralHistoryAsync(10, null, null));
        Assert.Same(getChildOrders, await api.GetChildOrdersAsync("BTC_JPY", 10, null, null, "COMPLETED", null, null, null));
        Assert.Same(getExecutions, await api.GetExecutionsAsync("BTC_JPY", 10, null, null, null, null));
        Assert.Same(getBalanceHistory, await api.GetBalanceHistoryAsync("JPY", 10, null, null));
        Assert.Same(getPositions, await api.GetPositionsAsync("FX_BTC_JPY"));
        Assert.Same(getTradingCommission, await api.GetTradingCommissionAsync("BTC_JPY"));
        Assert.Same(sendChildOrder, await api.SendChildOrderAsync("{}"));
        Assert.Same(sendParentOrder, await api.SendParentOrderAsync("{}"));
        Assert.Same(cancelChildOrder, await api.CancelChildOrderAsync("{}"));
        Assert.Same(cancelAllChildOrders, await api.CancelAllChildOrdersAsync("{}"));
        Assert.Same(cancelParentOrder, await api.CancelParentOrderAsync("{}"));
    }

    private static CallResult<ProtocolRequest, ProtocolResponse> TestCall()
    {
        return CallFactory.Success(
            new ProtocolRequest { EndpointId = "E", Method = "GET", Path = "/", Query = null, BodyText = null },
            new ProtocolResponse { StatusCode = 200, Headers = new Dictionary<string, string[]>(), BodyText = "{}" },
            new CallMeta { Layer = CallLayers.Protocol, Component = CallComponents.PublicEndpointModule, EndpointId = "E", Scope = "Public", Auth = "None" });
    }

    private sealed class FakeGetMarketsProtocolEndpoint : IGetMarketsProtocolEndpoint
    {
        private readonly CallResult<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetMarketsProtocolEndpoint(CallResult<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetBoardProtocolEndpoint : IGetBoardProtocolEndpoint
    {
        private readonly CallResult<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetBoardProtocolEndpoint(CallResult<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(string? productCode, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetExecutionsPublicProtocolEndpoint : IGetExecutionsPublicProtocolEndpoint
    {
        private readonly CallResult<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetExecutionsPublicProtocolEndpoint(CallResult<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(string? productCode, int? count, long? before, long? after, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetBoardStateProtocolEndpoint : IGetBoardStateProtocolEndpoint
    {
        private readonly CallResult<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetBoardStateProtocolEndpoint(CallResult<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(string? productCode, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetHealthProtocolEndpoint : IGetHealthProtocolEndpoint
    {
        private readonly CallResult<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetHealthProtocolEndpoint(CallResult<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(string? productCode, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetFundingRateProtocolEndpoint : IGetFundingRateProtocolEndpoint
    {
        private readonly CallResult<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetFundingRateProtocolEndpoint(CallResult<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(string productCode, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetCorporateLeverageProtocolEndpoint : IGetCorporateLeverageProtocolEndpoint
    {
        private readonly CallResult<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetCorporateLeverageProtocolEndpoint(CallResult<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetChatsProtocolEndpoint : IGetChatsProtocolEndpoint
    {
        private readonly CallResult<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetChatsProtocolEndpoint(CallResult<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(string? fromDate, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetTickerProtocolEndpoint : IGetTickerProtocolEndpoint
    {
        private readonly CallResult<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetTickerProtocolEndpoint(CallResult<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(string? productCode, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetBalanceProtocolEndpoint : IGetBalanceProtocolEndpoint
    {
        private readonly CallResult<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetBalanceProtocolEndpoint(CallResult<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetPermissionsProtocolEndpoint : IGetPermissionsProtocolEndpoint
    {
        private readonly CallResult<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetPermissionsProtocolEndpoint(CallResult<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetAddressesProtocolEndpoint : IGetAddressesProtocolEndpoint
    {
        private readonly CallResult<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetAddressesProtocolEndpoint(CallResult<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetCoinInsProtocolEndpoint : IGetCoinInsProtocolEndpoint
    {
        private readonly CallResult<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetCoinInsProtocolEndpoint(CallResult<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(int? count, long? before, long? after, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetCoinOutsProtocolEndpoint : IGetCoinOutsProtocolEndpoint
    {
        private readonly CallResult<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetCoinOutsProtocolEndpoint(CallResult<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(int? count, long? before, long? after, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetBankAccountsProtocolEndpoint : IGetBankAccountsProtocolEndpoint
    {
        private readonly CallResult<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetBankAccountsProtocolEndpoint(CallResult<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetDepositsProtocolEndpoint : IGetDepositsProtocolEndpoint
    {
        private readonly CallResult<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetDepositsProtocolEndpoint(CallResult<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(int? count, long? before, long? after, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeWithdrawProtocolEndpoint : IWithdrawProtocolEndpoint
    {
        private readonly CallResult<ProtocolRequest, ProtocolResponse> _call;
        public FakeWithdrawProtocolEndpoint(CallResult<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(string bodyJson, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetWithdrawalsProtocolEndpoint : IGetWithdrawalsProtocolEndpoint
    {
        private readonly CallResult<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetWithdrawalsProtocolEndpoint(CallResult<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(int? count, long? before, long? after, string? messageId, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetCollateralProtocolEndpoint : IGetCollateralProtocolEndpoint
    {
        private readonly CallResult<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetCollateralProtocolEndpoint(CallResult<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetParentOrdersProtocolEndpoint : IGetParentOrdersProtocolEndpoint
    {
        private readonly CallResult<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetParentOrdersProtocolEndpoint(CallResult<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(string? productCode, int? count, long? before, long? after, string? parentOrderState, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetParentOrderProtocolEndpoint : IGetParentOrderProtocolEndpoint
    {
        private readonly CallResult<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetParentOrderProtocolEndpoint(CallResult<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(string? parentOrderId, string? parentOrderAcceptanceId, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetCollateralAccountsProtocolEndpoint : IGetCollateralAccountsProtocolEndpoint
    {
        private readonly CallResult<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetCollateralAccountsProtocolEndpoint(CallResult<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetCollateralHistoryProtocolEndpoint : IGetCollateralHistoryProtocolEndpoint
    {
        private readonly CallResult<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetCollateralHistoryProtocolEndpoint(CallResult<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(int? count, long? before, long? after, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetChildOrdersProtocolEndpoint : IGetChildOrdersProtocolEndpoint
    {
        private readonly CallResult<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetChildOrdersProtocolEndpoint(CallResult<ProtocolRequest, ProtocolResponse> call) => _call = call;

        public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(
            string? productCode,
            int? count,
            long? before,
            long? after,
            string? childOrderState,
            string? childOrderId,
            string? childOrderAcceptanceId,
            string? parentOrderId,
            CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetExecutionsProtocolEndpoint : IGetExecutionsProtocolEndpoint
    {
        private readonly CallResult<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetExecutionsProtocolEndpoint(CallResult<ProtocolRequest, ProtocolResponse> call) => _call = call;

        public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(
            string productCode,
            int? count,
            long? before,
            long? after,
            string? childOrderId,
            string? childOrderAcceptanceId,
            CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetBalanceHistoryProtocolEndpoint : IGetBalanceHistoryProtocolEndpoint
    {
        private readonly CallResult<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetBalanceHistoryProtocolEndpoint(CallResult<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(string? currencyCode, int? count, long? before, long? after, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetPositionsProtocolEndpoint : IGetPositionsProtocolEndpoint
    {
        private readonly CallResult<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetPositionsProtocolEndpoint(CallResult<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(string productCode, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetTradingCommissionProtocolEndpoint : IGetTradingCommissionProtocolEndpoint
    {
        private readonly CallResult<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetTradingCommissionProtocolEndpoint(CallResult<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(string productCode, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeSendChildOrderProtocolEndpoint : ISendChildOrderProtocolEndpoint
    {
        private readonly CallResult<ProtocolRequest, ProtocolResponse> _call;
        public FakeSendChildOrderProtocolEndpoint(CallResult<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(string bodyJson, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeSendParentOrderProtocolEndpoint : ISendParentOrderProtocolEndpoint
    {
        private readonly CallResult<ProtocolRequest, ProtocolResponse> _call;
        public FakeSendParentOrderProtocolEndpoint(CallResult<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(string bodyJson, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeCancelChildOrderProtocolEndpoint : ICancelChildOrderProtocolEndpoint
    {
        private readonly CallResult<ProtocolRequest, ProtocolResponse> _call;
        public FakeCancelChildOrderProtocolEndpoint(CallResult<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(string bodyJson, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeCancelAllChildOrdersProtocolEndpoint : ICancelAllChildOrdersProtocolEndpoint
    {
        private readonly CallResult<ProtocolRequest, ProtocolResponse> _call;
        public FakeCancelAllChildOrdersProtocolEndpoint(CallResult<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(string bodyJson, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeCancelParentOrderProtocolEndpoint : ICancelParentOrderProtocolEndpoint
    {
        private readonly CallResult<ProtocolRequest, ProtocolResponse> _call;
        public FakeCancelParentOrderProtocolEndpoint(CallResult<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(string bodyJson, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }
}
