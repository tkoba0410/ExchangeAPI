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

        var actualMarkets = await api.GetMarketsCallAsync();
        var actualBoard = await api.GetBoardCallAsync("BTC_JPY");
        var actualBoardState = await api.GetBoardStateCallAsync("BTC_JPY");
        var actualHealth = await api.GetHealthCallAsync("BTC_JPY");
        var actualFundingRate = await api.GetFundingRateCallAsync("FX_BTC_JPY");
        var actualCorporateLeverage = await api.GetCorporateLeverageCallAsync();
        var actualChats = await api.GetChatsCallAsync(null);
        var actualExecutions = await api.GetExecutionsCallAsync("BTC_JPY", 10, null, null);
        var actualTicker = await api.GetTickerCallAsync("BTC_JPY");

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

        Assert.Same(getPermissions, await api.GetPermissionsCallAsync());
        Assert.Same(getAddresses, await api.GetAddressesCallAsync());
        Assert.Same(getCoinIns, await api.GetCoinInsCallAsync(10, null, null));
        Assert.Same(getCoinOuts, await api.GetCoinOutsCallAsync(10, null, null));
        Assert.Same(getBankAccounts, await api.GetBankAccountsCallAsync());
        Assert.Same(getDeposits, await api.GetDepositsCallAsync(10, null, null));
        Assert.Same(withdraw, await api.WithdrawCallAsync("{}"));
        Assert.Same(getWithdrawals, await api.GetWithdrawalsCallAsync(10, null, null, "MID1"));
        Assert.Same(getBalance, await api.GetBalanceCallAsync());
        Assert.Same(getParentOrders, await api.GetParentOrdersCallAsync("BTC_JPY", 10, null, null, "ACTIVE"));
        Assert.Same(getParentOrder, await api.GetParentOrderCallAsync("JCO1", null));
        Assert.Same(getCollateral, await api.GetCollateralCallAsync());
        Assert.Same(getCollateralAccounts, await api.GetCollateralAccountsCallAsync());
        Assert.Same(getCollateralHistory, await api.GetCollateralHistoryCallAsync(10, null, null));
        Assert.Same(getChildOrders, await api.GetChildOrdersCallAsync("BTC_JPY", 10, null, null, "COMPLETED", null, null, null));
        Assert.Same(getExecutions, await api.GetExecutionsCallAsync("BTC_JPY", 10, null, null, null, null));
        Assert.Same(getBalanceHistory, await api.GetBalanceHistoryCallAsync("JPY", 10, null, null));
        Assert.Same(getPositions, await api.GetPositionsCallAsync("FX_BTC_JPY"));
        Assert.Same(getTradingCommission, await api.GetTradingCommissionCallAsync("BTC_JPY"));
        Assert.Same(sendChildOrder, await api.SendChildOrderCallAsync("{}"));
        Assert.Same(sendParentOrder, await api.SendParentOrderCallAsync("{}"));
        Assert.Same(cancelChildOrder, await api.CancelChildOrderCallAsync("{}"));
        Assert.Same(cancelAllChildOrders, await api.CancelAllChildOrdersCallAsync("{}"));
        Assert.Same(cancelParentOrder, await api.CancelParentOrderCallAsync("{}"));
    }

    private static Call<ProtocolRequest, ProtocolResponse> TestCall()
    {
        return CallFactory.Success(
            new ProtocolRequest { EndpointId = "E", Method = "GET", Path = "/", Query = null, BodyText = null },
            new ProtocolResponse { StatusCode = 200, Headers = new Dictionary<string, string[]>(), BodyText = "{}" },
            new CallMeta { Layer = CallLayers.Protocol, Component = CallComponents.PublicEndpointModule, EndpointId = "E", Scope = "Public", Auth = "None" });
    }

    private sealed class FakeGetMarketsProtocolEndpoint : IGetMarketsProtocolEndpoint
    {
        private readonly Call<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetMarketsProtocolEndpoint(Call<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetBoardProtocolEndpoint : IGetBoardProtocolEndpoint
    {
        private readonly Call<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetBoardProtocolEndpoint(Call<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(string? productCode, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetExecutionsPublicProtocolEndpoint : IGetExecutionsPublicProtocolEndpoint
    {
        private readonly Call<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetExecutionsPublicProtocolEndpoint(Call<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(string? productCode, int? count, long? before, long? after, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetBoardStateProtocolEndpoint : IGetBoardStateProtocolEndpoint
    {
        private readonly Call<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetBoardStateProtocolEndpoint(Call<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(string? productCode, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetHealthProtocolEndpoint : IGetHealthProtocolEndpoint
    {
        private readonly Call<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetHealthProtocolEndpoint(Call<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(string? productCode, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetFundingRateProtocolEndpoint : IGetFundingRateProtocolEndpoint
    {
        private readonly Call<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetFundingRateProtocolEndpoint(Call<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(string productCode, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetCorporateLeverageProtocolEndpoint : IGetCorporateLeverageProtocolEndpoint
    {
        private readonly Call<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetCorporateLeverageProtocolEndpoint(Call<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetChatsProtocolEndpoint : IGetChatsProtocolEndpoint
    {
        private readonly Call<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetChatsProtocolEndpoint(Call<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(string? fromDate, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetTickerProtocolEndpoint : IGetTickerProtocolEndpoint
    {
        private readonly Call<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetTickerProtocolEndpoint(Call<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(string? productCode, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetBalanceProtocolEndpoint : IGetBalanceProtocolEndpoint
    {
        private readonly Call<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetBalanceProtocolEndpoint(Call<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetPermissionsProtocolEndpoint : IGetPermissionsProtocolEndpoint
    {
        private readonly Call<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetPermissionsProtocolEndpoint(Call<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetAddressesProtocolEndpoint : IGetAddressesProtocolEndpoint
    {
        private readonly Call<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetAddressesProtocolEndpoint(Call<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetCoinInsProtocolEndpoint : IGetCoinInsProtocolEndpoint
    {
        private readonly Call<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetCoinInsProtocolEndpoint(Call<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(int? count, long? before, long? after, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetCoinOutsProtocolEndpoint : IGetCoinOutsProtocolEndpoint
    {
        private readonly Call<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetCoinOutsProtocolEndpoint(Call<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(int? count, long? before, long? after, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetBankAccountsProtocolEndpoint : IGetBankAccountsProtocolEndpoint
    {
        private readonly Call<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetBankAccountsProtocolEndpoint(Call<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetDepositsProtocolEndpoint : IGetDepositsProtocolEndpoint
    {
        private readonly Call<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetDepositsProtocolEndpoint(Call<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(int? count, long? before, long? after, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeWithdrawProtocolEndpoint : IWithdrawProtocolEndpoint
    {
        private readonly Call<ProtocolRequest, ProtocolResponse> _call;
        public FakeWithdrawProtocolEndpoint(Call<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(string bodyJson, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetWithdrawalsProtocolEndpoint : IGetWithdrawalsProtocolEndpoint
    {
        private readonly Call<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetWithdrawalsProtocolEndpoint(Call<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(int? count, long? before, long? after, string? messageId, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetCollateralProtocolEndpoint : IGetCollateralProtocolEndpoint
    {
        private readonly Call<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetCollateralProtocolEndpoint(Call<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetParentOrdersProtocolEndpoint : IGetParentOrdersProtocolEndpoint
    {
        private readonly Call<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetParentOrdersProtocolEndpoint(Call<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(string? productCode, int? count, long? before, long? after, string? parentOrderState, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetParentOrderProtocolEndpoint : IGetParentOrderProtocolEndpoint
    {
        private readonly Call<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetParentOrderProtocolEndpoint(Call<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(string? parentOrderId, string? parentOrderAcceptanceId, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetCollateralAccountsProtocolEndpoint : IGetCollateralAccountsProtocolEndpoint
    {
        private readonly Call<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetCollateralAccountsProtocolEndpoint(Call<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetCollateralHistoryProtocolEndpoint : IGetCollateralHistoryProtocolEndpoint
    {
        private readonly Call<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetCollateralHistoryProtocolEndpoint(Call<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(int? count, long? before, long? after, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetChildOrdersProtocolEndpoint : IGetChildOrdersProtocolEndpoint
    {
        private readonly Call<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetChildOrdersProtocolEndpoint(Call<ProtocolRequest, ProtocolResponse> call) => _call = call;

        public Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(
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
        private readonly Call<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetExecutionsProtocolEndpoint(Call<ProtocolRequest, ProtocolResponse> call) => _call = call;

        public Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(
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
        private readonly Call<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetBalanceHistoryProtocolEndpoint(Call<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(string? currencyCode, int? count, long? before, long? after, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetPositionsProtocolEndpoint : IGetPositionsProtocolEndpoint
    {
        private readonly Call<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetPositionsProtocolEndpoint(Call<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(string productCode, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetTradingCommissionProtocolEndpoint : IGetTradingCommissionProtocolEndpoint
    {
        private readonly Call<ProtocolRequest, ProtocolResponse> _call;
        public FakeGetTradingCommissionProtocolEndpoint(Call<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(string productCode, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeSendChildOrderProtocolEndpoint : ISendChildOrderProtocolEndpoint
    {
        private readonly Call<ProtocolRequest, ProtocolResponse> _call;
        public FakeSendChildOrderProtocolEndpoint(Call<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(string bodyJson, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeSendParentOrderProtocolEndpoint : ISendParentOrderProtocolEndpoint
    {
        private readonly Call<ProtocolRequest, ProtocolResponse> _call;
        public FakeSendParentOrderProtocolEndpoint(Call<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(string bodyJson, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeCancelChildOrderProtocolEndpoint : ICancelChildOrderProtocolEndpoint
    {
        private readonly Call<ProtocolRequest, ProtocolResponse> _call;
        public FakeCancelChildOrderProtocolEndpoint(Call<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(string bodyJson, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeCancelAllChildOrdersProtocolEndpoint : ICancelAllChildOrdersProtocolEndpoint
    {
        private readonly Call<ProtocolRequest, ProtocolResponse> _call;
        public FakeCancelAllChildOrdersProtocolEndpoint(Call<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(string bodyJson, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeCancelParentOrderProtocolEndpoint : ICancelParentOrderProtocolEndpoint
    {
        private readonly Call<ProtocolRequest, ProtocolResponse> _call;
        public FakeCancelParentOrderProtocolEndpoint(Call<ProtocolRequest, ProtocolResponse> call) => _call = call;
        public Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(string bodyJson, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }
}
