using System.Collections.Generic;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Adapter.Api.ExchangeInfo;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Common.Dtos.Account;
using ExchangeApi.Contracts.Common.Dtos.Common;
using ExchangeApi.Contracts.Common.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Common.Dtos.Market;
using ExchangeApi.Contracts.Common.Dtos.Trading;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Exchanges.Bittrade.Normalized.Public.Api;
using ExchangeApi.Exchanges.Bittrade.Normalized.Public.Dtos;
using ExchangeApi.Exchanges.Bittrade.Normalized.Public.Requests;
using ExchangeApi.Primitives.CallCommon;
using Xunit;

namespace ExchangeApi.Tests.Exchanges.Bittrade.Adapter.Tests.Abstract;

public class BittradeExchangeInfoApiTests
{
    [Fact]
    public async Task GetExchangeInfoAsync_MapsSymbols()
    {
        var api = new BittradeExchangeInfoApi(new StubNormalizedExchangeInfoApi());

        var call = await api.GetExchangeInfoCallAsync();
        var ok = Assert.IsType<CallResult<ExchangeInfo>.Ok>(call.Result);
        var info = ok.Response;

        Assert.Single(info.Markets);
        var m = info.Markets[0];
        Assert.Equal("BTC/JPY", m.Symbol);
        Assert.Equal("btcjpy", m.ProductCode);
        Assert.Equal(new Price(0.01m), m.PriceIncrement);
        Assert.Equal(new Size(0.0001m), m.SizeIncrement);
        Assert.Equal(new Size(0.0001m), m.MinSize);
        Assert.Equal(1000m, m.MinNotional);
        Assert.True(m.IsSupported);
    }

    private sealed class StubNormalizedExchangeInfoApi : IBittradeNormalizedExchangeInfoApi
    {
        public Task<Call<GetSymbolsRequest, IReadOnlyList<BittradeSymbolNormalized>>> GetSymbolsCallAsync(
            System.Threading.CancellationToken ct = default)
        {
            IReadOnlyList<BittradeSymbolNormalized> symbols = new[]
            {
                new BittradeSymbolNormalized(
                    Symbol: "btcjpy",
                    BaseCurrency: "btc",
                    QuoteCurrency: "jpy",
                    PricePrecision: 2,
                    AmountPrecision: 4,
                    MinOrderAmount: 0.0001m,
                    MinOrderValue: 1000m,
                    State: "online")
            };
            var request = new GetSymbolsRequest();
            var meta = CallMeta.CreateInternal("Normalized", "StubNormalizedExchangeInfoApi");
            var call = new Call<GetSymbolsRequest, IReadOnlyList<BittradeSymbolNormalized>>(
                Id: CallId.New(),
                StartedAt: System.DateTimeOffset.UtcNow,
                Duration: System.TimeSpan.Zero,
                Request: request,
                Result: new CallResult<IReadOnlyList<BittradeSymbolNormalized>>.Ok(symbols),
                Meta: meta);
            return Task.FromResult(call);
        }

        public Task<Call<GetCurrencysRequest, IReadOnlyList<string>>> GetCurrencysCallAsync(
            System.Threading.CancellationToken ct = default)
        {
            IReadOnlyList<string> data = new[] { "btc", "jpy" };
            var request = new GetCurrencysRequest();
            var meta = CallMeta.CreateInternal("Normalized", "StubNormalizedExchangeInfoApi");
            var call = new Call<GetCurrencysRequest, IReadOnlyList<string>>(
                Id: CallId.New(),
                StartedAt: System.DateTimeOffset.UtcNow,
                Duration: System.TimeSpan.Zero,
                Request: request,
                Result: new CallResult<IReadOnlyList<string>>.Ok(data),
                Meta: meta);
            return Task.FromResult(call);
        }

        public Task<Call<GetTimestampRequest, System.DateTimeOffset>> GetTimestampCallAsync(
            System.Threading.CancellationToken ct = default)
        {
            var request = new GetTimestampRequest();
            var meta = CallMeta.CreateInternal("Normalized", "StubNormalizedExchangeInfoApi");
            var call = new Call<GetTimestampRequest, System.DateTimeOffset>(
                Id: CallId.New(),
                StartedAt: System.DateTimeOffset.UtcNow,
                Duration: System.TimeSpan.Zero,
                Request: request,
                Result: new CallResult<System.DateTimeOffset>.Ok(System.DateTimeOffset.UtcNow),
                Meta: meta);
            return Task.FromResult(call);
        }
    }
}
