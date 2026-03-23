using System.Text.Json;
using ExchangeApi.Stage10.Bitflyer.Composition.Factory;
using ExchangeApi.Stage10.Bitflyer.Composition.Options;
using ExchangeApi.Stage10.Bitflyer.Native.Private.Dtos;
using ExchangeApi.Stage10.Bitflyer.Native.Private.Requests;
using ExchangeApi.Stage10.Bitflyer.Native.Public.Requests;
using ExchangeApi.Tests.Stage10.Bitflyer.LiveTests.Infrastructure;

namespace ExchangeApi.Tests.Stage10.Bitflyer.LiveTests;

public sealed class Stage10LiveParityTests
{
    [BitflyerStage10LivePublicFact]
    [Trait("Category", "Live")]
    [Trait("Scope", "Public")]
    [Trait("Layer", "Parity")]
    public async Task GetTicker_ProtocolAndNative_AgreeOnContract()
    {
        using var bundle = BitflyerStage10ClientFactory.CreateNativeClient(new BitflyerStage10ClientOptions
        {
            BaseUri = BitflyerStage10LiveTestSettings.ResolveBaseUri(),
        });

        var wireCall = await bundle.Protocol.Public.GetTickerCallAsync(BitflyerStage10LiveTestSettings.DefaultProductCode);
        var wireResponse = BitflyerStage10LiveAssert.RequireWireSuccess(wireCall);
        using var wireJson = JsonDocument.Parse(wireResponse.Json);

        var nativeCall = await bundle.Public.GetTickerCallAsync(new GetTickerRequest
        {
            ProductCode = BitflyerStage10LiveTestSettings.DefaultProductCode,
        });
        var response = BitflyerStage10LiveAssert.RequireOk(nativeCall);
        var root = wireJson.RootElement;

        Assert.Equal(root.GetProperty("product_code").GetString(), response.ProductCode);
        Assert.Equal(root.GetProperty("state").GetString(), response.State);
        Assert.Equal(BitflyerStage10LiveAssert.ParseTimestamp(root.GetProperty("timestamp")), response.Timestamp);
        Assert.Equal(root.GetProperty("tick_id").GetInt64(), response.TickId);
        Assert.Equal(root.GetProperty("best_bid").GetDecimal(), response.BestBid);
        Assert.Equal(root.GetProperty("best_ask").GetDecimal(), response.BestAsk);
        Assert.Equal(root.GetProperty("best_bid_size").GetDecimal(), response.BestBidSize);
        Assert.Equal(root.GetProperty("best_ask_size").GetDecimal(), response.BestAskSize);
        Assert.Equal(root.GetProperty("total_bid_depth").GetDecimal(), response.TotalBidDepth);
        Assert.Equal(root.GetProperty("total_ask_depth").GetDecimal(), response.TotalAskDepth);
        Assert.Equal(root.GetProperty("market_bid_size").GetDecimal(), response.MarketBidSize);
        Assert.Equal(root.GetProperty("market_ask_size").GetDecimal(), response.MarketAskSize);
        Assert.Equal(root.GetProperty("ltp").GetDecimal(), response.Ltp);
        Assert.Equal(root.GetProperty("volume").GetDecimal(), response.Volume);
        Assert.Equal(root.GetProperty("volume_by_product").GetDecimal(), response.VolumeByProduct);
    }

    [BitflyerStage10LivePrivateFact]
    [Trait("Category", "Live")]
    [Trait("Scope", "Private")]
    [Trait("Layer", "Parity")]
    public async Task GetBalance_ProtocolAndNative_AgreeOnTopLevelArrayContract()
    {
        using var bundle = BitflyerStage10ClientFactory.CreateNativeClient(new BitflyerStage10ClientOptions
        {
            BaseUri = BitflyerStage10LiveTestSettings.ResolveBaseUri(),
            Credentials = BitflyerStage10LiveTestSettings.GetCredentials(),
        });

        Assert.NotNull(bundle.Protocol.Private);
        Assert.NotNull(bundle.Private);

        var wireCall = await bundle.Protocol.Private!.GetBalanceCallAsync();
        var wireResponse = BitflyerStage10LiveAssert.RequireWireSuccess(wireCall);
        using var wireJson = JsonDocument.Parse(wireResponse.Json);
        Assert.Equal(JsonValueKind.Array, wireJson.RootElement.ValueKind);

        var nativeCall = await bundle.Private!.GetBalanceCallAsync(new GetBalanceRequest());
        var response = BitflyerStage10LiveAssert.RequireOk(nativeCall);
        var responseByCurrency = response.ToDictionary(item => item.CurrencyCode, StringComparer.Ordinal);

        foreach (var element in wireJson.RootElement.EnumerateArray())
        {
            var currencyCode = element.GetProperty("currency_code").GetString();
            Assert.False(string.IsNullOrWhiteSpace(currencyCode));
            Assert.True(responseByCurrency.Remove(currencyCode!, out var nativeItem), $"Currency '{currencyCode}' was missing from Native response.");
            Assert.Equal(element.GetProperty("amount").GetDecimal(), nativeItem.Amount);
            Assert.Equal(element.GetProperty("available").GetDecimal(), nativeItem.Available);
        }

        Assert.Empty(responseByCurrency);
    }

    [Fact(Skip = "Protocol-vs-Native live parity for SendChildOrder would place two real orders. Keep body/DTO validation in unit tests and run post live manually.")]
    [Trait("Category", "Live")]
    [Trait("Scope", "Private")]
    [Trait("Layer", "Parity")]
    public Task SendChildOrder_ProtocolAndNative_Parity_IsNotSafeOnLive() =>
        Task.CompletedTask;
}
