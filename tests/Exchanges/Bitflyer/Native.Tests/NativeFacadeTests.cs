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
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Units;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Native.Tests;

public sealed class NativeFacadeTests
{
    [Fact]
    public async Task PublicFacade_ForwardsCalls()
    {
        var getMarkets = CallFactory.Success(
            new GetMarketsRequest(),
            (IReadOnlyList<GetMarkets.Item>)[new GetMarkets.Item { ProductCode = "BTC_JPY", MarketType = "Spot" }],
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PublicEndpointModule, EndpointId = "GetMarkets", Scope = "Public", Auth = "None" });
        var getBoard = CallFactory.Success(
            new GetBoardRequest { ProductCode = null },
            new GetBoardResponse
            {
                MidPrice = 1m,
                Bids = [new GetBoardLevel { Price = 1m, Size = 1m }],
                Asks = [new GetBoardLevel { Price = 2m, Size = 1m }],
            },
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PublicEndpointModule, EndpointId = "GetBoard", Scope = "Public", Auth = "None" });
        var getBoardState = CallFactory.Success(
            new GetBoardStateRequest { ProductCode = "BTC_JPY" },
            new GetBoardStateResponse
            {
                Health = "NORMAL",
                State = "RUNNING",
                Data = new GetBoardStateData { SpecialQuotation = 1m },
            },
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PublicEndpointModule, EndpointId = "GetBoardState", Scope = "Public", Auth = "None" });
        var getHealth = CallFactory.Success(
            new GetHealthRequest { ProductCode = "BTC_JPY" },
            new GetHealthResponse { Status = "NORMAL" },
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PublicEndpointModule, EndpointId = "GetHealth", Scope = "Public", Auth = "None" });
        var getFundingRate = CallFactory.Success(
            new GetFundingRateRequest { ProductCode = "FX_BTC_JPY" },
            new GetFundingRateResponse
            {
                CurrentFundingRate = 0.001m,
                NextFundingRateSettleDate = DateTimeOffset.UnixEpoch,
            },
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PublicEndpointModule, EndpointId = "GetFundingRate", Scope = "Public", Auth = "None" });
        var getCorporateLeverage = CallFactory.Success(
            new GetCorporateLeverageRequest(),
            new GetCorporateLeverageResponse
            {
                CurrentMax = 2m,
                CurrentStartDate = DateTimeOffset.UnixEpoch,
                NextMax = 4m,
                NextStartDate = DateTimeOffset.UnixEpoch.AddDays(1),
            },
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PublicEndpointModule, EndpointId = "GetCorporateLeverage", Scope = "Public", Auth = "None" });
        var getChats = CallFactory.Success(
            new GetChatsRequest(),
            (IReadOnlyList<GetChats.Item>)[new GetChats.Item
            {
                Nickname = "n",
                Message = "m",
                Date = DateTimeOffset.UnixEpoch,
            }],
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PublicEndpointModule, EndpointId = "GetChats", Scope = "Public", Auth = "None" });
        var getExecutions = CallFactory.Success(
            new GetExecutionsPublicRequest { ProductCode = "BTC_JPY", Count = 10 },
            (IReadOnlyList<GetExecutionsPublic.Item>)[new GetExecutionsPublic.Item
            {
                Id = 1,
                Side = "BUY",
                Price = 1m,
                Size = 1m,
                ExecDate = DateTimeOffset.UnixEpoch,
                BuyChildOrderAcceptanceId = "JRF-BUY",
                SellChildOrderAcceptanceId = "JRF-SELL",
            }],
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PublicEndpointModule, EndpointId = "GetExecutionsPublic", Scope = "Public", Auth = "None" });
        var expected = CallFactory.Success(
            new GetTickerRequest { ProductCode = null },
            new GetTickerResponse
            {
                ProductCode = "BTC_JPY",
                State = "RUNNING",
                Timestamp = DateTimeOffset.UtcNow,
                TickId = 1,
                BestBid = 1,
                BestAsk = 1,
                BestBidSize = 1,
                BestAskSize = 1,
                TotalBidDepth = 1,
                TotalAskDepth = 1,
                MarketBidSize = 0,
                MarketAskSize = 0,
                Ltp = 1,
                Volume = 1,
                VolumeByProduct = 1,
            },
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PublicEndpointModule, EndpointId = "GetTicker", Scope = "Public", Auth = "None" });
        var api = new BitflyerPublicNativeApi(
            new FakeGetMarketsNativeEndpoint(getMarkets),
            new FakeGetBoardNativeEndpoint(getBoard),
            new FakeGetBoardStateNativeEndpoint(getBoardState),
            new FakeGetHealthNativeEndpoint(getHealth),
            new FakeGetFundingRateNativeEndpoint(getFundingRate),
            new FakeGetCorporateLeverageNativeEndpoint(getCorporateLeverage),
            new FakeGetChatsNativeEndpoint(getChats),
            new FakeGetExecutionsPublicNativeEndpoint(getExecutions),
            new FakeGetTickerNativeEndpoint(expected));

        var actualMarkets = await api.GetMarketsCallAsync(new GetMarketsRequest());
        var actualBoard = await api.GetBoardCallAsync(new GetBoardRequest { ProductCode = null });
        var actualBoardState = await api.GetBoardStateCallAsync(new GetBoardStateRequest { ProductCode = "BTC_JPY" });
        var actualHealth = await api.GetHealthCallAsync(new GetHealthRequest { ProductCode = "BTC_JPY" });
        var actualFundingRate = await api.GetFundingRateCallAsync(new GetFundingRateRequest { ProductCode = "FX_BTC_JPY" });
        var actualCorporateLeverage = await api.GetCorporateLeverageCallAsync(new GetCorporateLeverageRequest());
        var actualChats = await api.GetChatsCallAsync(new GetChatsRequest());
        var actualExecutions = await api.GetExecutionsCallAsync(new GetExecutionsPublicRequest { ProductCode = "BTC_JPY", Count = 10 });
        var actualTicker = await api.GetTickerCallAsync(new GetTickerRequest { ProductCode = null });

        Assert.Same(getMarkets, actualMarkets);
        Assert.Same(getBoard, actualBoard);
        Assert.Same(getBoardState, actualBoardState);
        Assert.Same(getHealth, actualHealth);
        Assert.Same(getFundingRate, actualFundingRate);
        Assert.Same(getCorporateLeverage, actualCorporateLeverage);
        Assert.Same(getChats, actualChats);
        Assert.Same(getExecutions, actualExecutions);
        Assert.Same(expected, actualTicker);
    }

    [Fact]
    public async Task PrivateFacade_ForwardsCalls()
    {
        var getPermissions = CallFactory.Success(
            new GetPermissionsRequest(),
            (IReadOnlyList<string>)["/v1/me/getbalance"],
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PrivateEndpointModule, EndpointId = "GetPermissions", Scope = "Private", Auth = "KeySecret" });
        var getAddresses = CallFactory.Success(
            new GetAddressesRequest(),
            (IReadOnlyList<GetAddresses.Item>)[new GetAddresses.Item { Type = "NORMAL", CurrencyCode = "BTC", Address = "addr" }],
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PrivateEndpointModule, EndpointId = "GetAddresses", Scope = "Private", Auth = "KeySecret" });
        var getCoinIns = CallFactory.Success(
            new GetCoinInsRequest { Count = 10 },
            (IReadOnlyList<GetCoinIns.Item>)[new GetCoinIns.Item { Id = 1, OrderId = "CDP1", CurrencyCode = "BTC", Amount = 1m, Address = "addr", TxHash = "hash", Status = "COMPLETED", EventDate = DateTimeOffset.UnixEpoch }],
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PrivateEndpointModule, EndpointId = "GetCoinIns", Scope = "Private", Auth = "KeySecret" });
        var getCoinOuts = CallFactory.Success(
            new GetCoinOutsRequest { Count = 10 },
            (IReadOnlyList<GetCoinOuts.Item>)[new GetCoinOuts.Item { Id = 1, OrderId = "CWD1", CurrencyCode = "BTC", Amount = 1m, Address = "addr", TxHash = "hash", Fee = 0.1m, AdditionalFee = 0.01m, Status = "COMPLETED", EventDate = DateTimeOffset.UnixEpoch }],
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PrivateEndpointModule, EndpointId = "GetCoinOuts", Scope = "Private", Auth = "KeySecret" });
        var getBankAccounts = CallFactory.Success(
            new GetBankAccountsRequest(),
            (IReadOnlyList<GetBankAccounts.Item>)[new GetBankAccounts.Item { Id = 1, IsVerified = true, BankName = "bank", BranchName = "branch", AccountType = "普通", AccountNumber = "123", AccountName = "name" }],
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PrivateEndpointModule, EndpointId = "GetBankAccounts", Scope = "Private", Auth = "KeySecret" });
        var getDeposits = CallFactory.Success(
            new GetDepositsRequest { Count = 10 },
            (IReadOnlyList<GetDeposits.Item>)[new GetDeposits.Item { Id = 1, OrderId = "MDP1", CurrencyCode = "JPY", Amount = 10m, Status = "COMPLETED", EventDate = DateTimeOffset.UnixEpoch }],
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PrivateEndpointModule, EndpointId = "GetDeposits", Scope = "Private", Auth = "KeySecret" });
        var withdraw = CallFactory.Success(
            new WithdrawRequest { CurrencyCode = "JPY", BankAccountId = 1, Amount = 10m, Code = "123456" },
            new WithdrawResponse { MessageId = "MID1" },
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PrivateEndpointModule, EndpointId = "Withdraw", Scope = "Private", Auth = "KeySecret" });
        var getWithdrawals = CallFactory.Success(
            new GetWithdrawalsRequest { Count = 10, MessageId = "MID1" },
            (IReadOnlyList<GetWithdrawals.Item>)[new GetWithdrawals.Item { Id = 1, OrderId = "MWD1", CurrencyCode = "JPY", Amount = 10m, Status = "COMPLETED", EventDate = DateTimeOffset.UnixEpoch }],
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PrivateEndpointModule, EndpointId = "GetWithdrawals", Scope = "Private", Auth = "KeySecret" });
        var getBalance = CallFactory.Success(
            new GetBalanceRequest(),
            (IReadOnlyList<GetBalance.Item>)[new GetBalance.Item { CurrencyCode = "JPY", Amount = 1m, Available = 1m }],
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PrivateEndpointModule, EndpointId = "GetBalance", Scope = "Private", Auth = "KeySecret" });
        var getParentOrders = CallFactory.Success(
            new GetParentOrdersRequest { ProductCode = "BTC_JPY", Count = 10 },
            (IReadOnlyList<GetParentOrders.Item>)[new GetParentOrders.Item { Id = 1, ParentOrderId = "JCO1", ProductCode = "BTC_JPY", Side = "BUY", ParentOrderType = "SIMPLE", Price = 1m, AveragePrice = 1m, Size = 1m, ParentOrderState = "ACTIVE", ExpireDate = DateTimeOffset.UnixEpoch, ParentOrderDate = DateTimeOffset.UnixEpoch, ParentOrderAcceptanceId = "JRF1", OutstandingSize = 1m, CancelSize = 0m, ExecutedSize = 0m, TotalCommission = 0m }],
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PrivateEndpointModule, EndpointId = "GetParentOrders", Scope = "Private", Auth = "KeySecret" });
        var getParentOrder = CallFactory.Success(
            new GetParentOrderRequest { ParentOrderId = "JCO1" },
            new GetParentOrderResponse { Id = 1, ParentOrderId = "JCO1", OrderMethod = "SIMPLE", ExpireDate = DateTimeOffset.UnixEpoch, TimeInForce = "GTC", Parameters = [new GetParentOrderParameter { ProductCode = "BTC_JPY", ConditionType = "LIMIT", Side = "BUY", Price = 1m, Size = 1m, TriggerPrice = 0m, Offset = 0 }], ParentOrderAcceptanceId = "JRF1" },
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PrivateEndpointModule, EndpointId = "GetParentOrder", Scope = "Private", Auth = "KeySecret" });
        var getCollateral = CallFactory.Success(
            new GetCollateralRequest(),
            new GetCollateralResponse
            {
                Collateral = 10m,
                OpenPositionPnl = -1m,
                RequireCollateral = 2m,
                KeepRate = 5m,
                MarginCallAmount = null,
                MarginCallDueDate = null,
            },
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PrivateEndpointModule, EndpointId = "GetCollateral", Scope = "Private", Auth = "KeySecret" });
        var getCollateralAccounts = CallFactory.Success(
            new GetCollateralAccountsRequest(),
            (IReadOnlyList<GetCollateralAccounts.Item>)[new GetCollateralAccounts.Item { CurrencyCode = "JPY", Amount = 10m }],
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PrivateEndpointModule, EndpointId = "GetCollateralAccounts", Scope = "Private", Auth = "KeySecret" });
        var getCollateralHistory = CallFactory.Success(
            new GetCollateralHistoryRequest { Count = 10 },
            (IReadOnlyList<GetCollateralHistory.Item>)[new GetCollateralHistory.Item
            {
                Id = 1,
                CurrencyCode = "JPY",
                Change = -1m,
                Amount = 10m,
                ReasonCode = "CLEARING_COLL",
                Date = DateTimeOffset.UnixEpoch,
            }],
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PrivateEndpointModule, EndpointId = "GetCollateralHistory", Scope = "Private", Auth = "KeySecret" });
        var getChildOrders = CallFactory.Success(
            new GetChildOrdersRequest { ProductCode = "BTC_JPY", Count = 10 },
            (IReadOnlyList<GetChildOrders.Item>)[new GetChildOrders.Item
            {
                Id = 1,
                ChildOrderId = "JOR1",
                ProductCode = "BTC_JPY",
                Side = "BUY",
                ChildOrderType = "LIMIT",
                Price = 1m,
                AveragePrice = 1m,
                Size = 1m,
                ChildOrderState = "COMPLETED",
                ExpireDate = DateTimeOffset.UnixEpoch,
                ChildOrderDate = DateTimeOffset.UnixEpoch,
                ChildOrderAcceptanceId = "JRF1",
                OutstandingSize = 0m,
                CancelSize = 0m,
                ExecutedSize = 1m,
                TotalCommission = 0m,
                TimeInForce = "GTC",
            }],
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PrivateEndpointModule, EndpointId = "GetChildOrders", Scope = "Private", Auth = "KeySecret" });
        var getExecutions = CallFactory.Success(
            new GetExecutionsRequest { ProductCode = "BTC_JPY", Count = 10 },
            (IReadOnlyList<GetExecutions.Item>)[new GetExecutions.Item
            {
                Id = 1,
                ChildOrderId = "JOR1",
                Side = "BUY",
                Price = 1m,
                Size = 1m,
                Commission = 0m,
                ExecDate = DateTimeOffset.UnixEpoch,
                ChildOrderAcceptanceId = "JRF1",
            }],
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PrivateEndpointModule, EndpointId = "GetExecutionsPrivate", Scope = "Private", Auth = "KeySecret" });
        var getBalanceHistory = CallFactory.Success(
            new GetBalanceHistoryRequest { CurrencyCode = "JPY", Count = 10 },
            (IReadOnlyList<GetBalanceHistory.Item>)[new GetBalanceHistory.Item { Id = 1, TradeDate = DateTimeOffset.UnixEpoch, EventDate = DateTimeOffset.UnixEpoch, ProductCode = null, CurrencyCode = "JPY", TradeType = "DEPOSIT", Price = 0m, Amount = 10m, Quantity = 0m, Commission = 0m, Balance = 10m, OrderId = "ORDER1" }],
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PrivateEndpointModule, EndpointId = "GetBalanceHistory", Scope = "Private", Auth = "KeySecret" });
        var getPositions = CallFactory.Success(
            new GetPositionsRequest { ProductCode = "FX_BTC_JPY" },
            (IReadOnlyList<GetPositions.Item>)[new GetPositions.Item
            {
                ProductCode = "FX_BTC_JPY",
                Side = "BUY",
                Price = 1m,
                Size = 1m,
                Commission = 0m,
                SwapPointAccumulate = 0m,
                RequireCollateral = 1m,
                OpenDate = DateTimeOffset.UnixEpoch,
                Leverage = 2m,
                Pnl = 0m,
                Sfd = 0m,
            }],
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PrivateEndpointModule, EndpointId = "GetPositions", Scope = "Private", Auth = "KeySecret" });
        var getTradingCommission = CallFactory.Success(
            new GetTradingCommissionRequest { ProductCode = "BTC_JPY" },
            new GetTradingCommissionResponse { CommissionRate = 0.001m },
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PrivateEndpointModule, EndpointId = "GetTradingCommission", Scope = "Private", Auth = "KeySecret" });
        var sendChildOrder = CallFactory.Success(
            new SendChildOrderRequest { ProductCode = "BTC_JPY", ChildOrderType = "MARKET", Side = "BUY", Size = 1m },
            new SendChildOrderResponse { ChildOrderAcceptanceId = "A" },
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PrivateEndpointModule, EndpointId = "SendChildOrder", Scope = "Private", Auth = "KeySecret" });
        var sendParentOrder = CallFactory.Success(
            new SendParentOrderRequest { Parameters = [new SendParentOrderParameter { ProductCode = "BTC_JPY", ConditionType = "LIMIT", Side = "BUY", Price = 1m, Size = 1m }] },
            new SendParentOrderResponse { ParentOrderAcceptanceId = "A" },
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PrivateEndpointModule, EndpointId = "SendParentOrder", Scope = "Private", Auth = "KeySecret" });
        var cancelChildOrder = CallFactory.Success(
            new CancelChildOrderRequest { ProductCode = "BTC_JPY", ChildOrderId = "X" },
            new Unit(),
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PrivateEndpointModule, EndpointId = "CancelChildOrder", Scope = "Private", Auth = "KeySecret" });
        var cancelAllChildOrders = CallFactory.Success(
            new CancelAllChildOrdersRequest { ProductCode = "BTC_JPY" },
            new Unit(),
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PrivateEndpointModule, EndpointId = "CancelAllChildOrders", Scope = "Private", Auth = "KeySecret" });
        var cancelParentOrder = CallFactory.Success(
            new CancelParentOrderRequest { ProductCode = "BTC_JPY", ParentOrderId = "JCO1" },
            new Unit(),
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PrivateEndpointModule, EndpointId = "CancelParentOrder", Scope = "Private", Auth = "KeySecret" });

        var api = new BitflyerPrivateNativeApi(
            new FakeGetPermissionsNativeEndpoint(getPermissions),
            new FakeGetAddressesNativeEndpoint(getAddresses),
            new FakeGetCoinInsNativeEndpoint(getCoinIns),
            new FakeGetCoinOutsNativeEndpoint(getCoinOuts),
            new FakeGetBankAccountsNativeEndpoint(getBankAccounts),
            new FakeGetDepositsNativeEndpoint(getDeposits),
            new FakeWithdrawNativeEndpoint(withdraw),
            new FakeGetWithdrawalsNativeEndpoint(getWithdrawals),
            new FakeGetBalanceNativeEndpoint(getBalance),
            new FakeGetParentOrdersNativeEndpoint(getParentOrders),
            new FakeGetParentOrderNativeEndpoint(getParentOrder),
            new FakeGetCollateralNativeEndpoint(getCollateral),
            new FakeGetCollateralAccountsNativeEndpoint(getCollateralAccounts),
            new FakeGetCollateralHistoryNativeEndpoint(getCollateralHistory),
            new FakeGetChildOrdersNativeEndpoint(getChildOrders),
            new FakeGetExecutionsNativeEndpoint(getExecutions),
            new FakeGetBalanceHistoryNativeEndpoint(getBalanceHistory),
            new FakeGetPositionsNativeEndpoint(getPositions),
            new FakeGetTradingCommissionNativeEndpoint(getTradingCommission),
            new FakeSendChildOrderNativeEndpoint(sendChildOrder),
            new FakeSendParentOrderNativeEndpoint(sendParentOrder),
            new FakeCancelChildOrderNativeEndpoint(cancelChildOrder),
            new FakeCancelAllChildOrdersNativeEndpoint(cancelAllChildOrders),
            new FakeCancelParentOrderNativeEndpoint(cancelParentOrder));

        Assert.Same(getPermissions, await api.GetPermissionsCallAsync(new GetPermissionsRequest()));
        Assert.Same(getAddresses, await api.GetAddressesCallAsync(new GetAddressesRequest()));
        Assert.Same(getCoinIns, await api.GetCoinInsCallAsync(new GetCoinInsRequest { Count = 10 }));
        Assert.Same(getCoinOuts, await api.GetCoinOutsCallAsync(new GetCoinOutsRequest { Count = 10 }));
        Assert.Same(getBankAccounts, await api.GetBankAccountsCallAsync(new GetBankAccountsRequest()));
        Assert.Same(getDeposits, await api.GetDepositsCallAsync(new GetDepositsRequest { Count = 10 }));
        Assert.Same(withdraw, await api.WithdrawCallAsync(new WithdrawRequest { CurrencyCode = "JPY", BankAccountId = 1, Amount = 10m, Code = "123456" }));
        Assert.Same(getWithdrawals, await api.GetWithdrawalsCallAsync(new GetWithdrawalsRequest { Count = 10, MessageId = "MID1" }));
        Assert.Same(getBalance, await api.GetBalanceCallAsync(new GetBalanceRequest()));
        Assert.Same(getParentOrders, await api.GetParentOrdersCallAsync(new GetParentOrdersRequest { ProductCode = "BTC_JPY", Count = 10 }));
        Assert.Same(getParentOrder, await api.GetParentOrderCallAsync(new GetParentOrderRequest { ParentOrderId = "JCO1" }));
        Assert.Same(getCollateral, await api.GetCollateralCallAsync(new GetCollateralRequest()));
        Assert.Same(getCollateralAccounts, await api.GetCollateralAccountsCallAsync(new GetCollateralAccountsRequest()));
        Assert.Same(getCollateralHistory, await api.GetCollateralHistoryCallAsync(new GetCollateralHistoryRequest { Count = 10 }));
        Assert.Same(getChildOrders, await api.GetChildOrdersCallAsync(new GetChildOrdersRequest { ProductCode = "BTC_JPY", Count = 10 }));
        Assert.Same(getExecutions, await api.GetExecutionsCallAsync(new GetExecutionsRequest { ProductCode = "BTC_JPY", Count = 10 }));
        Assert.Same(getBalanceHistory, await api.GetBalanceHistoryCallAsync(new GetBalanceHistoryRequest { CurrencyCode = "JPY", Count = 10 }));
        Assert.Same(getPositions, await api.GetPositionsCallAsync(new GetPositionsRequest { ProductCode = "FX_BTC_JPY" }));
        Assert.Same(getTradingCommission, await api.GetTradingCommissionCallAsync(new GetTradingCommissionRequest { ProductCode = "BTC_JPY" }));
        Assert.Same(sendChildOrder, await api.SendChildOrderCallAsync(new SendChildOrderRequest { ProductCode = "BTC_JPY", ChildOrderType = "MARKET", Side = "BUY", Size = 1m }));
        Assert.Same(sendParentOrder, await api.SendParentOrderCallAsync(new SendParentOrderRequest { Parameters = [new SendParentOrderParameter { ProductCode = "BTC_JPY", ConditionType = "LIMIT", Side = "BUY", Price = 1m, Size = 1m }] }));
        Assert.Same(cancelChildOrder, await api.CancelChildOrderCallAsync(new CancelChildOrderRequest { ProductCode = "BTC_JPY", ChildOrderId = "X" }));
        Assert.Same(cancelAllChildOrders, await api.CancelAllChildOrdersCallAsync(new CancelAllChildOrdersRequest { ProductCode = "BTC_JPY" }));
        Assert.Same(cancelParentOrder, await api.CancelParentOrderCallAsync(new CancelParentOrderRequest { ProductCode = "BTC_JPY", ParentOrderId = "JCO1" }));
    }

    private sealed class FakeGetMarketsNativeEndpoint : IGetMarketsNativeEndpoint
    {
        private readonly Call<GetMarketsRequest, IReadOnlyList<GetMarkets.Item>> _call;
        public FakeGetMarketsNativeEndpoint(Call<GetMarketsRequest, IReadOnlyList<GetMarkets.Item>> call) => _call = call;
        public Task<Call<GetMarketsRequest, IReadOnlyList<GetMarkets.Item>>> CallAsync(GetMarketsRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetBoardNativeEndpoint : IGetBoardNativeEndpoint
    {
        private readonly Call<GetBoardRequest, GetBoardResponse> _call;
        public FakeGetBoardNativeEndpoint(Call<GetBoardRequest, GetBoardResponse> call) => _call = call;
        public Task<Call<GetBoardRequest, GetBoardResponse>> CallAsync(GetBoardRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetExecutionsPublicNativeEndpoint : IGetExecutionsPublicNativeEndpoint
    {
        private readonly Call<GetExecutionsPublicRequest, IReadOnlyList<GetExecutionsPublic.Item>> _call;
        public FakeGetExecutionsPublicNativeEndpoint(Call<GetExecutionsPublicRequest, IReadOnlyList<GetExecutionsPublic.Item>> call) => _call = call;
        public Task<Call<GetExecutionsPublicRequest, IReadOnlyList<GetExecutionsPublic.Item>>> CallAsync(GetExecutionsPublicRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetBoardStateNativeEndpoint : IGetBoardStateNativeEndpoint
    {
        private readonly Call<GetBoardStateRequest, GetBoardStateResponse> _call;
        public FakeGetBoardStateNativeEndpoint(Call<GetBoardStateRequest, GetBoardStateResponse> call) => _call = call;
        public Task<Call<GetBoardStateRequest, GetBoardStateResponse>> CallAsync(GetBoardStateRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetHealthNativeEndpoint : IGetHealthNativeEndpoint
    {
        private readonly Call<GetHealthRequest, GetHealthResponse> _call;
        public FakeGetHealthNativeEndpoint(Call<GetHealthRequest, GetHealthResponse> call) => _call = call;
        public Task<Call<GetHealthRequest, GetHealthResponse>> CallAsync(GetHealthRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetFundingRateNativeEndpoint : IGetFundingRateNativeEndpoint
    {
        private readonly Call<GetFundingRateRequest, GetFundingRateResponse> _call;
        public FakeGetFundingRateNativeEndpoint(Call<GetFundingRateRequest, GetFundingRateResponse> call) => _call = call;
        public Task<Call<GetFundingRateRequest, GetFundingRateResponse>> CallAsync(GetFundingRateRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetCorporateLeverageNativeEndpoint : IGetCorporateLeverageNativeEndpoint
    {
        private readonly Call<GetCorporateLeverageRequest, GetCorporateLeverageResponse> _call;
        public FakeGetCorporateLeverageNativeEndpoint(Call<GetCorporateLeverageRequest, GetCorporateLeverageResponse> call) => _call = call;
        public Task<Call<GetCorporateLeverageRequest, GetCorporateLeverageResponse>> CallAsync(GetCorporateLeverageRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetChatsNativeEndpoint : IGetChatsNativeEndpoint
    {
        private readonly Call<GetChatsRequest, IReadOnlyList<GetChats.Item>> _call;
        public FakeGetChatsNativeEndpoint(Call<GetChatsRequest, IReadOnlyList<GetChats.Item>> call) => _call = call;
        public Task<Call<GetChatsRequest, IReadOnlyList<GetChats.Item>>> CallAsync(GetChatsRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetTickerNativeEndpoint : IGetTickerNativeEndpoint
    {
        private readonly Call<GetTickerRequest, GetTickerResponse> _call;
        public FakeGetTickerNativeEndpoint(Call<GetTickerRequest, GetTickerResponse> call) => _call = call;
        public Task<Call<GetTickerRequest, GetTickerResponse>> CallAsync(GetTickerRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetBalanceNativeEndpoint : IGetBalanceNativeEndpoint
    {
        private readonly Call<GetBalanceRequest, IReadOnlyList<GetBalance.Item>> _call;
        public FakeGetBalanceNativeEndpoint(Call<GetBalanceRequest, IReadOnlyList<GetBalance.Item>> call) => _call = call;
        public Task<Call<GetBalanceRequest, IReadOnlyList<GetBalance.Item>>> CallAsync(GetBalanceRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetPermissionsNativeEndpoint : IGetPermissionsNativeEndpoint
    {
        private readonly Call<GetPermissionsRequest, IReadOnlyList<string>> _call;
        public FakeGetPermissionsNativeEndpoint(Call<GetPermissionsRequest, IReadOnlyList<string>> call) => _call = call;
        public Task<Call<GetPermissionsRequest, IReadOnlyList<string>>> CallAsync(GetPermissionsRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetAddressesNativeEndpoint : IGetAddressesNativeEndpoint
    {
        private readonly Call<GetAddressesRequest, IReadOnlyList<GetAddresses.Item>> _call;
        public FakeGetAddressesNativeEndpoint(Call<GetAddressesRequest, IReadOnlyList<GetAddresses.Item>> call) => _call = call;
        public Task<Call<GetAddressesRequest, IReadOnlyList<GetAddresses.Item>>> CallAsync(GetAddressesRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetCoinInsNativeEndpoint : IGetCoinInsNativeEndpoint
    {
        private readonly Call<GetCoinInsRequest, IReadOnlyList<GetCoinIns.Item>> _call;
        public FakeGetCoinInsNativeEndpoint(Call<GetCoinInsRequest, IReadOnlyList<GetCoinIns.Item>> call) => _call = call;
        public Task<Call<GetCoinInsRequest, IReadOnlyList<GetCoinIns.Item>>> CallAsync(GetCoinInsRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetCoinOutsNativeEndpoint : IGetCoinOutsNativeEndpoint
    {
        private readonly Call<GetCoinOutsRequest, IReadOnlyList<GetCoinOuts.Item>> _call;
        public FakeGetCoinOutsNativeEndpoint(Call<GetCoinOutsRequest, IReadOnlyList<GetCoinOuts.Item>> call) => _call = call;
        public Task<Call<GetCoinOutsRequest, IReadOnlyList<GetCoinOuts.Item>>> CallAsync(GetCoinOutsRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetBankAccountsNativeEndpoint : IGetBankAccountsNativeEndpoint
    {
        private readonly Call<GetBankAccountsRequest, IReadOnlyList<GetBankAccounts.Item>> _call;
        public FakeGetBankAccountsNativeEndpoint(Call<GetBankAccountsRequest, IReadOnlyList<GetBankAccounts.Item>> call) => _call = call;
        public Task<Call<GetBankAccountsRequest, IReadOnlyList<GetBankAccounts.Item>>> CallAsync(GetBankAccountsRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetDepositsNativeEndpoint : IGetDepositsNativeEndpoint
    {
        private readonly Call<GetDepositsRequest, IReadOnlyList<GetDeposits.Item>> _call;
        public FakeGetDepositsNativeEndpoint(Call<GetDepositsRequest, IReadOnlyList<GetDeposits.Item>> call) => _call = call;
        public Task<Call<GetDepositsRequest, IReadOnlyList<GetDeposits.Item>>> CallAsync(GetDepositsRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeWithdrawNativeEndpoint : IWithdrawNativeEndpoint
    {
        private readonly Call<WithdrawRequest, WithdrawResponse> _call;
        public FakeWithdrawNativeEndpoint(Call<WithdrawRequest, WithdrawResponse> call) => _call = call;
        public Task<Call<WithdrawRequest, WithdrawResponse>> CallAsync(WithdrawRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetWithdrawalsNativeEndpoint : IGetWithdrawalsNativeEndpoint
    {
        private readonly Call<GetWithdrawalsRequest, IReadOnlyList<GetWithdrawals.Item>> _call;
        public FakeGetWithdrawalsNativeEndpoint(Call<GetWithdrawalsRequest, IReadOnlyList<GetWithdrawals.Item>> call) => _call = call;
        public Task<Call<GetWithdrawalsRequest, IReadOnlyList<GetWithdrawals.Item>>> CallAsync(GetWithdrawalsRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetCollateralNativeEndpoint : IGetCollateralNativeEndpoint
    {
        private readonly Call<GetCollateralRequest, GetCollateralResponse> _call;
        public FakeGetCollateralNativeEndpoint(Call<GetCollateralRequest, GetCollateralResponse> call) => _call = call;
        public Task<Call<GetCollateralRequest, GetCollateralResponse>> CallAsync(GetCollateralRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetParentOrdersNativeEndpoint : IGetParentOrdersNativeEndpoint
    {
        private readonly Call<GetParentOrdersRequest, IReadOnlyList<GetParentOrders.Item>> _call;
        public FakeGetParentOrdersNativeEndpoint(Call<GetParentOrdersRequest, IReadOnlyList<GetParentOrders.Item>> call) => _call = call;
        public Task<Call<GetParentOrdersRequest, IReadOnlyList<GetParentOrders.Item>>> CallAsync(GetParentOrdersRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetParentOrderNativeEndpoint : IGetParentOrderNativeEndpoint
    {
        private readonly Call<GetParentOrderRequest, GetParentOrderResponse> _call;
        public FakeGetParentOrderNativeEndpoint(Call<GetParentOrderRequest, GetParentOrderResponse> call) => _call = call;
        public Task<Call<GetParentOrderRequest, GetParentOrderResponse>> CallAsync(GetParentOrderRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetCollateralAccountsNativeEndpoint : IGetCollateralAccountsNativeEndpoint
    {
        private readonly Call<GetCollateralAccountsRequest, IReadOnlyList<GetCollateralAccounts.Item>> _call;
        public FakeGetCollateralAccountsNativeEndpoint(Call<GetCollateralAccountsRequest, IReadOnlyList<GetCollateralAccounts.Item>> call) => _call = call;
        public Task<Call<GetCollateralAccountsRequest, IReadOnlyList<GetCollateralAccounts.Item>>> CallAsync(GetCollateralAccountsRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetCollateralHistoryNativeEndpoint : IGetCollateralHistoryNativeEndpoint
    {
        private readonly Call<GetCollateralHistoryRequest, IReadOnlyList<GetCollateralHistory.Item>> _call;
        public FakeGetCollateralHistoryNativeEndpoint(Call<GetCollateralHistoryRequest, IReadOnlyList<GetCollateralHistory.Item>> call) => _call = call;
        public Task<Call<GetCollateralHistoryRequest, IReadOnlyList<GetCollateralHistory.Item>>> CallAsync(GetCollateralHistoryRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetChildOrdersNativeEndpoint : IGetChildOrdersNativeEndpoint
    {
        private readonly Call<GetChildOrdersRequest, IReadOnlyList<GetChildOrders.Item>> _call;
        public FakeGetChildOrdersNativeEndpoint(Call<GetChildOrdersRequest, IReadOnlyList<GetChildOrders.Item>> call) => _call = call;
        public Task<Call<GetChildOrdersRequest, IReadOnlyList<GetChildOrders.Item>>> CallAsync(GetChildOrdersRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetExecutionsNativeEndpoint : IGetExecutionsNativeEndpoint
    {
        private readonly Call<GetExecutionsRequest, IReadOnlyList<GetExecutions.Item>> _call;
        public FakeGetExecutionsNativeEndpoint(Call<GetExecutionsRequest, IReadOnlyList<GetExecutions.Item>> call) => _call = call;
        public Task<Call<GetExecutionsRequest, IReadOnlyList<GetExecutions.Item>>> CallAsync(GetExecutionsRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetBalanceHistoryNativeEndpoint : IGetBalanceHistoryNativeEndpoint
    {
        private readonly Call<GetBalanceHistoryRequest, IReadOnlyList<GetBalanceHistory.Item>> _call;
        public FakeGetBalanceHistoryNativeEndpoint(Call<GetBalanceHistoryRequest, IReadOnlyList<GetBalanceHistory.Item>> call) => _call = call;
        public Task<Call<GetBalanceHistoryRequest, IReadOnlyList<GetBalanceHistory.Item>>> CallAsync(GetBalanceHistoryRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetPositionsNativeEndpoint : IGetPositionsNativeEndpoint
    {
        private readonly Call<GetPositionsRequest, IReadOnlyList<GetPositions.Item>> _call;
        public FakeGetPositionsNativeEndpoint(Call<GetPositionsRequest, IReadOnlyList<GetPositions.Item>> call) => _call = call;
        public Task<Call<GetPositionsRequest, IReadOnlyList<GetPositions.Item>>> CallAsync(GetPositionsRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetTradingCommissionNativeEndpoint : IGetTradingCommissionNativeEndpoint
    {
        private readonly Call<GetTradingCommissionRequest, GetTradingCommissionResponse> _call;
        public FakeGetTradingCommissionNativeEndpoint(Call<GetTradingCommissionRequest, GetTradingCommissionResponse> call) => _call = call;
        public Task<Call<GetTradingCommissionRequest, GetTradingCommissionResponse>> CallAsync(GetTradingCommissionRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeSendChildOrderNativeEndpoint : ISendChildOrderNativeEndpoint
    {
        private readonly Call<SendChildOrderRequest, SendChildOrderResponse> _call;
        public FakeSendChildOrderNativeEndpoint(Call<SendChildOrderRequest, SendChildOrderResponse> call) => _call = call;
        public Task<Call<SendChildOrderRequest, SendChildOrderResponse>> CallAsync(SendChildOrderRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeSendParentOrderNativeEndpoint : ISendParentOrderNativeEndpoint
    {
        private readonly Call<SendParentOrderRequest, SendParentOrderResponse> _call;
        public FakeSendParentOrderNativeEndpoint(Call<SendParentOrderRequest, SendParentOrderResponse> call) => _call = call;
        public Task<Call<SendParentOrderRequest, SendParentOrderResponse>> CallAsync(SendParentOrderRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeCancelChildOrderNativeEndpoint : ICancelChildOrderNativeEndpoint
    {
        private readonly Call<CancelChildOrderRequest, Unit> _call;
        public FakeCancelChildOrderNativeEndpoint(Call<CancelChildOrderRequest, Unit> call) => _call = call;
        public Task<Call<CancelChildOrderRequest, Unit>> CallAsync(CancelChildOrderRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeCancelAllChildOrdersNativeEndpoint : ICancelAllChildOrdersNativeEndpoint
    {
        private readonly Call<CancelAllChildOrdersRequest, Unit> _call;
        public FakeCancelAllChildOrdersNativeEndpoint(Call<CancelAllChildOrdersRequest, Unit> call) => _call = call;
        public Task<Call<CancelAllChildOrdersRequest, Unit>> CallAsync(CancelAllChildOrdersRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeCancelParentOrderNativeEndpoint : ICancelParentOrderNativeEndpoint
    {
        private readonly Call<CancelParentOrderRequest, Unit> _call;
        public FakeCancelParentOrderNativeEndpoint(Call<CancelParentOrderRequest, Unit> call) => _call = call;
        public Task<Call<CancelParentOrderRequest, Unit>> CallAsync(CancelParentOrderRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }
}
