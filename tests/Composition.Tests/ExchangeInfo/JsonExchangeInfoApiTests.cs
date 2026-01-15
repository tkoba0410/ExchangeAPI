using System;
using System.IO;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Common.Dtos.Account;
using ExchangeApi.Contracts.Common.Dtos.Common;
using ExchangeApi.Contracts.Common.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Common.Dtos.Market;
using ExchangeApi.Contracts.Common.Dtos.Trading;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Composition.Providers.ExchangeInfo;
using Xunit;

namespace ExchangeApi.Tests.Composition.Tests.ExchangeInfo;

public class JsonExchangeInfoApiTests : IAsyncLifetime
{
    private readonly string _basePath = Path.Combine(Path.GetTempPath(), $"exchangeinfo-base-{Guid.NewGuid():N}.json");
    private readonly string _overlayPath = Path.Combine(Path.GetTempPath(), $"exchangeinfo-overlay-{Guid.NewGuid():N}.json");

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync()
    {
        TryDelete(_basePath);
        TryDelete(_overlayPath);
        return Task.CompletedTask;
    }

    [Fact]
    public async Task LoadSingleFile_ShouldParseMarkets()
    {
        File.WriteAllText(_basePath, """
        {
          "markets": [
            {
              "symbol": "BTC/JPY",
              "productCode": "BTC_JPY",
              "type": "Spot",
              "priceIncrement": 1,
              "sizeIncrement": 0.001,
              "minSize": 0.001,
              "makerFeeRate": 0.001,
              "takerFeeRate": 0.002,
              "feeCurrency": "BTC",
              "feeType": "Percentage",
              "isSupported": true
            }
          ],
          "features": { "supportsWebSocket": false, "supportsMargin": false, "supportsStopOrder": false, "supportsParentOrder": false, "supportsCandlestick": false, "supportsOrderBookDelta": false, "supportsRealtimeExecutions": false, "supportsWithdraw": false },
          "rateLimits": { "requestsPerMinute": 500, "ordersPerMinute": 100 },
          "maintenance": { "status": "Planned", "plannedUntil": "2025-01-01T04:10:00Z", "message": "daily" }
        }
        """);

        var api = new JsonExchangeInfoApi(new[] { _basePath }, cacheTtl: TimeSpan.FromSeconds(1));

        var call = await api.GetExchangeInfoCallAsync();
        var ok = Assert.IsType<ExchangeApi.Primitives.CallCommon.CallResult<ExchangeApi.Contracts.Common.Dtos.ExchangeInfo.ExchangeInfo>.Ok>(call.Result);
        var info = ok.Response;

        Assert.Single(info.Markets);
        var market = info.Markets[0];
        Assert.Equal("BTC_JPY", market.ProductCode);
        Assert.Equal("BTC", market.FeeCurrency);
        Assert.Equal(FeeType.Percentage, market.FeeType);
        Assert.NotNull(info.Features);
        Assert.NotNull(info.RateLimits);
        Assert.NotNull(info.Maintenance);
    }

    [Fact]
    public async Task Overlay_ShouldOverrideBaseline()
    {
        File.WriteAllText(_basePath, """
        { "markets": [ { "symbol": "BTC/JPY", "productCode": "BTC_JPY", "type": "Spot", "feeCurrency": "BTC", "makerFeeRate": 0.001, "takerFeeRate": 0.002, "feeType": "Percentage" } ],
          "features": { "supportsWebSocket": false, "supportsMargin": false, "supportsStopOrder": false, "supportsParentOrder": false, "supportsCandlestick": false, "supportsOrderBookDelta": false, "supportsRealtimeExecutions": false, "supportsWithdraw": false },
          "rateLimits": { "requestsPerMinute": 500, "ordersPerMinute": 100 }
        }
        """);

        File.WriteAllText(_overlayPath, """
        { "markets": [ { "symbol": "BTC/JPY", "productCode": "BTC_JPY", "type": "Spot", "feeCurrency": "JPY", "makerFeeRate": 0.003, "takerFeeRate": 0.004, "feeType": "Flat" } ],
          "features": { "supportsWebSocket": true, "supportsMargin": false, "supportsStopOrder": false, "supportsParentOrder": false, "supportsCandlestick": false, "supportsOrderBookDelta": false, "supportsRealtimeExecutions": false, "supportsWithdraw": false },
          "rateLimits": { "requestsPerMinute": 1000, "ordersPerMinute": 200 }
        }
        """);

        var api = new JsonExchangeInfoApi(new[] { _basePath, _overlayPath }, cacheTtl: TimeSpan.FromSeconds(1));

        var call = await api.GetExchangeInfoCallAsync();
        var ok = Assert.IsType<ExchangeApi.Primitives.CallCommon.CallResult<ExchangeApi.Contracts.Common.Dtos.ExchangeInfo.ExchangeInfo>.Ok>(call.Result);
        var info = ok.Response;

        Assert.Single(info.Markets);
        var market = info.Markets[0];
        Assert.Equal("JPY", market.FeeCurrency); // overlay wins
        Assert.Equal(FeeType.Flat, market.FeeType);
        Assert.Equal(0.003m, market.MakerFeeRate);
        Assert.True(info.Features?.SupportsWebSocket);
        Assert.Equal(1000, info.RateLimits?.RequestsPerMinute);
    }

    private static void TryDelete(string path)
    {
        try
        {
            if (File.Exists(path)) File.Delete(path);
        }
        catch
        {
            // ignore cleanup failures
        }
    }
}
