using System.Reflection;
using System.Text.Json.Serialization;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Api;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.CancelAllChildOrders;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.CancelChildOrder;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.CancelParentOrder;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetAddresses;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetBalance;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetBalanceHistory;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetBankAccounts;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCollateral;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCollateralAccounts;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCollateralHistory;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetChildOrders;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCoinIns;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCoinOuts;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetDeposits;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetExecutions;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetParentOrder;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetParentOrders;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetPermissions;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetPositions;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.SendChildOrder;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.SendParentOrder;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetTradingCommission;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetWithdrawals;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.Withdraw;
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

public sealed class FixedContractTests
{
    [Fact]
    public void Fixed_Request_And_Response_Dtos_KeepKnown_JsonPropertyNames()
    {
        AssertJsonProperty(typeof(GetMarkets.Item), nameof(GetMarkets.Item.ProductCode), typeof(string), "product_code");
        AssertJsonProperty(typeof(GetMarkets.Item), nameof(GetMarkets.Item.MarketType), typeof(BitflyerMarketType), "market_type");

        AssertJsonProperty(typeof(GetBoardRequest), nameof(GetBoardRequest.ProductCode), typeof(string), "product_code");
        AssertProperty(typeof(GetBoardResponse), nameof(GetBoardResponse.MidPrice), typeof(decimal));
        AssertProperty(typeof(GetBoardResponse), nameof(GetBoardResponse.Bids), typeof(IReadOnlyList<GetBoardLevel>));
        AssertProperty(typeof(GetBoardResponse), nameof(GetBoardResponse.Asks), typeof(IReadOnlyList<GetBoardLevel>));
        AssertJsonProperty(typeof(GetBoardLevel), nameof(GetBoardLevel.Price), typeof(decimal), "price");
        AssertJsonProperty(typeof(GetBoardLevel), nameof(GetBoardLevel.Size), typeof(decimal), "size");

        AssertJsonProperty(typeof(GetExecutionsPublicRequest), nameof(GetExecutionsPublicRequest.ProductCode), typeof(string), "product_code");
        AssertJsonProperty(typeof(GetExecutionsPublicRequest), nameof(GetExecutionsPublicRequest.Count), typeof(int?), "count");
        AssertJsonProperty(typeof(GetExecutionsPublicRequest), nameof(GetExecutionsPublicRequest.Before), typeof(long?), "before");
        AssertJsonProperty(typeof(GetExecutionsPublicRequest), nameof(GetExecutionsPublicRequest.After), typeof(long?), "after");
        AssertJsonProperty(typeof(GetExecutionsPublic.Item), nameof(GetExecutionsPublic.Item.Id), typeof(long), "id");
        AssertJsonProperty(typeof(GetExecutionsPublic.Item), nameof(GetExecutionsPublic.Item.Side), typeof(BitflyerOrderSide), "side");
        AssertJsonProperty(typeof(GetExecutionsPublic.Item), nameof(GetExecutionsPublic.Item.Price), typeof(decimal), "price");
        AssertJsonProperty(typeof(GetExecutionsPublic.Item), nameof(GetExecutionsPublic.Item.Size), typeof(decimal), "size");
        AssertJsonProperty(typeof(GetExecutionsPublic.Item), nameof(GetExecutionsPublic.Item.ExecDate), typeof(DateTimeOffset), "exec_date");
        AssertJsonProperty(typeof(GetExecutionsPublic.Item), nameof(GetExecutionsPublic.Item.BuyChildOrderAcceptanceId), typeof(string), "buy_child_order_acceptance_id");
        AssertJsonProperty(typeof(GetExecutionsPublic.Item), nameof(GetExecutionsPublic.Item.SellChildOrderAcceptanceId), typeof(string), "sell_child_order_acceptance_id");

        AssertJsonProperty(typeof(GetTickerRequest), nameof(GetTickerRequest.ProductCode), typeof(string), "product_code");
        AssertJsonIgnoreWhenWritingNull(typeof(GetTickerRequest), nameof(GetTickerRequest.ProductCode));
        AssertJsonProperty(typeof(GetTickerResponse), nameof(GetTickerResponse.ProductCode), typeof(string), "product_code");
        AssertJsonProperty(typeof(GetTickerResponse), nameof(GetTickerResponse.State), typeof(BitflyerTradingState), "state");
        AssertJsonProperty(typeof(GetTickerResponse), nameof(GetTickerResponse.Timestamp), typeof(DateTimeOffset), "timestamp");
        AssertJsonProperty(typeof(GetTickerResponse), nameof(GetTickerResponse.TickId), typeof(long), "tick_id");
        AssertJsonProperty(typeof(GetTickerResponse), nameof(GetTickerResponse.BestBid), typeof(decimal), "best_bid");
        AssertJsonProperty(typeof(GetTickerResponse), nameof(GetTickerResponse.BestAsk), typeof(decimal), "best_ask");
        AssertJsonProperty(typeof(GetTickerResponse), nameof(GetTickerResponse.BestBidSize), typeof(decimal), "best_bid_size");
        AssertJsonProperty(typeof(GetTickerResponse), nameof(GetTickerResponse.BestAskSize), typeof(decimal), "best_ask_size");
        AssertJsonProperty(typeof(GetTickerResponse), nameof(GetTickerResponse.TotalBidDepth), typeof(decimal), "total_bid_depth");
        AssertJsonProperty(typeof(GetTickerResponse), nameof(GetTickerResponse.TotalAskDepth), typeof(decimal), "total_ask_depth");
        AssertJsonProperty(typeof(GetTickerResponse), nameof(GetTickerResponse.MarketBidSize), typeof(decimal), "market_bid_size");
        AssertJsonProperty(typeof(GetTickerResponse), nameof(GetTickerResponse.MarketAskSize), typeof(decimal), "market_ask_size");
        AssertJsonProperty(typeof(GetTickerResponse), nameof(GetTickerResponse.Ltp), typeof(decimal), "ltp");
        AssertJsonProperty(typeof(GetTickerResponse), nameof(GetTickerResponse.Volume), typeof(decimal), "volume");
        AssertJsonProperty(typeof(GetTickerResponse), nameof(GetTickerResponse.VolumeByProduct), typeof(decimal), "volume_by_product");

        AssertJsonProperty(typeof(GetBalance.Item), nameof(GetBalance.Item.CurrencyCode), typeof(string), "currency_code");
        AssertJsonProperty(typeof(GetBalance.Item), nameof(GetBalance.Item.Amount), typeof(decimal), "amount");
        AssertJsonProperty(typeof(GetBalance.Item), nameof(GetBalance.Item.Available), typeof(decimal), "available");

        AssertJsonProperty(typeof(GetCollateralResponse), nameof(GetCollateralResponse.Collateral), typeof(decimal), "collateral");
        AssertJsonProperty(typeof(GetCollateralResponse), nameof(GetCollateralResponse.OpenPositionPnl), typeof(decimal), "open_position_pnl");
        AssertJsonProperty(typeof(GetCollateralResponse), nameof(GetCollateralResponse.RequireCollateral), typeof(decimal), "require_collateral");
        AssertJsonProperty(typeof(GetCollateralResponse), nameof(GetCollateralResponse.KeepRate), typeof(decimal), "keep_rate");
        AssertJsonProperty(typeof(GetCollateralResponse), nameof(GetCollateralResponse.MarginCallAmount), typeof(decimal?), "margin_call_amount");
        AssertJsonProperty(typeof(GetCollateralResponse), nameof(GetCollateralResponse.MarginCallDueDate), typeof(DateTimeOffset?), "margin_call_due_date");

        AssertJsonProperty(typeof(GetCollateralAccounts.Item), nameof(GetCollateralAccounts.Item.CurrencyCode), typeof(string), "currency_code");
        AssertJsonProperty(typeof(GetCollateralAccounts.Item), nameof(GetCollateralAccounts.Item.Amount), typeof(decimal), "amount");

        AssertProperty(typeof(GetBoardStateRequest), nameof(GetBoardStateRequest.ProductCode), typeof(string));
        AssertProperty(typeof(GetBoardStateResponse), nameof(GetBoardStateResponse.Health), typeof(BitflyerHealthStatus));
        AssertProperty(typeof(GetBoardStateResponse), nameof(GetBoardStateResponse.State), typeof(BitflyerTradingState));
        AssertProperty(typeof(GetBoardStateResponse), nameof(GetBoardStateResponse.Data), typeof(GetBoardStateData));
        AssertProperty(typeof(GetBoardStateData), nameof(GetBoardStateData.SpecialQuotation), typeof(decimal?));

        AssertProperty(typeof(GetHealthRequest), nameof(GetHealthRequest.ProductCode), typeof(string));
        AssertProperty(typeof(GetHealthResponse), nameof(GetHealthResponse.Status), typeof(BitflyerHealthStatus));

        AssertProperty(typeof(GetFundingRateRequest), nameof(GetFundingRateRequest.ProductCode), typeof(string));
        AssertProperty(typeof(GetFundingRateResponse), nameof(GetFundingRateResponse.CurrentFundingRate), typeof(decimal));
        AssertProperty(typeof(GetFundingRateResponse), nameof(GetFundingRateResponse.NextFundingRateSettleDate), typeof(DateTimeOffset));

        AssertProperty(typeof(GetCorporateLeverageResponse), nameof(GetCorporateLeverageResponse.CurrentMax), typeof(decimal));
        AssertProperty(typeof(GetCorporateLeverageResponse), nameof(GetCorporateLeverageResponse.CurrentStartDate), typeof(DateTimeOffset));
        AssertProperty(typeof(GetCorporateLeverageResponse), nameof(GetCorporateLeverageResponse.NextMax), typeof(decimal?));
        AssertProperty(typeof(GetCorporateLeverageResponse), nameof(GetCorporateLeverageResponse.NextStartDate), typeof(DateTimeOffset?));

        AssertProperty(typeof(GetChatsRequest), nameof(GetChatsRequest.FromDate), typeof(string));
        AssertProperty(typeof(GetChats.Item), nameof(GetChats.Item.Nickname), typeof(string));
        AssertProperty(typeof(GetChats.Item), nameof(GetChats.Item.Message), typeof(string));
        AssertProperty(typeof(GetChats.Item), nameof(GetChats.Item.Date), typeof(DateTimeOffset));

        AssertProperty(typeof(GetAddresses.Item), nameof(GetAddresses.Item.Type), typeof(BitflyerAddressType));
        AssertProperty(typeof(GetAddresses.Item), nameof(GetAddresses.Item.CurrencyCode), typeof(string));
        AssertProperty(typeof(GetAddresses.Item), nameof(GetAddresses.Item.Address), typeof(string));

        AssertProperty(typeof(GetBankAccounts.Item), nameof(GetBankAccounts.Item.Id), typeof(long));
        AssertProperty(typeof(GetBankAccounts.Item), nameof(GetBankAccounts.Item.IsVerified), typeof(bool));
        AssertProperty(typeof(GetBankAccounts.Item), nameof(GetBankAccounts.Item.BankName), typeof(string));
        AssertProperty(typeof(GetBankAccounts.Item), nameof(GetBankAccounts.Item.BranchName), typeof(string));
        AssertProperty(typeof(GetBankAccounts.Item), nameof(GetBankAccounts.Item.AccountType), typeof(string));
        AssertProperty(typeof(GetBankAccounts.Item), nameof(GetBankAccounts.Item.AccountNumber), typeof(string));
        AssertProperty(typeof(GetBankAccounts.Item), nameof(GetBankAccounts.Item.AccountName), typeof(string));

        AssertEmptyRequest(typeof(GetPermissionsRequest));
        AssertCallMethod(
            typeof(IBitflyerPrivateNativeApi),
            nameof(IBitflyerPrivateNativeApi.GetPermissionsCallAsync),
            typeof(GetPermissionsRequest),
            typeof(IReadOnlyList<string>));

        AssertProperty(typeof(GetCoinInsRequest), nameof(GetCoinInsRequest.Count), typeof(int?));
        AssertProperty(typeof(GetCoinInsRequest), nameof(GetCoinInsRequest.Before), typeof(long?));
        AssertProperty(typeof(GetCoinInsRequest), nameof(GetCoinInsRequest.After), typeof(long?));
        AssertProperty(typeof(GetCoinIns.Item), nameof(GetCoinIns.Item.Id), typeof(long));
        AssertProperty(typeof(GetCoinIns.Item), nameof(GetCoinIns.Item.OrderId), typeof(string));
        AssertProperty(typeof(GetCoinIns.Item), nameof(GetCoinIns.Item.CurrencyCode), typeof(string));
        AssertProperty(typeof(GetCoinIns.Item), nameof(GetCoinIns.Item.Amount), typeof(decimal));
        AssertProperty(typeof(GetCoinIns.Item), nameof(GetCoinIns.Item.Address), typeof(string));
        AssertProperty(typeof(GetCoinIns.Item), nameof(GetCoinIns.Item.TxHash), typeof(string));
        AssertProperty(typeof(GetCoinIns.Item), nameof(GetCoinIns.Item.Status), typeof(BitflyerTransferStatus));
        AssertProperty(typeof(GetCoinIns.Item), nameof(GetCoinIns.Item.EventDate), typeof(DateTimeOffset));

        AssertProperty(typeof(GetCoinOutsRequest), nameof(GetCoinOutsRequest.Count), typeof(int?));
        AssertProperty(typeof(GetCoinOutsRequest), nameof(GetCoinOutsRequest.Before), typeof(long?));
        AssertProperty(typeof(GetCoinOutsRequest), nameof(GetCoinOutsRequest.After), typeof(long?));
        AssertProperty(typeof(GetCoinOuts.Item), nameof(GetCoinOuts.Item.Id), typeof(long));
        AssertProperty(typeof(GetCoinOuts.Item), nameof(GetCoinOuts.Item.OrderId), typeof(string));
        AssertProperty(typeof(GetCoinOuts.Item), nameof(GetCoinOuts.Item.CurrencyCode), typeof(string));
        AssertProperty(typeof(GetCoinOuts.Item), nameof(GetCoinOuts.Item.Amount), typeof(decimal));
        AssertProperty(typeof(GetCoinOuts.Item), nameof(GetCoinOuts.Item.Address), typeof(string));
        AssertProperty(typeof(GetCoinOuts.Item), nameof(GetCoinOuts.Item.TxHash), typeof(string));
        AssertProperty(typeof(GetCoinOuts.Item), nameof(GetCoinOuts.Item.Fee), typeof(decimal));
        AssertProperty(typeof(GetCoinOuts.Item), nameof(GetCoinOuts.Item.AdditionalFee), typeof(decimal));
        AssertProperty(typeof(GetCoinOuts.Item), nameof(GetCoinOuts.Item.Status), typeof(BitflyerTransferStatus));
        AssertProperty(typeof(GetCoinOuts.Item), nameof(GetCoinOuts.Item.EventDate), typeof(DateTimeOffset));

        AssertProperty(typeof(GetDepositsRequest), nameof(GetDepositsRequest.Count), typeof(int?));
        AssertProperty(typeof(GetDepositsRequest), nameof(GetDepositsRequest.Before), typeof(long?));
        AssertProperty(typeof(GetDepositsRequest), nameof(GetDepositsRequest.After), typeof(long?));
        AssertProperty(typeof(GetDeposits.Item), nameof(GetDeposits.Item.Id), typeof(long));
        AssertProperty(typeof(GetDeposits.Item), nameof(GetDeposits.Item.OrderId), typeof(string));
        AssertProperty(typeof(GetDeposits.Item), nameof(GetDeposits.Item.CurrencyCode), typeof(string));
        AssertProperty(typeof(GetDeposits.Item), nameof(GetDeposits.Item.Amount), typeof(decimal));
        AssertProperty(typeof(GetDeposits.Item), nameof(GetDeposits.Item.Status), typeof(BitflyerTransferStatus));
        AssertProperty(typeof(GetDeposits.Item), nameof(GetDeposits.Item.EventDate), typeof(DateTimeOffset));

        AssertJsonProperty(typeof(WithdrawRequest), nameof(WithdrawRequest.CurrencyCode), typeof(string), "currency_code");
        AssertJsonProperty(typeof(WithdrawRequest), nameof(WithdrawRequest.BankAccountId), typeof(long), "bank_account_id");
        AssertJsonProperty(typeof(WithdrawRequest), nameof(WithdrawRequest.Amount), typeof(decimal), "amount");
        AssertJsonProperty(typeof(WithdrawRequest), nameof(WithdrawRequest.Code), typeof(string), "code");
        AssertProperty(typeof(WithdrawResponse), nameof(WithdrawResponse.MessageId), typeof(string));
        AssertCallMethod(
            typeof(IBitflyerPrivateNativeApi),
            nameof(IBitflyerPrivateNativeApi.WithdrawCallAsync),
            typeof(WithdrawRequest),
            typeof(WithdrawResponse));

        AssertProperty(typeof(GetWithdrawalsRequest), nameof(GetWithdrawalsRequest.Count), typeof(int?));
        AssertProperty(typeof(GetWithdrawalsRequest), nameof(GetWithdrawalsRequest.Before), typeof(long?));
        AssertProperty(typeof(GetWithdrawalsRequest), nameof(GetWithdrawalsRequest.After), typeof(long?));
        AssertProperty(typeof(GetWithdrawalsRequest), nameof(GetWithdrawalsRequest.MessageId), typeof(string));
        AssertProperty(typeof(GetWithdrawals.Item), nameof(GetWithdrawals.Item.Id), typeof(long));
        AssertProperty(typeof(GetWithdrawals.Item), nameof(GetWithdrawals.Item.OrderId), typeof(string));
        AssertProperty(typeof(GetWithdrawals.Item), nameof(GetWithdrawals.Item.CurrencyCode), typeof(string));
        AssertProperty(typeof(GetWithdrawals.Item), nameof(GetWithdrawals.Item.Amount), typeof(decimal));
        AssertProperty(typeof(GetWithdrawals.Item), nameof(GetWithdrawals.Item.Status), typeof(BitflyerTransferStatus));
        AssertProperty(typeof(GetWithdrawals.Item), nameof(GetWithdrawals.Item.EventDate), typeof(DateTimeOffset));

        AssertProperty(typeof(GetParentOrdersRequest), nameof(GetParentOrdersRequest.ProductCode), typeof(string));
        AssertProperty(typeof(GetParentOrdersRequest), nameof(GetParentOrdersRequest.Count), typeof(int?));
        AssertProperty(typeof(GetParentOrdersRequest), nameof(GetParentOrdersRequest.Before), typeof(long?));
        AssertProperty(typeof(GetParentOrdersRequest), nameof(GetParentOrdersRequest.After), typeof(long?));
        AssertProperty(typeof(GetParentOrdersRequest), nameof(GetParentOrdersRequest.ParentOrderState), typeof(BitflyerOrderState?));
        AssertProperty(typeof(GetParentOrders.Item), nameof(GetParentOrders.Item.Id), typeof(long));
        AssertProperty(typeof(GetParentOrders.Item), nameof(GetParentOrders.Item.ParentOrderId), typeof(string));
        AssertProperty(typeof(GetParentOrders.Item), nameof(GetParentOrders.Item.ProductCode), typeof(string));
        AssertProperty(typeof(GetParentOrders.Item), nameof(GetParentOrders.Item.Side), typeof(BitflyerParentOrderSide));
        AssertProperty(typeof(GetParentOrders.Item), nameof(GetParentOrders.Item.ParentOrderType), typeof(BitflyerParentOrderType));
        AssertProperty(typeof(GetParentOrders.Item), nameof(GetParentOrders.Item.Price), typeof(decimal));
        AssertProperty(typeof(GetParentOrders.Item), nameof(GetParentOrders.Item.AveragePrice), typeof(decimal));
        AssertProperty(typeof(GetParentOrders.Item), nameof(GetParentOrders.Item.Size), typeof(decimal));
        AssertProperty(typeof(GetParentOrders.Item), nameof(GetParentOrders.Item.ParentOrderState), typeof(BitflyerOrderState));
        AssertProperty(typeof(GetParentOrders.Item), nameof(GetParentOrders.Item.ExpireDate), typeof(DateTimeOffset));
        AssertProperty(typeof(GetParentOrders.Item), nameof(GetParentOrders.Item.ParentOrderDate), typeof(DateTimeOffset));
        AssertProperty(typeof(GetParentOrders.Item), nameof(GetParentOrders.Item.ParentOrderAcceptanceId), typeof(string));
        AssertProperty(typeof(GetParentOrders.Item), nameof(GetParentOrders.Item.OutstandingSize), typeof(decimal));
        AssertProperty(typeof(GetParentOrders.Item), nameof(GetParentOrders.Item.CancelSize), typeof(decimal));
        AssertProperty(typeof(GetParentOrders.Item), nameof(GetParentOrders.Item.ExecutedSize), typeof(decimal));
        AssertProperty(typeof(GetParentOrders.Item), nameof(GetParentOrders.Item.TotalCommission), typeof(decimal));

        AssertProperty(typeof(GetBalanceHistoryRequest), nameof(GetBalanceHistoryRequest.CurrencyCode), typeof(string));
        AssertProperty(typeof(GetBalanceHistoryRequest), nameof(GetBalanceHistoryRequest.Count), typeof(int?));
        AssertProperty(typeof(GetBalanceHistoryRequest), nameof(GetBalanceHistoryRequest.Before), typeof(long?));
        AssertProperty(typeof(GetBalanceHistoryRequest), nameof(GetBalanceHistoryRequest.After), typeof(long?));
        AssertProperty(typeof(GetBalanceHistory.Item), nameof(GetBalanceHistory.Item.Id), typeof(long));
        AssertProperty(typeof(GetBalanceHistory.Item), nameof(GetBalanceHistory.Item.TradeDate), typeof(DateTimeOffset));
        AssertProperty(typeof(GetBalanceHistory.Item), nameof(GetBalanceHistory.Item.EventDate), typeof(DateTimeOffset));
        AssertProperty(typeof(GetBalanceHistory.Item), nameof(GetBalanceHistory.Item.ProductCode), typeof(string));
        AssertProperty(typeof(GetBalanceHistory.Item), nameof(GetBalanceHistory.Item.CurrencyCode), typeof(string));
        AssertProperty(typeof(GetBalanceHistory.Item), nameof(GetBalanceHistory.Item.TradeType), typeof(BitflyerTradeType));
        AssertProperty(typeof(GetBalanceHistory.Item), nameof(GetBalanceHistory.Item.Price), typeof(decimal));
        AssertProperty(typeof(GetBalanceHistory.Item), nameof(GetBalanceHistory.Item.Amount), typeof(decimal));
        AssertProperty(typeof(GetBalanceHistory.Item), nameof(GetBalanceHistory.Item.Quantity), typeof(decimal));
        AssertProperty(typeof(GetBalanceHistory.Item), nameof(GetBalanceHistory.Item.Commission), typeof(decimal));
        AssertProperty(typeof(GetBalanceHistory.Item), nameof(GetBalanceHistory.Item.Balance), typeof(decimal));
        AssertProperty(typeof(GetBalanceHistory.Item), nameof(GetBalanceHistory.Item.OrderId), typeof(string));

        AssertProperty(typeof(GetPositionsRequest), nameof(GetPositionsRequest.ProductCode), typeof(string));
        AssertJsonProperty(typeof(GetPositionsRequest), nameof(GetPositionsRequest.ProductCode), typeof(string), "product_code");
        AssertJsonProperty(typeof(GetPositions.Item), nameof(GetPositions.Item.ProductCode), typeof(string), "product_code");
        AssertJsonProperty(typeof(GetPositions.Item), nameof(GetPositions.Item.Side), typeof(BitflyerOrderSide), "side");
        AssertJsonProperty(typeof(GetPositions.Item), nameof(GetPositions.Item.Price), typeof(decimal), "price");
        AssertJsonProperty(typeof(GetPositions.Item), nameof(GetPositions.Item.Size), typeof(decimal), "size");
        AssertJsonProperty(typeof(GetPositions.Item), nameof(GetPositions.Item.Commission), typeof(decimal), "commission");
        AssertJsonProperty(typeof(GetPositions.Item), nameof(GetPositions.Item.SwapPointAccumulate), typeof(decimal), "swap_point_accumulate");
        AssertJsonProperty(typeof(GetPositions.Item), nameof(GetPositions.Item.RequireCollateral), typeof(decimal), "require_collateral");
        AssertJsonProperty(typeof(GetPositions.Item), nameof(GetPositions.Item.OpenDate), typeof(DateTimeOffset), "open_date");
        AssertJsonProperty(typeof(GetPositions.Item), nameof(GetPositions.Item.Leverage), typeof(decimal), "leverage");
        AssertJsonProperty(typeof(GetPositions.Item), nameof(GetPositions.Item.Pnl), typeof(decimal), "pnl");
        AssertJsonProperty(typeof(GetPositions.Item), nameof(GetPositions.Item.Sfd), typeof(decimal), "sfd");

        AssertJsonProperty(typeof(GetCollateralHistoryRequest), nameof(GetCollateralHistoryRequest.Count), typeof(int?), "count");
        AssertJsonProperty(typeof(GetCollateralHistoryRequest), nameof(GetCollateralHistoryRequest.Before), typeof(long?), "before");
        AssertJsonProperty(typeof(GetCollateralHistoryRequest), nameof(GetCollateralHistoryRequest.After), typeof(long?), "after");
        AssertJsonProperty(typeof(GetCollateralHistory.Item), nameof(GetCollateralHistory.Item.Id), typeof(long), "id");
        AssertJsonProperty(typeof(GetCollateralHistory.Item), nameof(GetCollateralHistory.Item.CurrencyCode), typeof(string), "currency_code");
        AssertJsonProperty(typeof(GetCollateralHistory.Item), nameof(GetCollateralHistory.Item.Change), typeof(decimal), "change");
        AssertJsonProperty(typeof(GetCollateralHistory.Item), nameof(GetCollateralHistory.Item.Amount), typeof(decimal), "amount");
        AssertJsonProperty(typeof(GetCollateralHistory.Item), nameof(GetCollateralHistory.Item.ReasonCode), typeof(string), "reason_code");
        AssertJsonProperty(typeof(GetCollateralHistory.Item), nameof(GetCollateralHistory.Item.Date), typeof(DateTimeOffset), "date");

        AssertJsonProperty(typeof(GetChildOrdersRequest), nameof(GetChildOrdersRequest.ProductCode), typeof(string), "product_code");
        AssertJsonProperty(typeof(GetChildOrdersRequest), nameof(GetChildOrdersRequest.Count), typeof(int?), "count");
        AssertJsonProperty(typeof(GetChildOrdersRequest), nameof(GetChildOrdersRequest.Before), typeof(long?), "before");
        AssertJsonProperty(typeof(GetChildOrdersRequest), nameof(GetChildOrdersRequest.After), typeof(long?), "after");
        AssertJsonProperty(typeof(GetChildOrdersRequest), nameof(GetChildOrdersRequest.ChildOrderState), typeof(BitflyerOrderState?), "child_order_state");
        AssertJsonProperty(typeof(GetChildOrdersRequest), nameof(GetChildOrdersRequest.ChildOrderId), typeof(string), "child_order_id");
        AssertJsonProperty(typeof(GetChildOrdersRequest), nameof(GetChildOrdersRequest.ChildOrderAcceptanceId), typeof(string), "child_order_acceptance_id");
        AssertJsonProperty(typeof(GetChildOrdersRequest), nameof(GetChildOrdersRequest.ParentOrderId), typeof(string), "parent_order_id");
        AssertJsonProperty(typeof(GetChildOrders.Item), nameof(GetChildOrders.Item.Id), typeof(long), "id");
        AssertJsonProperty(typeof(GetChildOrders.Item), nameof(GetChildOrders.Item.ChildOrderId), typeof(string), "child_order_id");
        AssertJsonProperty(typeof(GetChildOrders.Item), nameof(GetChildOrders.Item.ProductCode), typeof(string), "product_code");
        AssertJsonProperty(typeof(GetChildOrders.Item), nameof(GetChildOrders.Item.Side), typeof(BitflyerOrderSide), "side");
        AssertJsonProperty(typeof(GetChildOrders.Item), nameof(GetChildOrders.Item.ChildOrderType), typeof(BitflyerChildOrderType), "child_order_type");
        AssertJsonProperty(typeof(GetChildOrders.Item), nameof(GetChildOrders.Item.Price), typeof(decimal), "price");
        AssertJsonProperty(typeof(GetChildOrders.Item), nameof(GetChildOrders.Item.AveragePrice), typeof(decimal), "average_price");
        AssertJsonProperty(typeof(GetChildOrders.Item), nameof(GetChildOrders.Item.Size), typeof(decimal), "size");
        AssertJsonProperty(typeof(GetChildOrders.Item), nameof(GetChildOrders.Item.ChildOrderState), typeof(BitflyerOrderState), "child_order_state");
        AssertJsonProperty(typeof(GetChildOrders.Item), nameof(GetChildOrders.Item.ExpireDate), typeof(DateTimeOffset), "expire_date");
        AssertJsonProperty(typeof(GetChildOrders.Item), nameof(GetChildOrders.Item.ChildOrderDate), typeof(DateTimeOffset), "child_order_date");
        AssertJsonProperty(typeof(GetChildOrders.Item), nameof(GetChildOrders.Item.ChildOrderAcceptanceId), typeof(string), "child_order_acceptance_id");
        AssertJsonProperty(typeof(GetChildOrders.Item), nameof(GetChildOrders.Item.OutstandingSize), typeof(decimal), "outstanding_size");
        AssertJsonProperty(typeof(GetChildOrders.Item), nameof(GetChildOrders.Item.CancelSize), typeof(decimal), "cancel_size");
        AssertJsonProperty(typeof(GetChildOrders.Item), nameof(GetChildOrders.Item.ExecutedSize), typeof(decimal), "executed_size");
        AssertJsonProperty(typeof(GetChildOrders.Item), nameof(GetChildOrders.Item.TotalCommission), typeof(decimal), "total_commission");
        AssertJsonProperty(typeof(GetChildOrders.Item), nameof(GetChildOrders.Item.TimeInForce), typeof(BitflyerTimeInForce), "time_in_force");

        AssertJsonProperty(typeof(GetExecutionsRequest), nameof(GetExecutionsRequest.ProductCode), typeof(string), "product_code");
        AssertJsonProperty(typeof(GetExecutionsRequest), nameof(GetExecutionsRequest.Count), typeof(int?), "count");
        AssertJsonProperty(typeof(GetExecutionsRequest), nameof(GetExecutionsRequest.Before), typeof(long?), "before");
        AssertJsonProperty(typeof(GetExecutionsRequest), nameof(GetExecutionsRequest.After), typeof(long?), "after");
        AssertJsonProperty(typeof(GetExecutionsRequest), nameof(GetExecutionsRequest.ChildOrderId), typeof(string), "child_order_id");
        AssertJsonProperty(typeof(GetExecutionsRequest), nameof(GetExecutionsRequest.ChildOrderAcceptanceId), typeof(string), "child_order_acceptance_id");
        AssertJsonProperty(typeof(GetExecutions.Item), nameof(GetExecutions.Item.Id), typeof(long), "id");
        AssertJsonProperty(typeof(GetExecutions.Item), nameof(GetExecutions.Item.ChildOrderId), typeof(string), "child_order_id");
        AssertJsonProperty(typeof(GetExecutions.Item), nameof(GetExecutions.Item.Side), typeof(BitflyerOrderSide), "side");
        AssertJsonProperty(typeof(GetExecutions.Item), nameof(GetExecutions.Item.Price), typeof(decimal), "price");
        AssertJsonProperty(typeof(GetExecutions.Item), nameof(GetExecutions.Item.Size), typeof(decimal), "size");
        AssertJsonProperty(typeof(GetExecutions.Item), nameof(GetExecutions.Item.Commission), typeof(decimal), "commission");
        AssertJsonProperty(typeof(GetExecutions.Item), nameof(GetExecutions.Item.ExecDate), typeof(DateTimeOffset), "exec_date");
        AssertJsonProperty(typeof(GetExecutions.Item), nameof(GetExecutions.Item.ChildOrderAcceptanceId), typeof(string), "child_order_acceptance_id");

        AssertJsonProperty(typeof(GetTradingCommissionRequest), nameof(GetTradingCommissionRequest.ProductCode), typeof(string), "product_code");
        AssertJsonProperty(typeof(GetTradingCommissionResponse), nameof(GetTradingCommissionResponse.CommissionRate), typeof(decimal), "commission_rate");

        AssertJsonProperty(typeof(SendChildOrderRequest), nameof(SendChildOrderRequest.ProductCode), typeof(string), "product_code");
        AssertJsonProperty(typeof(SendChildOrderRequest), nameof(SendChildOrderRequest.ChildOrderType), typeof(BitflyerChildOrderType), "child_order_type");
        AssertJsonProperty(typeof(SendChildOrderRequest), nameof(SendChildOrderRequest.Side), typeof(BitflyerOrderSide), "side");
        AssertJsonProperty(typeof(SendChildOrderRequest), nameof(SendChildOrderRequest.Price), typeof(decimal?), "price");
        AssertJsonProperty(typeof(SendChildOrderRequest), nameof(SendChildOrderRequest.Size), typeof(decimal), "size");
        AssertJsonProperty(typeof(SendChildOrderRequest), nameof(SendChildOrderRequest.MinuteToExpire), typeof(int?), "minute_to_expire");
        AssertJsonProperty(typeof(SendChildOrderRequest), nameof(SendChildOrderRequest.TimeInForce), typeof(BitflyerTimeInForce?), "time_in_force");
        AssertJsonIgnoreWhenWritingNull(typeof(SendChildOrderRequest), nameof(SendChildOrderRequest.Price));
        AssertJsonIgnoreWhenWritingNull(typeof(SendChildOrderRequest), nameof(SendChildOrderRequest.MinuteToExpire));
        AssertJsonIgnoreWhenWritingNull(typeof(SendChildOrderRequest), nameof(SendChildOrderRequest.TimeInForce));
        AssertJsonProperty(typeof(SendChildOrderResponse), nameof(SendChildOrderResponse.ChildOrderAcceptanceId), typeof(string), "child_order_acceptance_id");
        AssertCallMethod(
            typeof(IBitflyerPrivateNativeApi),
            nameof(IBitflyerPrivateNativeApi.SendChildOrderCallAsync),
            typeof(SendChildOrderRequest),
            typeof(SendChildOrderResponse));

        AssertJsonProperty(typeof(CancelChildOrderRequest), nameof(CancelChildOrderRequest.ProductCode), typeof(string), "product_code");
        AssertJsonProperty(typeof(CancelChildOrderRequest), nameof(CancelChildOrderRequest.ChildOrderId), typeof(string), "child_order_id");
        AssertJsonProperty(typeof(CancelChildOrderRequest), nameof(CancelChildOrderRequest.ChildOrderAcceptanceId), typeof(string), "child_order_acceptance_id");
        AssertJsonIgnoreWhenWritingNull(typeof(CancelChildOrderRequest), nameof(CancelChildOrderRequest.ChildOrderId));
        AssertJsonIgnoreWhenWritingNull(typeof(CancelChildOrderRequest), nameof(CancelChildOrderRequest.ChildOrderAcceptanceId));
        AssertCallMethod(
            typeof(IBitflyerPrivateNativeApi),
            nameof(IBitflyerPrivateNativeApi.CancelChildOrderCallAsync),
            typeof(CancelChildOrderRequest),
            typeof(Unit));

        AssertJsonProperty(typeof(SendParentOrderRequest), nameof(SendParentOrderRequest.OrderMethod), typeof(BitflyerOrderMethod?), "order_method");
        AssertJsonProperty(typeof(SendParentOrderRequest), nameof(SendParentOrderRequest.MinuteToExpire), typeof(int?), "minute_to_expire");
        AssertJsonProperty(typeof(SendParentOrderRequest), nameof(SendParentOrderRequest.TimeInForce), typeof(BitflyerTimeInForce?), "time_in_force");
        AssertJsonProperty(typeof(SendParentOrderRequest), nameof(SendParentOrderRequest.Parameters), typeof(IReadOnlyList<SendParentOrderParameter>), "parameters");
        AssertJsonIgnoreWhenWritingNull(typeof(SendParentOrderRequest), nameof(SendParentOrderRequest.OrderMethod));
        AssertJsonIgnoreWhenWritingNull(typeof(SendParentOrderRequest), nameof(SendParentOrderRequest.MinuteToExpire));
        AssertJsonIgnoreWhenWritingNull(typeof(SendParentOrderRequest), nameof(SendParentOrderRequest.TimeInForce));
        AssertJsonProperty(typeof(SendParentOrderParameter), nameof(SendParentOrderParameter.ProductCode), typeof(string), "product_code");
        AssertJsonProperty(typeof(SendParentOrderParameter), nameof(SendParentOrderParameter.ConditionType), typeof(BitflyerConditionType), "condition_type");
        AssertJsonProperty(typeof(SendParentOrderParameter), nameof(SendParentOrderParameter.Side), typeof(BitflyerOrderSide), "side");
        AssertJsonProperty(typeof(SendParentOrderParameter), nameof(SendParentOrderParameter.Price), typeof(decimal?), "price");
        AssertJsonProperty(typeof(SendParentOrderParameter), nameof(SendParentOrderParameter.Size), typeof(decimal), "size");
        AssertJsonProperty(typeof(SendParentOrderParameter), nameof(SendParentOrderParameter.TriggerPrice), typeof(decimal?), "trigger_price");
        AssertJsonProperty(typeof(SendParentOrderParameter), nameof(SendParentOrderParameter.Offset), typeof(long?), "offset");
        AssertJsonIgnoreWhenWritingNull(typeof(SendParentOrderParameter), nameof(SendParentOrderParameter.Price));
        AssertJsonIgnoreWhenWritingNull(typeof(SendParentOrderParameter), nameof(SendParentOrderParameter.TriggerPrice));
        AssertJsonIgnoreWhenWritingNull(typeof(SendParentOrderParameter), nameof(SendParentOrderParameter.Offset));
        AssertProperty(typeof(SendParentOrderResponse), nameof(SendParentOrderResponse.ParentOrderAcceptanceId), typeof(string));
        AssertCallMethod(
            typeof(IBitflyerPrivateNativeApi),
            nameof(IBitflyerPrivateNativeApi.SendParentOrderCallAsync),
            typeof(SendParentOrderRequest),
            typeof(SendParentOrderResponse));

        AssertProperty(typeof(GetParentOrderRequest), nameof(GetParentOrderRequest.ParentOrderId), typeof(string));
        AssertProperty(typeof(GetParentOrderRequest), nameof(GetParentOrderRequest.ParentOrderAcceptanceId), typeof(string));
        AssertProperty(typeof(GetParentOrderResponse), nameof(GetParentOrderResponse.Id), typeof(long));
        AssertProperty(typeof(GetParentOrderResponse), nameof(GetParentOrderResponse.ParentOrderId), typeof(string));
        AssertProperty(typeof(GetParentOrderResponse), nameof(GetParentOrderResponse.OrderMethod), typeof(BitflyerOrderMethod));
        AssertProperty(typeof(GetParentOrderResponse), nameof(GetParentOrderResponse.ExpireDate), typeof(DateTimeOffset));
        AssertProperty(typeof(GetParentOrderResponse), nameof(GetParentOrderResponse.TimeInForce), typeof(BitflyerTimeInForce));
        AssertProperty(typeof(GetParentOrderResponse), nameof(GetParentOrderResponse.Parameters), typeof(IReadOnlyList<GetParentOrderParameter>));
        AssertProperty(typeof(GetParentOrderResponse), nameof(GetParentOrderResponse.ParentOrderAcceptanceId), typeof(string));
        AssertProperty(typeof(GetParentOrderParameter), nameof(GetParentOrderParameter.ProductCode), typeof(string));
        AssertProperty(typeof(GetParentOrderParameter), nameof(GetParentOrderParameter.ConditionType), typeof(BitflyerConditionType));
        AssertProperty(typeof(GetParentOrderParameter), nameof(GetParentOrderParameter.Side), typeof(BitflyerOrderSide));
        AssertProperty(typeof(GetParentOrderParameter), nameof(GetParentOrderParameter.Price), typeof(decimal));
        AssertProperty(typeof(GetParentOrderParameter), nameof(GetParentOrderParameter.Size), typeof(decimal));
        AssertProperty(typeof(GetParentOrderParameter), nameof(GetParentOrderParameter.TriggerPrice), typeof(decimal));
        AssertProperty(typeof(GetParentOrderParameter), nameof(GetParentOrderParameter.Offset), typeof(decimal));
        AssertCallMethod(
            typeof(IBitflyerPrivateNativeApi),
            nameof(IBitflyerPrivateNativeApi.GetParentOrderCallAsync),
            typeof(GetParentOrderRequest),
            typeof(GetParentOrderResponse));

        AssertJsonProperty(typeof(CancelParentOrderRequest), nameof(CancelParentOrderRequest.ProductCode), typeof(string), "product_code");
        AssertJsonProperty(typeof(CancelParentOrderRequest), nameof(CancelParentOrderRequest.ParentOrderId), typeof(string), "parent_order_id");
        AssertJsonProperty(typeof(CancelParentOrderRequest), nameof(CancelParentOrderRequest.ParentOrderAcceptanceId), typeof(string), "parent_order_acceptance_id");
        AssertJsonIgnoreWhenWritingNull(typeof(CancelParentOrderRequest), nameof(CancelParentOrderRequest.ParentOrderId));
        AssertJsonIgnoreWhenWritingNull(typeof(CancelParentOrderRequest), nameof(CancelParentOrderRequest.ParentOrderAcceptanceId));
        AssertCallMethod(
            typeof(IBitflyerPrivateNativeApi),
            nameof(IBitflyerPrivateNativeApi.CancelParentOrderCallAsync),
            typeof(CancelParentOrderRequest),
            typeof(Unit));

        AssertJsonProperty(typeof(CancelAllChildOrdersRequest), nameof(CancelAllChildOrdersRequest.ProductCode), typeof(string), "product_code");
        AssertCallMethod(
            typeof(IBitflyerPrivateNativeApi),
            nameof(IBitflyerPrivateNativeApi.CancelAllChildOrdersCallAsync),
            typeof(CancelAllChildOrdersRequest),
            typeof(Unit));
    }

