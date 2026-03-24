using System.Text.Json;
using ExchangeApi.Exchanges.Binance.Composition.Factory;
using ExchangeApi.Exchanges.Binance.Native.Public.Endpoints.GetKlines;
using ExchangeApi.Exchanges.Binance.Vocabulary;
using ExchangeApi.Tests.Exchanges.Binance.LiveTests.Infrastructure;

namespace ExchangeApi.Tests.Exchanges.Binance.LiveTests;

public sealed class LiveTests
{
    [BinancePublicReadLiveFact]
    public async Task GetKlines_ClosedWindowParity()
    {
        var settings = BinanceLiveTestSettings.Load();
        var client = BinanceClientFactory.CreateNativeClient(settings.ToClientOptions());
        const long oneHourInMilliseconds = 60L * 60L * 1000L;
        var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var currentHourStart = now - (now % oneHourInMilliseconds);
        var request = new GetKlinesRequest
        {
            Symbol = BinanceSymbols.BtcJpy,
            Interval = "1h",
            Limit = 2,
            StartTime = currentHourStart - (2L * oneHourInMilliseconds),
            EndTime = currentHourStart - 1L,
        };

        var nativeCall = await client.Public.GetKlinesCallAsync(request);
        var protocolCall = await client.Protocol.Public.GetKlinesCallAsync(
            BinanceSymbols.BtcJpy,
            "1h",
            startTime: request.StartTime,
            endTime: request.EndTime,
            limit: 2);

        Assert.True(protocolCall.IsSuccess, protocolCall.Error?.Message);
        Assert.True(nativeCall.IsSuccess, nativeCall.Error?.Message);
        Assert.NotNull(protocolCall.Response);
        Assert.NotNull(protocolCall.Response!.BodyText);
        Assert.NotNull(nativeCall.Response);

        using var document = JsonDocument.Parse(protocolCall.Response.BodyText);
        var root = document.RootElement;
        Assert.Equal(JsonValueKind.Array, root.ValueKind);

        var native = nativeCall.Response!;
        Assert.Equal(root.GetArrayLength(), native.Count);

        for (var index = 0; index < native.Count; index++)
        {
            var protocolItem = root[index];
            var nativeItem = native[index];

            Assert.Equal(protocolItem[0].GetInt64(), nativeItem.OpenTime);
            Assert.Equal(decimal.Parse(protocolItem[1].GetString()!), nativeItem.OpenPrice);
            Assert.Equal(decimal.Parse(protocolItem[4].GetString()!), nativeItem.ClosePrice);
            Assert.Equal(protocolItem[6].GetInt64(), nativeItem.CloseTime);
            Assert.Equal(protocolItem[8].GetInt32(), nativeItem.NumberOfTrades);
        }
    }
}
