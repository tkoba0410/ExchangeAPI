using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeInfoDto = ExchangeApi.Contracts.Common.Dtos.ExchangeInfoResponse;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Exchanges.Common.Application.ExchangeInfo.Adapter.Internal;
using ExchangeApi.Exchanges.Bitflyer.Application.ExchangeInfo.Adapter.Public.Api;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Internal;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Private.Api;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Api;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Api.Markets;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Api;
using ExchangeApi.Exchanges.Bitflyer.Raw.Api;
using ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Fakes;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Helpers;

internal static class BitflyerTestHelpers
{
    public static INormalizedApi CreateNormalizedApi(IRawApi raw, IMarketResolver markets) =>
        NormalizedApi.FromRaw(raw, markets);

    public static INormalizedApi CreateNormalizedApi(
        RawPublicDtos.GetTickerResponse ticker,
        IMarketResolver markets,
        RawPublicDtos.GetBoardResponse? board = null,
        FakeBitflyerPrivateApi? privateApi = null,
        FakeBitflyerPrivateTradingApi? tradingApi = null)
    {
        var raw = new FakeBitflyerPublicApi(ticker, board, privateApi, tradingApi);
        return NormalizedApi.FromRaw(raw, markets);
    }

    public static INormalizedApi CreateTradingApi(
        FakeBitflyerPrivateTradingApi tradingApi,
        IMarketResolver markets,
        FakeBitflyerPrivateApi? privateApi = null) =>
        CreateNormalizedApi(
            new RawPublicDtos.GetTickerResponse { ProductCode = "BTC_JPY" },
            markets,
            privateApi: privateApi,
            tradingApi: tradingApi);

    public static IMarketResolver CreateResolver()
    {
        var resolver = new ExchangeInfoMarketResolver(new StubExchangeInfoApi(new ExchangeInfoDto(
            new[] { new ExchangeMarketInfo(Symbol.ParseOrThrow("BTC/JPY"), ProductCode.ParseOrThrow("BTC_JPY"), MarketType.ParseOrThrow("Spot")) },
            null,
            null,
            null)));
        return new NormalizedMarketResolver(resolver);
    }

    private sealed class StubExchangeInfoApi : IExchangeInfoProvider
    {
        private readonly ExchangeInfoDto _info;

        public StubExchangeInfoApi(ExchangeInfoDto info) => _info = info;

        public Task<Call<ExchangeInfoRequest, ExchangeInfoDto>> GetExchangeInfoAsync(
            ExchangeInfoRequest request,
            CancellationToken cancellationToken = default)
        {
            var meta = CallMeta.CreateInternal("Contracts", "StubExchangeInfoApi");
            var call = new Call<ExchangeInfoRequest, ExchangeInfoDto>(
                Id: CallId.New(),
                StartedAt: System.DateTimeOffset.UtcNow,
                Duration: System.TimeSpan.Zero,
                Request: new ExchangeInfoRequest(),
                Result: new CallResult<ExchangeInfoDto>.Ok(_info),
                Meta: meta);
            return Task.FromResult(call);
        }

    }
}
