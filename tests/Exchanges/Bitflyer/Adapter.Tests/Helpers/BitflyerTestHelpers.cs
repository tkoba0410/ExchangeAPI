using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Common.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Internal;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Private.Api;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Public.Api;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Api;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Internal.Markets;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Api;
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

    public static BitflyerApiBundle CreateBundle(IBitflyerRawApi raw)
    {
        var publicApi = new BitflyerNormalizedPublicApi(raw);
        var exchangeInfo = new BitflyerExchangeInfoApi(publicApi);
        var contractMarkets = new ExchangeInfoMarketResolver(exchangeInfo);
        var markets = new BitflyerNormalizedMarketResolver(contractMarkets);
        var normalized = BitflyerNormalizedApi.FromRaw(raw, markets);
        return new BitflyerApiBundle(normalized, publicApi, exchangeInfo, contractMarkets);
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
