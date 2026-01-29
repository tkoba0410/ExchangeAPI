using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Common.Dtos.Account;
using ExchangeApi.Contracts.Common.Dtos.Common;
using ExchangeApi.Contracts.Common.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Common.Dtos.Market;
using ExchangeApi.Contracts.Common.Dtos.Trading;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Internal;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Api;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Internal.Markets;
using ExchangeApi.Exchanges.Bitflyer.Raw.Api;
using ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Fakes;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Helpers;

internal static class BitflyerTestHelpers
{
    public static IBitflyerNormalizedApi CreateNormalizedApi(IBitflyerRawApi raw, IBitflyerMarketResolver markets) =>
        BitflyerNormalizedApi.FromRaw(raw, markets);

    public static IBitflyerNormalizedApi CreateNormalizedApi(
        RawPublicDtos.Ticker ticker,
        IBitflyerMarketResolver markets,
        RawPublicDtos.Board? board = null,
        FakeBitflyerPrivateApi? privateApi = null,
        FakeBitflyerPrivateTradingApi? tradingApi = null)
    {
        var raw = new FakeBitflyerPublicApi(ticker, board, privateApi, tradingApi);
        return BitflyerNormalizedApi.FromRaw(raw, markets);
    }

    public static IBitflyerNormalizedApi CreateTradingApi(
        FakeBitflyerPrivateTradingApi tradingApi,
        IBitflyerMarketResolver markets,
        FakeBitflyerPrivateApi? privateApi = null) =>
        CreateNormalizedApi(
            new RawPublicDtos.Ticker { ProductCode = "BTC_JPY" },
            markets,
            privateApi: privateApi,
            tradingApi: tradingApi);

    public static IBitflyerMarketResolver CreateResolver()
    {
        var resolver = new ExchangeInfoMarketResolver(new StubExchangeInfoApi(new ExchangeInfo(
            new[] { new ExchangeMarketInfo("BTC/JPY", "BTC_JPY", "Spot") },
            null,
            null,
            null)));
        return new BitflyerNormalizedMarketResolver(resolver);
    }

    private sealed class StubExchangeInfoApi : IExchangeInfoProvider
    {
        private readonly ExchangeInfo _info;

        public StubExchangeInfoApi(ExchangeInfo info) => _info = info;

        public Task<Call<GetExchangeInfoRequest, ExchangeInfo>> GetExchangeInfoCallAsync(
            CancellationToken cancellationToken = default)
        {
            var meta = CallMeta.CreateInternal("Contracts", "StubExchangeInfoApi");
            var call = new Call<GetExchangeInfoRequest, ExchangeInfo>(
                Id: CallId.New(),
                StartedAt: System.DateTimeOffset.UtcNow,
                Duration: System.TimeSpan.Zero,
                Request: new GetExchangeInfoRequest(),
                Result: new CallResult<ExchangeInfo>.Ok(_info),
                Meta: meta);
            return Task.FromResult(call);
        }

    }
}
