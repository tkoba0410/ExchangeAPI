using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Dtos.Account;
using ExchangeApi.Contracts.Dtos.Common;
using ExchangeApi.Contracts.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Dtos.Market;
using ExchangeApi.Contracts.Dtos.Trading;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Contracts.Requests;
using ExchangeApi.Shared.Domain.Services;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Apis;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Call;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Exchanges.Bitflyer.Raw.Call;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Encoding;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private.Models;
using ExchangeApi.Exchanges.Bitflyer.Raw.Public;
using ExchangeApi.Exchanges.Bitflyer.Raw.Public.Models;
using ExchangeApi.Exchanges.Bitflyer.Raw.RawApi;
using ExchangeApi.Exchanges.Bitflyer.Raw.Requests;
using ExchangeApi.Contracts.Common.CallCommon;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Helpers;

internal static class BitflyerTestHelpers
{
    public static BitflyerNormalizedMarketDataFacade CreateMarketData(IBitflyerRawMarketDataApi raw) =>
        new(raw);

    public static IBitflyerNormalizedAccountApi CreateAccountApi(
        IBitflyerRawAccountApi accountApi,
        IExchangeMarketResolver markets) =>
        new BitflyerNormalizedAccountApi(accountApi, markets);

    public static IBitflyerNormalizedTradingApi CreateTradingApi(
        IBitflyerRawPrivateTradingApi tradingApi,
        IBitflyerPrivateApi accountApi,
        IExchangeMarketResolver markets) =>
        new BitflyerNormalizedTradingApi(tradingApi, accountApi, markets);

    public static IExchangeMarketResolver CreateResolver() =>
        new ExchangeInfoMarketResolver(new StubExchangeInfoApi(new ExchangeInfo(
            new[] { new ExchangeMarketInfo("BTC/JPY", "BTC_JPY", "Spot") },
            null,
            null,
            null)));

    private sealed class StubExchangeInfoApi : IExchangeInfoApi
    {
        private readonly ExchangeInfo _info;

        public StubExchangeInfoApi(ExchangeInfo info) => _info = info;

        public Task<Call<GetExchangeInfoRequest, ExchangeInfo>> GetExchangeInfoCallAsync(
            CancellationToken cancellationToken = default)
        {
            var meta = new CallMeta(
                Layer: "Contracts",
                Component: "StubExchangeInfoApi",
                Tags: null,
                Children: null);
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
