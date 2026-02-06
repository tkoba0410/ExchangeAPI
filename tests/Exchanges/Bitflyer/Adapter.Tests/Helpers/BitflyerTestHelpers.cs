using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeInfoDto = ExchangeApi.Contracts.Common.Dtos.GetExchangeInfoResponse;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Exchanges.Common.ExchangeInfo.Adapter.Internal;
using ExchangeApi.Exchanges.Bitflyer.ExchangeInfo.Adapter.Public.Api;
using ExchangeApi.Exchanges.Bitflyer.Api.Adapter.Internal;
using ExchangeApi.Exchanges.Bitflyer.Api.Adapter.Private.Api;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Api;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Internal.Markets;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Public.Api;
using ExchangeApi.Exchanges.Bitflyer.Api.Raw.Api;
using ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Fakes;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Helpers;

internal static class BitflyerTestHelpers
{
    public static IBitflyerNormalizedApi CreateNormalizedApi(IBitflyerRawApi raw, IBitflyerMarketResolver markets) =>
        BitflyerNormalizedApi.FromRaw(raw, markets);

    public static IBitflyerNormalizedApi CreateNormalizedApi(
        RawPublicDtos.GetTickerResponse ticker,
        IBitflyerMarketResolver markets,
        RawPublicDtos.GetBoardResponse? board = null,
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
            new RawPublicDtos.GetTickerResponse { ProductCode = "BTC_JPY" },
            markets,
            privateApi: privateApi,
            tradingApi: tradingApi);

    public static IBitflyerMarketResolver CreateResolver()
    {
        var resolver = new ExchangeInfoMarketResolver(new StubExchangeInfoApi(new ExchangeInfoDto(
            new[] { new ExchangeMarketInfo(Symbol.ParseOrThrow("BTC/JPY"), ProductCode.ParseOrThrow("BTC_JPY"), MarketType.ParseOrThrow("Spot")) },
            null,
            null,
            null)));
        return new BitflyerNormalizedMarketResolver(resolver);
    }

    public static BitflyerApiBundle CreateBundle(IBitflyerRawApi raw)
    {
        var publicApi = new BitflyerNormalizedPublicApi(raw);
        var exchangeInfo = new BitflyerExchangeInfoApi();
        var contractMarkets = new ExchangeInfoMarketResolver(exchangeInfo);
        var markets = new BitflyerNormalizedMarketResolver(contractMarkets);
        var normalized = BitflyerNormalizedApi.FromRaw(raw, markets);
        return new BitflyerApiBundle(normalized, publicApi, exchangeInfo, contractMarkets);
    }

    private sealed class StubExchangeInfoApi : IExchangeInfoProvider
    {
        private readonly ExchangeInfoDto _info;

        public StubExchangeInfoApi(ExchangeInfoDto info) => _info = info;

        public Task<Call<GetExchangeInfoRequest, ExchangeInfoDto>> GetExchangeInfoCallAsync(
            CancellationToken cancellationToken = default)
        {
            var meta = CallMeta.CreateInternal("Contracts", "StubExchangeInfoApi");
            var call = new Call<GetExchangeInfoRequest, ExchangeInfoDto>(
                Id: CallId.New(),
                StartedAt: System.DateTimeOffset.UtcNow,
                Duration: System.TimeSpan.Zero,
                Request: new GetExchangeInfoRequest(),
                Result: new CallResult<ExchangeInfoDto>.Ok(_info),
                Meta: meta);
            return Task.FromResult(call);
        }

    }
}
