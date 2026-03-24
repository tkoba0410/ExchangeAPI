using System.Text.Json;
using ExchangeApi.Exchanges.Bitflyer.Composition.Factory;
using ExchangeApi.Exchanges.Bitflyer.Composition.Options;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.CancelChildOrder;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetBalance;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.SendChildOrder;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetTicker;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Tests.Exchanges.Bitflyer.LiveTests.Infrastructure;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.LiveTests;

public sealed class Stage10LiveTests
{
    [BitflyerPublicReadLiveFact]
    public async Task GetTicker_ReadParity()
    {
        var settings = BitflyerLiveTestSettings.Load();

        var client = BitflyerClientFactory.CreateNativeClient(CreateOptions(settings));
        var request = new GetTickerRequest { ProductCode = ProductCodes.BtcJpy };

        var nativeCall = await client.Public.GetTickerCallAsync(request);
        var protocolCall = await client.Protocol.Public.GetTickerCallAsync(ProductCodes.BtcJpy);

        Assert.True(protocolCall.IsSuccess);
        Assert.True(nativeCall.IsSuccess);
        Assert.NotNull(protocolCall.Response);
        Assert.NotNull(protocolCall.Response!.BodyText);
        Assert.NotNull(nativeCall.Response);

        using var document = JsonDocument.Parse(protocolCall.Response.BodyText);
        var root = document.RootElement;
        var native = nativeCall.Response!;

        Assert.Equal(root.GetProperty("product_code").GetString(), native.ProductCode);
        Assert.Equal(root.GetProperty("state").GetString(), native.State);
        Assert.Equal(root.GetProperty("tick_id").GetInt64(), native.TickId);
        Assert.Equal(root.GetProperty("best_bid").GetDecimal(), native.BestBid);
        Assert.Equal(root.GetProperty("best_ask").GetDecimal(), native.BestAsk);
        Assert.Equal(root.GetProperty("ltp").GetDecimal(), native.Ltp);
        Assert.Equal(root.GetProperty("volume").GetDecimal(), native.Volume);
        Assert.Equal(root.GetProperty("volume_by_product").GetDecimal(), native.VolumeByProduct);
    }

    [BitflyerPrivateReadLiveFact]
    public async Task GetBalance_ReadParity()
    {
        var settings = BitflyerLiveTestSettings.Load();

        var client = BitflyerClientFactory.CreateNativeClient(CreateOptions(settings));
        Assert.NotNull(client.Private);
        Assert.NotNull(client.Protocol.Private);

        var nativeCall = await client.Private!.GetBalanceCallAsync(new GetBalanceRequest());
        var protocolCall = await client.Protocol.Private!.GetBalanceCallAsync();

        Assert.True(protocolCall.IsSuccess);
        Assert.True(nativeCall.IsSuccess);
        Assert.NotNull(protocolCall.Response);
        Assert.NotNull(protocolCall.Response!.BodyText);
        Assert.NotNull(nativeCall.Response);

        using var document = JsonDocument.Parse(protocolCall.Response.BodyText);
        var root = document.RootElement;
        var native = nativeCall.Response!;

        Assert.Equal(JsonValueKind.Array, root.ValueKind);
        Assert.Equal(root.GetArrayLength(), native.Count);

        var protocolByCurrency = root.EnumerateArray()
            .ToDictionary(
                item => item.GetProperty("currency_code").GetString()!,
                item => item);

        foreach (var item in native)
        {
            Assert.True(protocolByCurrency.TryGetValue(item.CurrencyCode, out var protocolItem));
            Assert.Equal(protocolItem.GetProperty("amount").GetDecimal(), item.Amount);
            Assert.Equal(protocolItem.GetProperty("available").GetDecimal(), item.Available);
        }
    }

    [BitflyerWriteLiveFact]
    public async Task SendChildOrder_CancelChildOrder_WriteLifecycle()
    {
        var settings = BitflyerLiveTestSettings.Load();
        var client = BitflyerClientFactory.CreateNativeClient(CreateOptions(settings));

        Assert.NotNull(client.Private);

        var tickerCall = await client.Public.GetTickerCallAsync(new GetTickerRequest { ProductCode = ProductCodes.BtcJpy });
        Assert.True(tickerCall.IsSuccess);
        Assert.NotNull(tickerCall.Response);

        var ticker = tickerCall.Response!;
        var limitPrice = Math.Max(1m, decimal.Floor(ticker.Ltp * 0.5m));
        var orderRequest = new SendChildOrderRequest
        {
            ProductCode = ProductCodes.BtcJpy,
            ChildOrderType = ChildOrderTypes.Limit,
            Side = OrderSides.Buy,
            Price = limitPrice,
            Size = 0.001m,
            MinuteToExpire = 1,
            TimeInForce = TimeInForces.Gtc,
        };

        string? acceptanceId = null;

        try
        {
            var sendCall = await client.Private!.SendChildOrderCallAsync(orderRequest);
            Assert.True(sendCall.IsSuccess, sendCall.Error?.Message);
            Assert.NotNull(sendCall.Response);
            Assert.False(string.IsNullOrWhiteSpace(sendCall.Response!.ChildOrderAcceptanceId));

            acceptanceId = sendCall.Response.ChildOrderAcceptanceId;
        }
        finally
        {
            if (!string.IsNullOrWhiteSpace(acceptanceId))
            {
                var cancelCall = await client.Private!.CancelChildOrderCallAsync(new CancelChildOrderRequest
                {
                    ProductCode = ProductCodes.BtcJpy,
                    ChildOrderAcceptanceId = acceptanceId,
                });

                Assert.True(cancelCall.IsSuccess, cancelCall.Error?.Message);
            }
        }
    }

    private static BitflyerClientOptions CreateOptions(BitflyerLiveTestSettings settings)
    {
        return new BitflyerClientOptions
        {
            BaseUri = settings.BaseUri,
            Credentials = settings.Credentials,
            EnableProtocolDebugLogging = settings.EnableProtocolDebugLogging,
            ProtocolDebugLogDirectory = Path.Combine("local", "logs", "bitflyer", "stage10", "live-tests"),
        };
    }
}
