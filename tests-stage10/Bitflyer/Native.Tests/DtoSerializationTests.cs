using System.Text.Json;
using ExchangeApi.Stage10.Bitflyer.Vocabulary;
using ExchangeApi.Stage10.Bitflyer.Native.Private.Dtos;
using ExchangeApi.Stage10.Bitflyer.Native.Private.Requests;
using ExchangeApi.Stage10.Bitflyer.Native.Public.Dtos;
using ExchangeApi.Stage10.Bitflyer.Native.Public.Requests;

namespace ExchangeApi.Tests.Stage10.Bitflyer.Native.Tests;

public sealed class DtoSerializationTests
{
    [Fact]
    public void GetTickerDtos_SerializeWithSnakeCasePropertyNames()
    {
        var requestJson = JsonSerializer.Serialize(new GetTickerRequest
        {
            ProductCode = ProductCodes.BtcJpy,
        });
        var responseJson = JsonSerializer.Serialize(new GetTickerResponse
        {
            ProductCode = ProductCodes.BtcJpy,
            State = "RUNNING",
            Timestamp = DateTimeOffset.Parse("2024-01-01T00:00:00Z"),
            TickId = 1,
            BestBid = 100m,
            BestAsk = 101m,
            BestBidSize = 0.1m,
            BestAskSize = 0.2m,
            TotalBidDepth = 10m,
            TotalAskDepth = 20m,
            MarketBidSize = 0m,
            MarketAskSize = 0m,
            Ltp = 100.5m,
            Volume = 200m,
            VolumeByProduct = 300m,
        });

        Assert.Contains($"\"product_code\":\"{ProductCodes.BtcJpy}\"", requestJson);
        Assert.Contains($"\"product_code\":\"{ProductCodes.BtcJpy}\"", responseJson);
        Assert.Contains("\"tick_id\":1", responseJson);
        Assert.Contains("\"market_bid_size\":0", responseJson);
        Assert.Contains("\"volume_by_product\":300", responseJson);
    }

    [Fact]
    public void PrivateDtos_SerializeWithSnakeCaseAndOmitNullOptionals()
    {
        var sendRequestJson = JsonSerializer.Serialize(new SendChildOrderRequest
        {
            ProductCode = ProductCodes.BtcJpy,
            ChildOrderType = "LIMIT",
            Side = "BUY",
            Size = 0.01m,
            Price = 100m,
            MinuteToExpire = null,
            TimeInForce = null,
        });
        var cancelRequestJson = JsonSerializer.Serialize(new CancelChildOrderRequest
        {
            ProductCode = ProductCodes.BtcJpy,
            ChildOrderAcceptanceId = "JRF20240101-000000-000001",
        });
        var balanceJson = JsonSerializer.Serialize(new GetBalance.Item
        {
            CurrencyCode = "JPY",
            Amount = 1000m,
            Available = 500m,
        });
        var responseJson = JsonSerializer.Serialize(new SendChildOrderResponse
        {
            ChildOrderAcceptanceId = "JRF20240101-000000-000001",
        });

        Assert.Contains($"\"product_code\":\"{ProductCodes.BtcJpy}\"", sendRequestJson);
        Assert.Contains("\"child_order_type\":\"LIMIT\"", sendRequestJson);
        Assert.Contains("\"price\":100", sendRequestJson);
        Assert.DoesNotContain("minute_to_expire", sendRequestJson);
        Assert.DoesNotContain("time_in_force", sendRequestJson);
        Assert.Contains($"\"product_code\":\"{ProductCodes.BtcJpy}\"", cancelRequestJson);
        Assert.Contains("\"child_order_acceptance_id\":\"JRF20240101-000000-000001\"", cancelRequestJson);
        Assert.DoesNotContain("child_order_id", cancelRequestJson);
        Assert.Contains("\"currency_code\":\"JPY\"", balanceJson);
        Assert.Contains("\"child_order_acceptance_id\":\"JRF20240101-000000-000001\"", responseJson);
    }
}
