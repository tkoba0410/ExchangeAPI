using System.Reflection;
using System.Text.Json.Serialization;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetBalance;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCollateral;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCollateralAccounts;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetTradingCommission;
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
}