    private static void AssertJsonProperty(Type type, string propertyName, Type propertyType, string jsonPropertyName)
    {
        var property = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        Assert.NotNull(property);
        Assert.Equal(propertyType, property!.PropertyType);

        var attribute = property.GetCustomAttribute<JsonPropertyNameAttribute>();
        Assert.NotNull(attribute);
        Assert.Equal(jsonPropertyName, attribute!.Name);
    }

    private static void AssertJsonIgnoreWhenWritingNull(Type type, string propertyName)
    {
        var property = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        Assert.NotNull(property);

        var attribute = property!.GetCustomAttribute<JsonIgnoreAttribute>();
        Assert.NotNull(attribute);
        Assert.Equal(JsonIgnoreCondition.WhenWritingNull, attribute!.Condition);
    }

    private static void AssertProperty(Type type, string propertyName, Type propertyType)
    {
        var property = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        Assert.NotNull(property);
        Assert.Equal(propertyType, property!.PropertyType);
    }

    private static void AssertEmptyRequest(Type type)
    {
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        Assert.Empty(properties);
    }

    private static void AssertCallMethod(Type type, string methodName, Type requestType, Type responseType)
    {
        var method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);
        Assert.NotNull(method);

        var parameters = method!.GetParameters();
        Assert.Equal(2, parameters.Length);
        Assert.Equal(requestType, parameters[0].ParameterType);
        Assert.Equal(typeof(CancellationToken), parameters[1].ParameterType);

        var expectedCallType = typeof(Call<,>).MakeGenericType(requestType, responseType);
        var expectedReturnType = typeof(Task<>).MakeGenericType(expectedCallType);
        Assert.Equal(expectedReturnType, method.ReturnType);
    }
}
