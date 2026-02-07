using System.Collections.Generic;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.ExchangeInfo.Adapter.Public.Api;
using ExchangeInfoDto = ExchangeApi.Contracts.Common.Dtos.ExchangeInfoResponse;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Public.Api;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Tests.Exchanges.Bittrade.Adapter.Tests.Helpers;
using Xunit;

namespace ExchangeApi.Tests.Exchanges.Bittrade.Adapter.Tests.Abstract;

public class BittradeExchangeInfoApiTests
{
    [Fact]
    public async Task GetExchangeInfoAsync_MapsSymbols()
    {
        var api = new BittradeExchangeInfoApi(new BittradeNormalizedPublicApi(new StubRawApi()));

        var call = await api.GetExchangeInfoAsync(new ExchangeInfoRequest());
        var ok = Assert.IsType<CallResult<ExchangeInfoDto>.Ok>(call.Result);
        var info = ok.Response;

        Assert.Single(info.Markets);
        var m = info.Markets[0];
        Assert.Equal("BTC/JPY", m.Symbol.Value);
        Assert.Equal("btcjpy", m.ProductCode.Value);
        Assert.Equal(new Price(0.01m), m.PriceIncrement);
        Assert.Equal(new Size(0.0001m), m.SizeIncrement);
        Assert.Equal(new Size(0.0001m), m.MinSize);
        Assert.Equal(1000m, m.MinNotional);
        Assert.True(m.IsSupported);
    }

    private sealed class StubRawApi : BittradeRawApiStub
    {
        public override Task<Call<RawPublicRequests.GetSymbolsRequest, RawPublicDtos.GetSymbolsResponse>> GetSymbolsCallAsync(
            RawPublicRequests.GetSymbolsRequest request,
            System.Threading.CancellationToken ct = default)
        {
            IReadOnlyList<RawPublicDtos.RawSymbolInfo> symbols = new[]
            {
                new RawPublicDtos.RawSymbolInfo(
                    Symbol: "btcjpy",
                    BaseCurrency: "btc",
                    QuoteCurrency: "jpy",
                    PricePrecision: 2,
                    AmountPrecision: 4,
                    ValuePrecision: null,
                    MinOrderAmount: "0.0001",
                    MinOrderValue: "1000",
                    State: "online")
            };
            var meta = CallMeta.CreateInternal("Raw", "StubRawApi");
            var call = new Call<RawPublicRequests.GetSymbolsRequest, RawPublicDtos.GetSymbolsResponse>(
                Id: CallId.New(),
                StartedAt: System.DateTimeOffset.UtcNow,
                Duration: System.TimeSpan.Zero,
                Request: request,
                Result: new CallResult<RawPublicDtos.GetSymbolsResponse>.Ok(new RawPublicDtos.GetSymbolsResponse("ok", symbols)),
                Meta: meta);
            return Task.FromResult(call);
        }

        public override Task<Call<RawPublicRequests.GetCurrencysRequest, RawPublicDtos.GetCurrencysResponse>> GetCurrencysCallAsync(
            RawPublicRequests.GetCurrencysRequest request,
            System.Threading.CancellationToken ct = default)
        {
            IReadOnlyList<string> data = new[] { "btc", "jpy" };
            var meta = CallMeta.CreateInternal("Raw", "StubRawApi");
            var call = new Call<RawPublicRequests.GetCurrencysRequest, RawPublicDtos.GetCurrencysResponse>(
                Id: CallId.New(),
                StartedAt: System.DateTimeOffset.UtcNow,
                Duration: System.TimeSpan.Zero,
                Request: request,
                Result: new CallResult<RawPublicDtos.GetCurrencysResponse>.Ok(new RawPublicDtos.GetCurrencysResponse("ok", data)),
                Meta: meta);
            return Task.FromResult(call);
        }

        public override Task<Call<RawPublicRequests.GetTimestampRequest, RawPublicDtos.GetTimestampResponse>> GetTimestampCallAsync(
            RawPublicRequests.GetTimestampRequest request,
            System.Threading.CancellationToken ct = default)
        {
            var data = System.DateTimeOffset.UtcNow;
            var meta = CallMeta.CreateInternal("Raw", "StubRawApi");
            var call = new Call<RawPublicRequests.GetTimestampRequest, RawPublicDtos.GetTimestampResponse>(
                Id: CallId.New(),
                StartedAt: System.DateTimeOffset.UtcNow,
                Duration: System.TimeSpan.Zero,
                Request: request,
                Result: new CallResult<RawPublicDtos.GetTimestampResponse>.Ok(new RawPublicDtos.GetTimestampResponse("ok", data)),
                Meta: meta);
            return Task.FromResult(call);
        }
    }
}
