using System.Collections.Generic;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Adapter.Api.ExchangeInfo;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Dtos.Account;
using ExchangeApi.Contracts.Dtos.Common;
using ExchangeApi.Contracts.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Dtos.Market;
using ExchangeApi.Contracts.Dtos.Trading;
using ExchangeApi.Contracts.Common.DomainCommon.Types;
using ExchangeApi.Exchanges.Bittrade.Normalized.Apis;
using ExchangeApi.Exchanges.Bittrade.Normalized.Dtos;
using ExchangeApi.Exchanges.Bittrade.Normalized.Requests;
using ExchangeApi.Contracts.Common.CallCommon;
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
            var meta = new CallMeta(
                Layer: "Normalized",
                Component: "StubNormalizedExchangeInfoApi",
                Tags: null,
                Children: null);
            var call = new Call<GetSymbolsRequest, IReadOnlyList<BittradeSymbolNormalized>>(
                Id: CallId.New(),
                StartedAt: System.DateTimeOffset.UtcNow,
                Duration: System.TimeSpan.Zero,
                Request: request,
                Result: new CallResult<IReadOnlyList<BittradeSymbolNormalized>>.Ok(symbols),
                Meta: meta);
            return Task.FromResult(call);
        }
    }
}
