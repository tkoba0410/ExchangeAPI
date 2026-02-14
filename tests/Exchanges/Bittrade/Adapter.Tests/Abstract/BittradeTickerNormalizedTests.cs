using System.Text.Json;
using ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Mappers;
using ExchangeApi.Exchanges.Bittrade.Raw;
using ExchangeApi.Exchanges.Bittrade.Raw.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Raw.Public.Api;
using ExchangeApi.Exchanges.Bittrade.Raw.Internal;
using ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Types;
using ExchangeApi.Primitives.CallCommon;
using Xunit;

namespace ExchangeApi.Tests.Exchanges.Bittrade.Adapter.Tests.Abstract;

public sealed class TickerNormalizedTests
{
    [Fact]
    public void Normalize_keeps_raw_snapshot()
    {
        var json = """
        {
          "status": "ok",
          "tick": {
            "close": 100,
            "open": 99,
            "low": 98,
            "high": 101,
            "amount": 12.3,
            "vol": 456.7,
            "ts": 1704067200000,
            "bid": [99.5, 1.2],
            "ask": [100.5, 1.3]
          },
          "ts": 1704067200000
        }
        """;

        var raw = RawJson.DeserializeOrThrow<RawPublicDtos.GetDetailMergedResponse>(json, "Bittrade.GetTicker");
        var ok = Normalizer.TryNormalizeTicker(raw, TimestampPolicy.Required, json, out var normalized, out var error);
        Assert.True(ok, error?.Message);
        Assert.NotNull(normalized);
        Assert.Null(error);

        Assert.Equal(JsonValueKind.Object, normalized!.RawSnapshot.ValueKind);
        Assert.True(normalized.RawSnapshot.TryGetProperty("tick", out _));
    }

    [Fact]
    public void Normalize_ticker_missing_timestamp_fails_when_required()
    {
        var json = """
        {
          "status": "ok",
          "tick": {
            "close": 100,
            "open": 99,
            "low": 98,
            "high": 101,
            "amount": 12.3,
            "vol": 456.7,
            "bid": [99.5, 1.2],
            "ask": [100.5, 1.3]
          }
        }
        """;

        var raw = RawJson.DeserializeOrThrow<RawPublicDtos.GetDetailMergedResponse>(json, "Bittrade.GetTicker");
        var ok = Normalizer.TryNormalizeTicker(raw, TimestampPolicy.Required, json, out var normalized, out var error);
        Assert.False(ok);
        Assert.Null(normalized);
        Assert.NotNull(error);
        Assert.Equal(CallErrorKind.Mapping, error!.Kind);
        Assert.Contains("Missing required timestamp", error.Message);
    }

    [Fact]
    public void Normalize_tickers_missing_timestamp_allows_null_when_optional()
    {
        var entries = new[]
        {
            new RawPublicDtos.RawTickerEntry(
                Symbol: "BTC/JPY",
                Open: 1m,
                Close: 2m,
                Low: 1m,
                High: 3m,
                Amount: 4m,
                Volume: 5m,
                Count: 6L,
                Bid: null,
                Ask: null)
        };

        var ok = Normalizer.TryNormalizeTickers(entries, TimestampPolicy.Optional, out var normalized, out var error);
        Assert.True(ok);
        Assert.NotNull(normalized);
        Assert.Null(error);
        Assert.Single(normalized!);
        Assert.Null(normalized[0].Timestamp);
    }
}
