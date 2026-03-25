using System.Reflection;
using System.Text.Json.Serialization;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetAddresses;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetBalance;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetBankAccounts;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCollateral;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCollateralAccounts;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetTradingCommission;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetBoard;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetBoardState;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetChats;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetCorporateLeverage;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetExecutionsPublic;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetFundingRate;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetHealth;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetMarkets;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetTicker;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Native.Tests;

public sealed class FixedContractTests
{
    [Fact]
    public void Fixed_Request_And_Response_Dtos_KeepKnown_JsonPropertyNames()
    {
        AssertJsonProperty(typeof(GetMarkets.Item), nameof(GetMarkets.Item.ProductCode), typeof(string), "product_code");
        AssertJsonProperty(typeof(GetMarkets.Item), nameof(GetMarkets.Item.MarketType), typeof(string), "market_type");

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
        AssertJsonProperty(typeof(GetExecutionsPublic.Item), nameof(GetExecutionsPublic.Item.Side), typeof(string), "side");
        AssertJsonProperty(typeof(GetExecutionsPublic.Item), nameof(GetExecutionsPublic.Item.Price), typeof(decimal), "price");
        AssertJsonProperty(typeof(GetExecutionsPublic.Item), nameof(GetExecutionsPublic.Item.Size), typeof(decimal), "size");
        AssertJsonProperty(typeof(GetExecutionsPublic.Item), nameof(GetExecutionsPublic.Item.ExecDate), typeof(DateTimeOffset), "exec_date");
        AssertJsonProperty(typeof(GetExecutionsPublic.Item), nameof(GetExecutionsPublic.Item.BuyChildOrderAcceptanceId), typeof(string), "buy_child_order_acceptance_id");
        AssertJsonProperty(typeof(GetExecutionsPublic.Item), nameof(GetExecutionsPublic.Item.SellChildOrderAcceptanceId), typeof(string), "sell_child_order_acceptance_id");

        AssertJsonProperty(typeof(GetTickerRequest), nameof(GetTickerRequest.ProductCode), typeof(string), "product_code");
        AssertJsonIgnoreWhenWritingNull(typeof(GetTickerRequest), nameof(GetTickerRequest.ProductCode));
        AssertJsonProperty(typeof(GetTickerResponse), nameof(GetTickerResponse.ProductCode), typeof(string), "product_code");
        AssertJsonProperty(typeof(GetTickerResponse), nameof(GetTickerResponse.State), typeof(string), "state");
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
        AssertProperty(typeof(GetBoardStateResponse), nameof(GetBoardStateResponse.Health), typeof(string));
        AssertProperty(typeof(GetBoardStateResponse), nameof(GetBoardStateResponse.State), typeof(string));
        AssertProperty(typeof(GetBoardStateResponse), nameof(GetBoardStateResponse.Data), typeof(GetBoardStateData));
        AssertProperty(typeof(GetBoardStateData), nameof(GetBoardStateData.SpecialQuotation), typeof(decimal?));

        AssertProperty(typeof(GetHealthRequest), nameof(GetHealthRequest.ProductCode), typeof(string));
        AssertProperty(typeof(GetHealthResponse), nameof(GetHealthResponse.Status), typeof(string));

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

        AssertProperty(typeof(GetAddresses.Item), nameof(GetAddresses.Item.Type), typeof(string));
        AssertProperty(typeof(GetAddresses.Item), nameof(GetAddresses.Item.CurrencyCode), typeof(string));
        AssertProperty(typeof(GetAddresses.Item), nameof(GetAddresses.Item.Address), typeof(string));

        AssertProperty(typeof(GetBankAccounts.Item), nameof(GetBankAccounts.Item.Id), typeof(long));
        AssertProperty(typeof(GetBankAccounts.Item), nameof(GetBankAccounts.Item.IsVerified), typeof(bool));
        AssertProperty(typeof(GetBankAccounts.Item), nameof(GetBankAccounts.Item.BankName), typeof(string));
        AssertProperty(typeof(GetBankAccounts.Item), nameof(GetBankAccounts.Item.BranchName), typeof(string));
        AssertProperty(typeof(GetBankAccounts.Item), nameof(GetBankAccounts.Item.AccountType), typeof(string));
        AssertProperty(typeof(GetBankAccounts.Item), nameof(GetBankAccounts.Item.AccountNumber), typeof(string));
        AssertProperty(typeof(GetBankAccounts.Item), nameof(GetBankAccounts.Item.AccountName), typeof(string));

        AssertJsonProperty(typeof(GetTradingCommissionRequest), nameof(GetTradingCommissionRequest.ProductCode), typeof(string), "product_code");
        AssertJsonProperty(typeof(GetTradingCommissionResponse), nameof(GetTradingCommissionResponse.CommissionRate), typeof(decimal), "commission_rate");
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
}
