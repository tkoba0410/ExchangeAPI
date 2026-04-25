using ExchangeApi.Exchanges.Bitflyer.Native.Private.Api;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.CancelAllChildOrders;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.CancelChildOrder;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.CancelParentOrder;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetAddresses;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetBalance;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetBalanceHistory;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetBankAccounts;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetChildOrders;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCoinIns;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCoinOuts;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCollateral;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCollateralAccounts;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCollateralHistory;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetDeposits;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetExecutions;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetParentOrder;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetParentOrders;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetPermissions;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetPositions;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetTradingCommission;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetWithdrawals;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.SendChildOrder;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.SendParentOrder;
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
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
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
            (IReadOnlyList<GetMarkets.Item>)[new GetMarkets.Item { ProductCode = "BTC_JPY", MarketType = MarketTypes.Spot }],
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
                Health = HealthStatuses.Normal,
                State = TradingStates.Running,
                Data = new GetBoardStateData { SpecialQuotation = 1m },
            },
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PublicEndpointModule, EndpointId = "GetBoardState", Scope = "Public", Auth = "None" });
        var getHealth = CallFactory.Success(
            new GetHealthRequest { ProductCode = "BTC_JPY" },
            new GetHealthResponse { Status = HealthStatuses.Normal },
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
                Side = OrderSides.Buy,
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
                State = TradingStates.Running,
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

        var actualMarkets = await api.GetMarketsAsync(new GetMarketsRequest());
        var actualBoard = await api.GetBoardAsync(new GetBoardRequest { ProductCode = null });
        var actualBoardState = await api.GetBoardStateAsync(new GetBoardStateRequest { ProductCode = "BTC_JPY" });
        var actualHealth = await api.GetHealthAsync(new GetHealthRequest { ProductCode = "BTC_JPY" });
        var actualFundingRate = await api.GetFundingRateAsync(new GetFundingRateRequest { ProductCode = "FX_BTC_JPY" });
        var actualCorporateLeverage = await api.GetCorporateLeverageAsync(new GetCorporateLeverageRequest());
        var actualChats = await api.GetChatsAsync(new GetChatsRequest());
        var actualExecutions = await api.GetExecutionsAsync(new GetExecutionsPublicRequest { ProductCode = "BTC_JPY", Count = 10 });
        var actualTicker = await api.GetTickerAsync(new GetTickerRequest { ProductCode = null });

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
            (IReadOnlyList<GetAddresses.Item>)[new GetAddresses.Item { Type = AddressTypes.Normal, CurrencyCode = "BTC", Address = "addr" }],
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PrivateEndpointModule, EndpointId = "GetAddresses", Scope = "Private", Auth = "KeySecret" });
        var getCoinIns = CallFactory.Success(
            new GetCoinInsRequest { Count = 10 },
            (IReadOnlyList<GetCoinIns.Item>)[new GetCoinIns.Item { Id = 1, OrderId = "CDP1", CurrencyCode = "BTC", Amount = 1m, Address = "addr", TxHash = "hash", Status = TransferStatuses.Completed, EventDate = DateTimeOffset.UnixEpoch }],
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PrivateEndpointModule, EndpointId = "GetCoinIns", Scope = "Private", Auth = "KeySecret" });
        var getCoinOuts = CallFactory.Success(
            new GetCoinOutsRequest { Count = 10 },
            (IReadOnlyList<GetCoinOuts.Item>)[new GetCoinOuts.Item { Id = 1, OrderId = "CWD1", CurrencyCode = "BTC", Amount = 1m, Address = "addr", TxHash = "hash", Fee = 0.1m, AdditionalFee = 0.01m, Status = TransferStatuses.Completed, EventDate = DateTimeOffset.UnixEpoch }],
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PrivateEndpointModule, EndpointId = "GetCoinOuts", Scope = "Private", Auth = "KeySecret" });
        var getBankAccounts = CallFactory.Success(
            new GetBankAccountsRequest(),
            (IReadOnlyList<GetBankAccounts.Item>)[new GetBankAccounts.Item { Id = 1, IsVerified = true, BankName = "bank", BranchName = "branch", AccountType = "普通", AccountNumber = "123", AccountName = "name" }],
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PrivateEndpointModule, EndpointId = "GetBankAccounts", Scope = "Private", Auth = "KeySecret" });
        var getDeposits = CallFactory.Success(
            new GetDepositsRequest { Count = 10 },
            (IReadOnlyList<GetDeposits.Item>)[new GetDeposits.Item { Id = 1, OrderId = "MDP1", CurrencyCode = "JPY", Amount = 10m, Status = TransferStatuses.Completed, EventDate = DateTimeOffset.UnixEpoch }],
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PrivateEndpointModule, EndpointId = "GetDeposits", Scope = "Private", Auth = "KeySecret" });
        var withdraw = CallFactory.Success(
            new WithdrawRequest { CurrencyCode = "JPY", BankAccountId = 1, Amount = 10m, Code = "123456" },
            new WithdrawResponse { MessageId = "MID1" },
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PrivateEndpointModule, EndpointId = "Withdraw", Scope = "Private", Auth = "KeySecret" });
        var getWithdrawals = CallFactory.Success(
            new GetWithdrawalsRequest { Count = 10, MessageId = "MID1" },
            (IReadOnlyList<GetWithdrawals.Item>)[new GetWithdrawals.Item { Id = 1, OrderId = "MWD1", CurrencyCode = "JPY", Amount = 10m, Status = TransferStatuses.Completed, EventDate = DateTimeOffset.UnixEpoch }],
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PrivateEndpointModule, EndpointId = "GetWithdrawals", Scope = "Private", Auth = "KeySecret" });
        var getBalance = CallFactory.Success(
            new GetBalanceRequest(),
            (IReadOnlyList<GetBalance.Item>)[new GetBalance.Item { CurrencyCode = "JPY", Amount = 1m, Available = 1m }],
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PrivateEndpointModule, EndpointId = "GetBalance", Scope = "Private", Auth = "KeySecret" });
        var getParentOrders = CallFactory.Success(
            new GetParentOrdersRequest { ProductCode = "BTC_JPY", Count = 10 },
            (IReadOnlyList<GetParentOrders.Item>)[new GetParentOrders.Item { Id = 1, ParentOrderId = "JCO1", ProductCode = "BTC_JPY", Side = ParentOrderSides.Buy, ParentOrderType = ParentOrderTypes.Unknown, Price = 1m, AveragePrice = 1m, Size = 1m, ParentOrderState = ParentOrderStates.Active, ExpireDate = DateTimeOffset.UnixEpoch, ParentOrderDate = DateTimeOffset.UnixEpoch, ParentOrderAcceptanceId = "JRF1", OutstandingSize = 1m, CancelSize = 0m, ExecutedSize = 0m, TotalCommission = 0m }],
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PrivateEndpointModule, EndpointId = "GetParentOrders", Scope = "Private", Auth = "KeySecret" });
        var getParentOrder = CallFactory.Success(
            new GetParentOrderRequest { ParentOrderId = "JCO1" },
            new GetParentOrderResponse { Id = 1, ParentOrderId = "JCO1", OrderMethod = ParentOrderMethods.Simple, ExpireDate = DateTimeOffset.UnixEpoch, TimeInForce = TimeInForces.Gtc, Parameters = [new GetParentOrderParameter { ProductCode = "BTC_JPY", ConditionType = ParentOrderConditionTypes.Limit, Side = OrderSides.Buy, Price = 1m, Size = 1m, TriggerPrice = 0m, Offset = 0 }], ParentOrderAcceptanceId = "JRF1" },
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
                Side = OrderSides.Buy,
                ChildOrderType = ChildOrderTypes.Limit,
                Price = 1m,
                AveragePrice = 1m,
                Size = 1m,
                ChildOrderState = ChildOrderStates.Completed,
                ExpireDate = DateTimeOffset.UnixEpoch,
                ChildOrderDate = DateTimeOffset.UnixEpoch,
                ChildOrderAcceptanceId = "JRF1",
                OutstandingSize = 0m,
                CancelSize = 0m,
                ExecutedSize = 1m,
                TotalCommission = 0m,
                TimeInForce = TimeInForces.Gtc,
            }],
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PrivateEndpointModule, EndpointId = "GetChildOrders", Scope = "Private", Auth = "KeySecret" });
        var getExecutions = CallFactory.Success(
            new GetExecutionsRequest { ProductCode = "BTC_JPY", Count = 10 },
            (IReadOnlyList<GetExecutions.Item>)[new GetExecutions.Item
            {
                Id = 1,
                ChildOrderId = "JOR1",
                Side = OrderSides.Buy,
                Price = 1m,
                Size = 1m,
                Commission = 0m,
                ExecDate = DateTimeOffset.UnixEpoch,
                ChildOrderAcceptanceId = "JRF1",
            }],
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PrivateEndpointModule, EndpointId = "GetExecutionsPrivate", Scope = "Private", Auth = "KeySecret" });
        var getBalanceHistory = CallFactory.Success(
            new GetBalanceHistoryRequest { CurrencyCode = "JPY", Count = 10 },
            (IReadOnlyList<GetBalanceHistory.Item>)[new GetBalanceHistory.Item { Id = 1, TradeDate = DateTimeOffset.UnixEpoch, EventDate = DateTimeOffset.UnixEpoch, ProductCode = null, CurrencyCode = "JPY", TradeType = TradeTypes.Deposit, Price = 0m, Amount = 10m, Quantity = 0m, Commission = 0m, Balance = 10m, OrderId = "ORDER1" }],
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PrivateEndpointModule, EndpointId = "GetBalanceHistory", Scope = "Private", Auth = "KeySecret" });
        var getPositions = CallFactory.Success(
            new GetPositionsRequest { ProductCode = "FX_BTC_JPY" },
            (IReadOnlyList<GetPositions.Item>)[new GetPositions.Item
            {
                ProductCode = "FX_BTC_JPY",
                Side = OrderSides.Buy,
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
            new SendChildOrderRequest { ProductCode = "BTC_JPY", ChildOrderType = ChildOrderTypes.Market, Side = OrderSides.Buy, Size = 1m },
            new SendChildOrderResponse { ChildOrderAcceptanceId = "A" },
            new CallMeta { Layer = CallLayers.Native, Component = CallComponents.PrivateEndpointModule, EndpointId = "SendChildOrder", Scope = "Private", Auth = "KeySecret" });
        var sendParentOrder = CallFactory.Success(
            new SendParentOrderRequest { Parameters = [new SendParentOrderParameter { ProductCode = "BTC_JPY", ConditionType = ParentOrderConditionTypes.Limit, Side = OrderSides.Buy, Price = 1m, Size = 1m }] },
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

        Assert.Same(getPermissions, await api.GetPermissionsAsync(new GetPermissionsRequest()));
        Assert.Same(getAddresses, await api.GetAddressesAsync(new GetAddressesRequest()));
        Assert.Same(getCoinIns, await api.GetCoinInsAsync(new GetCoinInsRequest { Count = 10 }));
        Assert.Same(getCoinOuts, await api.GetCoinOutsAsync(new GetCoinOutsRequest { Count = 10 }));
        Assert.Same(getBankAccounts, await api.GetBankAccountsAsync(new GetBankAccountsRequest()));
        Assert.Same(getDeposits, await api.GetDepositsAsync(new GetDepositsRequest { Count = 10 }));
        Assert.Same(withdraw, await api.WithdrawAsync(new WithdrawRequest { CurrencyCode = "JPY", BankAccountId = 1, Amount = 10m, Code = "123456" }));
        Assert.Same(getWithdrawals, await api.GetWithdrawalsAsync(new GetWithdrawalsRequest { Count = 10, MessageId = "MID1" }));
        Assert.Same(getBalance, await api.GetBalanceAsync(new GetBalanceRequest()));
        Assert.Same(getParentOrders, await api.GetParentOrdersAsync(new GetParentOrdersRequest { ProductCode = "BTC_JPY", Count = 10 }));
        Assert.Same(getParentOrder, await api.GetParentOrderAsync(new GetParentOrderRequest { ParentOrderId = "JCO1" }));
        Assert.Same(getCollateral, await api.GetCollateralAsync(new GetCollateralRequest()));
        Assert.Same(getCollateralAccounts, await api.GetCollateralAccountsAsync(new GetCollateralAccountsRequest()));
        Assert.Same(getCollateralHistory, await api.GetCollateralHistoryAsync(new GetCollateralHistoryRequest { Count = 10 }));
        Assert.Same(getChildOrders, await api.GetChildOrdersAsync(new GetChildOrdersRequest { ProductCode = "BTC_JPY", Count = 10 }));
        Assert.Same(getExecutions, await api.GetExecutionsAsync(new GetExecutionsRequest { ProductCode = "BTC_JPY", Count = 10 }));
        Assert.Same(getBalanceHistory, await api.GetBalanceHistoryAsync(new GetBalanceHistoryRequest { CurrencyCode = "JPY", Count = 10 }));
        Assert.Same(getPositions, await api.GetPositionsAsync(new GetPositionsRequest { ProductCode = "FX_BTC_JPY" }));
        Assert.Same(getTradingCommission, await api.GetTradingCommissionAsync(new GetTradingCommissionRequest { ProductCode = "BTC_JPY" }));
        Assert.Same(sendChildOrder, await api.SendChildOrderAsync(new SendChildOrderRequest { ProductCode = "BTC_JPY", ChildOrderType = ChildOrderTypes.Market, Side = OrderSides.Buy, Size = 1m }));
        Assert.Same(sendParentOrder, await api.SendParentOrderAsync(new SendParentOrderRequest { Parameters = [new SendParentOrderParameter { ProductCode = "BTC_JPY", ConditionType = ParentOrderConditionTypes.Limit, Side = OrderSides.Buy, Price = 1m, Size = 1m }] }));
        Assert.Same(cancelChildOrder, await api.CancelChildOrderAsync(new CancelChildOrderRequest { ProductCode = "BTC_JPY", ChildOrderId = "X" }));
        Assert.Same(cancelAllChildOrders, await api.CancelAllChildOrdersAsync(new CancelAllChildOrdersRequest { ProductCode = "BTC_JPY" }));
        Assert.Same(cancelParentOrder, await api.CancelParentOrderAsync(new CancelParentOrderRequest { ProductCode = "BTC_JPY", ParentOrderId = "JCO1" }));
    }

    private sealed class FakeGetMarketsNativeEndpoint : IGetMarketsNativeEndpoint
    {
        private readonly CallResult<GetMarketsRequest, IReadOnlyList<GetMarkets.Item>> _call;
        public FakeGetMarketsNativeEndpoint(CallResult<GetMarketsRequest, IReadOnlyList<GetMarkets.Item>> call) => _call = call;
        public Task<CallResult<GetMarketsRequest, IReadOnlyList<GetMarkets.Item>>> CallAsync(GetMarketsRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetBoardNativeEndpoint : IGetBoardNativeEndpoint
    {
        private readonly CallResult<GetBoardRequest, GetBoardResponse> _call;
        public FakeGetBoardNativeEndpoint(CallResult<GetBoardRequest, GetBoardResponse> call) => _call = call;
        public Task<CallResult<GetBoardRequest, GetBoardResponse>> CallAsync(GetBoardRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetExecutionsPublicNativeEndpoint : IGetExecutionsPublicNativeEndpoint
    {
        private readonly CallResult<GetExecutionsPublicRequest, IReadOnlyList<GetExecutionsPublic.Item>> _call;
        public FakeGetExecutionsPublicNativeEndpoint(CallResult<GetExecutionsPublicRequest, IReadOnlyList<GetExecutionsPublic.Item>> call) => _call = call;
        public Task<CallResult<GetExecutionsPublicRequest, IReadOnlyList<GetExecutionsPublic.Item>>> CallAsync(GetExecutionsPublicRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetBoardStateNativeEndpoint : IGetBoardStateNativeEndpoint
    {
        private readonly CallResult<GetBoardStateRequest, GetBoardStateResponse> _call;
        public FakeGetBoardStateNativeEndpoint(CallResult<GetBoardStateRequest, GetBoardStateResponse> call) => _call = call;
        public Task<CallResult<GetBoardStateRequest, GetBoardStateResponse>> CallAsync(GetBoardStateRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetHealthNativeEndpoint : IGetHealthNativeEndpoint
    {
        private readonly CallResult<GetHealthRequest, GetHealthResponse> _call;
        public FakeGetHealthNativeEndpoint(CallResult<GetHealthRequest, GetHealthResponse> call) => _call = call;
        public Task<CallResult<GetHealthRequest, GetHealthResponse>> CallAsync(GetHealthRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetFundingRateNativeEndpoint : IGetFundingRateNativeEndpoint
    {
        private readonly CallResult<GetFundingRateRequest, GetFundingRateResponse> _call;
        public FakeGetFundingRateNativeEndpoint(CallResult<GetFundingRateRequest, GetFundingRateResponse> call) => _call = call;
        public Task<CallResult<GetFundingRateRequest, GetFundingRateResponse>> CallAsync(GetFundingRateRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetCorporateLeverageNativeEndpoint : IGetCorporateLeverageNativeEndpoint
    {
        private readonly CallResult<GetCorporateLeverageRequest, GetCorporateLeverageResponse> _call;
        public FakeGetCorporateLeverageNativeEndpoint(CallResult<GetCorporateLeverageRequest, GetCorporateLeverageResponse> call) => _call = call;
        public Task<CallResult<GetCorporateLeverageRequest, GetCorporateLeverageResponse>> CallAsync(GetCorporateLeverageRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetChatsNativeEndpoint : IGetChatsNativeEndpoint
    {
        private readonly CallResult<GetChatsRequest, IReadOnlyList<GetChats.Item>> _call;
        public FakeGetChatsNativeEndpoint(CallResult<GetChatsRequest, IReadOnlyList<GetChats.Item>> call) => _call = call;
        public Task<CallResult<GetChatsRequest, IReadOnlyList<GetChats.Item>>> CallAsync(GetChatsRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetTickerNativeEndpoint : IGetTickerNativeEndpoint
    {
        private readonly CallResult<GetTickerRequest, GetTickerResponse> _call;
        public FakeGetTickerNativeEndpoint(CallResult<GetTickerRequest, GetTickerResponse> call) => _call = call;
        public Task<CallResult<GetTickerRequest, GetTickerResponse>> CallAsync(GetTickerRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetBalanceNativeEndpoint : IGetBalanceNativeEndpoint
    {
        private readonly CallResult<GetBalanceRequest, IReadOnlyList<GetBalance.Item>> _call;
        public FakeGetBalanceNativeEndpoint(CallResult<GetBalanceRequest, IReadOnlyList<GetBalance.Item>> call) => _call = call;
        public Task<CallResult<GetBalanceRequest, IReadOnlyList<GetBalance.Item>>> CallAsync(GetBalanceRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetPermissionsNativeEndpoint : IGetPermissionsNativeEndpoint
    {
        private readonly CallResult<GetPermissionsRequest, IReadOnlyList<string>> _call;
        public FakeGetPermissionsNativeEndpoint(CallResult<GetPermissionsRequest, IReadOnlyList<string>> call) => _call = call;
        public Task<CallResult<GetPermissionsRequest, IReadOnlyList<string>>> CallAsync(GetPermissionsRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetAddressesNativeEndpoint : IGetAddressesNativeEndpoint
    {
        private readonly CallResult<GetAddressesRequest, IReadOnlyList<GetAddresses.Item>> _call;
        public FakeGetAddressesNativeEndpoint(CallResult<GetAddressesRequest, IReadOnlyList<GetAddresses.Item>> call) => _call = call;
        public Task<CallResult<GetAddressesRequest, IReadOnlyList<GetAddresses.Item>>> CallAsync(GetAddressesRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetCoinInsNativeEndpoint : IGetCoinInsNativeEndpoint
    {
        private readonly CallResult<GetCoinInsRequest, IReadOnlyList<GetCoinIns.Item>> _call;
        public FakeGetCoinInsNativeEndpoint(CallResult<GetCoinInsRequest, IReadOnlyList<GetCoinIns.Item>> call) => _call = call;
        public Task<CallResult<GetCoinInsRequest, IReadOnlyList<GetCoinIns.Item>>> CallAsync(GetCoinInsRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetCoinOutsNativeEndpoint : IGetCoinOutsNativeEndpoint
    {
        private readonly CallResult<GetCoinOutsRequest, IReadOnlyList<GetCoinOuts.Item>> _call;
        public FakeGetCoinOutsNativeEndpoint(CallResult<GetCoinOutsRequest, IReadOnlyList<GetCoinOuts.Item>> call) => _call = call;
        public Task<CallResult<GetCoinOutsRequest, IReadOnlyList<GetCoinOuts.Item>>> CallAsync(GetCoinOutsRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetBankAccountsNativeEndpoint : IGetBankAccountsNativeEndpoint
    {
        private readonly CallResult<GetBankAccountsRequest, IReadOnlyList<GetBankAccounts.Item>> _call;
        public FakeGetBankAccountsNativeEndpoint(CallResult<GetBankAccountsRequest, IReadOnlyList<GetBankAccounts.Item>> call) => _call = call;
        public Task<CallResult<GetBankAccountsRequest, IReadOnlyList<GetBankAccounts.Item>>> CallAsync(GetBankAccountsRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetDepositsNativeEndpoint : IGetDepositsNativeEndpoint
    {
        private readonly CallResult<GetDepositsRequest, IReadOnlyList<GetDeposits.Item>> _call;
        public FakeGetDepositsNativeEndpoint(CallResult<GetDepositsRequest, IReadOnlyList<GetDeposits.Item>> call) => _call = call;
        public Task<CallResult<GetDepositsRequest, IReadOnlyList<GetDeposits.Item>>> CallAsync(GetDepositsRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeWithdrawNativeEndpoint : IWithdrawNativeEndpoint
    {
        private readonly CallResult<WithdrawRequest, WithdrawResponse> _call;
        public FakeWithdrawNativeEndpoint(CallResult<WithdrawRequest, WithdrawResponse> call) => _call = call;
        public Task<CallResult<WithdrawRequest, WithdrawResponse>> CallAsync(WithdrawRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetWithdrawalsNativeEndpoint : IGetWithdrawalsNativeEndpoint
    {
        private readonly CallResult<GetWithdrawalsRequest, IReadOnlyList<GetWithdrawals.Item>> _call;
        public FakeGetWithdrawalsNativeEndpoint(CallResult<GetWithdrawalsRequest, IReadOnlyList<GetWithdrawals.Item>> call) => _call = call;
        public Task<CallResult<GetWithdrawalsRequest, IReadOnlyList<GetWithdrawals.Item>>> CallAsync(GetWithdrawalsRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetCollateralNativeEndpoint : IGetCollateralNativeEndpoint
    {
        private readonly CallResult<GetCollateralRequest, GetCollateralResponse> _call;
        public FakeGetCollateralNativeEndpoint(CallResult<GetCollateralRequest, GetCollateralResponse> call) => _call = call;
        public Task<CallResult<GetCollateralRequest, GetCollateralResponse>> CallAsync(GetCollateralRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetParentOrdersNativeEndpoint : IGetParentOrdersNativeEndpoint
    {
        private readonly CallResult<GetParentOrdersRequest, IReadOnlyList<GetParentOrders.Item>> _call;
        public FakeGetParentOrdersNativeEndpoint(CallResult<GetParentOrdersRequest, IReadOnlyList<GetParentOrders.Item>> call) => _call = call;
        public Task<CallResult<GetParentOrdersRequest, IReadOnlyList<GetParentOrders.Item>>> CallAsync(GetParentOrdersRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetParentOrderNativeEndpoint : IGetParentOrderNativeEndpoint
    {
        private readonly CallResult<GetParentOrderRequest, GetParentOrderResponse> _call;
        public FakeGetParentOrderNativeEndpoint(CallResult<GetParentOrderRequest, GetParentOrderResponse> call) => _call = call;
        public Task<CallResult<GetParentOrderRequest, GetParentOrderResponse>> CallAsync(GetParentOrderRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetCollateralAccountsNativeEndpoint : IGetCollateralAccountsNativeEndpoint
    {
        private readonly CallResult<GetCollateralAccountsRequest, IReadOnlyList<GetCollateralAccounts.Item>> _call;
        public FakeGetCollateralAccountsNativeEndpoint(CallResult<GetCollateralAccountsRequest, IReadOnlyList<GetCollateralAccounts.Item>> call) => _call = call;
        public Task<CallResult<GetCollateralAccountsRequest, IReadOnlyList<GetCollateralAccounts.Item>>> CallAsync(GetCollateralAccountsRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetCollateralHistoryNativeEndpoint : IGetCollateralHistoryNativeEndpoint
    {
        private readonly CallResult<GetCollateralHistoryRequest, IReadOnlyList<GetCollateralHistory.Item>> _call;
        public FakeGetCollateralHistoryNativeEndpoint(CallResult<GetCollateralHistoryRequest, IReadOnlyList<GetCollateralHistory.Item>> call) => _call = call;
        public Task<CallResult<GetCollateralHistoryRequest, IReadOnlyList<GetCollateralHistory.Item>>> CallAsync(GetCollateralHistoryRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetChildOrdersNativeEndpoint : IGetChildOrdersNativeEndpoint
    {
        private readonly CallResult<GetChildOrdersRequest, IReadOnlyList<GetChildOrders.Item>> _call;
        public FakeGetChildOrdersNativeEndpoint(CallResult<GetChildOrdersRequest, IReadOnlyList<GetChildOrders.Item>> call) => _call = call;
        public Task<CallResult<GetChildOrdersRequest, IReadOnlyList<GetChildOrders.Item>>> CallAsync(GetChildOrdersRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetExecutionsNativeEndpoint : IGetExecutionsNativeEndpoint
    {
        private readonly CallResult<GetExecutionsRequest, IReadOnlyList<GetExecutions.Item>> _call;
        public FakeGetExecutionsNativeEndpoint(CallResult<GetExecutionsRequest, IReadOnlyList<GetExecutions.Item>> call) => _call = call;
        public Task<CallResult<GetExecutionsRequest, IReadOnlyList<GetExecutions.Item>>> CallAsync(GetExecutionsRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetBalanceHistoryNativeEndpoint : IGetBalanceHistoryNativeEndpoint
    {
        private readonly CallResult<GetBalanceHistoryRequest, IReadOnlyList<GetBalanceHistory.Item>> _call;
        public FakeGetBalanceHistoryNativeEndpoint(CallResult<GetBalanceHistoryRequest, IReadOnlyList<GetBalanceHistory.Item>> call) => _call = call;
        public Task<CallResult<GetBalanceHistoryRequest, IReadOnlyList<GetBalanceHistory.Item>>> CallAsync(GetBalanceHistoryRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetPositionsNativeEndpoint : IGetPositionsNativeEndpoint
    {
        private readonly CallResult<GetPositionsRequest, IReadOnlyList<GetPositions.Item>> _call;
        public FakeGetPositionsNativeEndpoint(CallResult<GetPositionsRequest, IReadOnlyList<GetPositions.Item>> call) => _call = call;
        public Task<CallResult<GetPositionsRequest, IReadOnlyList<GetPositions.Item>>> CallAsync(GetPositionsRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeGetTradingCommissionNativeEndpoint : IGetTradingCommissionNativeEndpoint
    {
        private readonly CallResult<GetTradingCommissionRequest, GetTradingCommissionResponse> _call;
        public FakeGetTradingCommissionNativeEndpoint(CallResult<GetTradingCommissionRequest, GetTradingCommissionResponse> call) => _call = call;
        public Task<CallResult<GetTradingCommissionRequest, GetTradingCommissionResponse>> CallAsync(GetTradingCommissionRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeSendChildOrderNativeEndpoint : ISendChildOrderNativeEndpoint
    {
        private readonly CallResult<SendChildOrderRequest, SendChildOrderResponse> _call;
        public FakeSendChildOrderNativeEndpoint(CallResult<SendChildOrderRequest, SendChildOrderResponse> call) => _call = call;
        public Task<CallResult<SendChildOrderRequest, SendChildOrderResponse>> CallAsync(SendChildOrderRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeSendParentOrderNativeEndpoint : ISendParentOrderNativeEndpoint
    {
        private readonly CallResult<SendParentOrderRequest, SendParentOrderResponse> _call;
        public FakeSendParentOrderNativeEndpoint(CallResult<SendParentOrderRequest, SendParentOrderResponse> call) => _call = call;
        public Task<CallResult<SendParentOrderRequest, SendParentOrderResponse>> CallAsync(SendParentOrderRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeCancelChildOrderNativeEndpoint : ICancelChildOrderNativeEndpoint
    {
        private readonly CallResult<CancelChildOrderRequest, Unit> _call;
        public FakeCancelChildOrderNativeEndpoint(CallResult<CancelChildOrderRequest, Unit> call) => _call = call;
        public Task<CallResult<CancelChildOrderRequest, Unit>> CallAsync(CancelChildOrderRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeCancelAllChildOrdersNativeEndpoint : ICancelAllChildOrdersNativeEndpoint
    {
        private readonly CallResult<CancelAllChildOrdersRequest, Unit> _call;
        public FakeCancelAllChildOrdersNativeEndpoint(CallResult<CancelAllChildOrdersRequest, Unit> call) => _call = call;
        public Task<CallResult<CancelAllChildOrdersRequest, Unit>> CallAsync(CancelAllChildOrdersRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }

    private sealed class FakeCancelParentOrderNativeEndpoint : ICancelParentOrderNativeEndpoint
    {
        private readonly CallResult<CancelParentOrderRequest, Unit> _call;
        public FakeCancelParentOrderNativeEndpoint(CallResult<CancelParentOrderRequest, Unit> call) => _call = call;
        public Task<CallResult<CancelParentOrderRequest, Unit>> CallAsync(CancelParentOrderRequest request, CancellationToken cancellationToken = default) => Task.FromResult(_call);
    }
}
