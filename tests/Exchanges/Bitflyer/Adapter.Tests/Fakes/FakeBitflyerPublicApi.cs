using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Spec.CallCommon;
using RawProductCode = ExchangeApi.Exchanges.Bitflyer.Raw.Types.RawProductCode;

namespace ExchangeApi.Exchanges.Bitflyer.Tests.Fakes
{
    internal sealed class FakeBitflyerPublicApi : IBitflyerRawMarketDataApi
    {
        private readonly Ticker _response;
        private readonly Board? _board;
        private static readonly BitflyerRawRequest DefaultRequest =
            new BitflyerRawRequest("test", new Dictionary<string, string?>());

        public FakeBitflyerPublicApi(Ticker response, Board? board = null)
        {
            _response = response;
            _board = board;
        }

        public Task<Ticker> GetTickerAsync(
            RawProductCode productCode,
            bool useAliasPath = false,
            CancellationToken cancellationToken = default)
        {
            // Stage1 では BTC_JPY のみ想定なので、簡単なガードだけ入れておく
            if (productCode.Value != "BTC_JPY")
            {
                throw new System.ArgumentException($"Unexpected productCode: {productCode}", nameof(productCode));
            }

            return Task.FromResult(_response);
        }

        public Task<Board> GetBoardAsync(RawProductCode productCode, bool useAliasPath = false, CancellationToken cancellationToken = default)
        {
            if (_board is null)
            {
                throw new System.InvalidOperationException("Board response is not configured.");
            }

            if (productCode.Value != "BTC_JPY")
            {
                throw new System.ArgumentException($"Unexpected productCode: {productCode}", nameof(productCode));
            }

            return Task.FromResult(_board);
        }

        public Task<IReadOnlyList<ExecutionPublicResponse>> GetExecutionsAsync(
            RawProductCode productCode,
            int? count = null,
            long? before = null,
            long? after = null,
            bool useAliasPath = false,
            CancellationToken cancellationToken = default)
        {
            if (productCode.Value != "BTC_JPY")
            {
                throw new System.ArgumentException($"Unexpected productCode: {productCode}", nameof(productCode));
            }

            IReadOnlyList<ExecutionPublicResponse> executions = new[]
            {
                new ExecutionPublicResponse
                {
                    Id = 1,
                    ProductCode = new RawProductCode("BTC_JPY"),
                    Side = "BUY",
                    Price = 100m,
                    Size = 0.01m,
                    ExecDate = System.DateTimeOffset.UtcNow,
                }
            };

            return Task.FromResult(executions);
        }

        public Task<IReadOnlyList<Market>> GetMarketsAsync(string? region = null, bool useAliasPath = false, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<Market>>(new[] { new Market(new RawProductCode("BTC_JPY"), "BTC_JPY") });

        public Task<IReadOnlyList<Chat>> GetChatsAsync(string? fromDate = null, string? region = null, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<Chat>>(new[] { new Chat("n", "m", System.DateTimeOffset.UtcNow) });

        public Task<HealthResponse> GetHealthAsync(RawProductCode productCode, CancellationToken cancellationToken = default) =>
            Task.FromResult(new HealthResponse("NORMAL"));

        public Task<BoardStateResponse> GetBoardStateAsync(RawProductCode productCode, CancellationToken cancellationToken = default) =>
            Task.FromResult(new BoardStateResponse("NORMAL", "RUNNING", null));

        public Task<CorporateLeverageResponse> GetCorporateLeverageAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(new CorporateLeverageResponse(
                CurrentMax: 7.7m,
                CurrentStartDate: System.DateTimeOffset.UtcNow,
                NextMax: 7.65m,
                NextStartDate: System.DateTimeOffset.UtcNow.AddDays(7)));

        public Task<FundingRateResponse> GetFundingRateAsync(RawProductCode productCode, CancellationToken cancellationToken = default) =>
            Task.FromResult(new FundingRateResponse(0m, System.DateTimeOffset.UtcNow));

        public Task<BitflyerRawCall<Ticker, JsonElement>> GetTickerCallAsync(
            RawProductCode productCode,
            bool useAliasPath = false,
            CancellationToken cancellationToken = default)
        {
            var response = GetTickerAsync(productCode, useAliasPath, cancellationToken);
            return response.ContinueWith(task => MakeOkCall(task.Result), cancellationToken);
        }

        public Task<BitflyerRawCall<Board, JsonElement>> GetBoardCallAsync(
            RawProductCode productCode,
            bool useAliasPath = false,
            CancellationToken cancellationToken = default)
        {
            var response = GetBoardAsync(productCode, useAliasPath, cancellationToken);
            return response.ContinueWith(task => MakeOkCall(task.Result), cancellationToken);
        }

        public Task<BitflyerRawCall<IReadOnlyList<ExecutionPublicResponse>, JsonElement>> GetExecutionsCallAsync(
            RawProductCode productCode,
            int? count = null,
            long? before = null,
            long? after = null,
            bool useAliasPath = false,
            CancellationToken cancellationToken = default)
        {
            var response = GetExecutionsAsync(productCode, count, before, after, useAliasPath, cancellationToken);
            return response.ContinueWith(task => MakeOkCall(task.Result), cancellationToken);
        }

        public Task<BitflyerRawCall<IReadOnlyList<Market>, JsonElement>> GetMarketsCallAsync(
            string? region = null,
            bool useAliasPath = false,
            CancellationToken cancellationToken = default)
        {
            var response = GetMarketsAsync(region, useAliasPath, cancellationToken);
            return response.ContinueWith(task => MakeOkCall(task.Result), cancellationToken);
        }

        public Task<BitflyerRawCall<IReadOnlyList<Chat>, JsonElement>> GetChatsCallAsync(
            string? fromDate = null,
            string? region = null,
            CancellationToken cancellationToken = default)
        {
            var response = GetChatsAsync(fromDate, region, cancellationToken);
            return response.ContinueWith(task => MakeOkCall(task.Result), cancellationToken);
        }

        public Task<BitflyerRawCall<HealthResponse, JsonElement>> GetHealthCallAsync(
            RawProductCode productCode,
            CancellationToken cancellationToken = default)
        {
            var response = GetHealthAsync(productCode, cancellationToken);
            return response.ContinueWith(task => MakeOkCall(task.Result), cancellationToken);
        }

        public Task<BitflyerRawCall<BoardStateResponse, JsonElement>> GetBoardStateCallAsync(
            RawProductCode productCode,
            CancellationToken cancellationToken = default)
        {
            var response = GetBoardStateAsync(productCode, cancellationToken);
            return response.ContinueWith(task => MakeOkCall(task.Result), cancellationToken);
        }

        public Task<BitflyerRawCall<CorporateLeverageResponse, JsonElement>> GetCorporateLeverageCallAsync(
            CancellationToken cancellationToken = default)
        {
            var response = GetCorporateLeverageAsync(cancellationToken);
            return response.ContinueWith(task => MakeOkCall(task.Result), cancellationToken);
        }

        public Task<BitflyerRawCall<FundingRateResponse, JsonElement>> GetFundingRateCallAsync(
            RawProductCode productCode,
            CancellationToken cancellationToken = default)
        {
            var response = GetFundingRateAsync(productCode, cancellationToken);
            return response.ContinueWith(task => MakeOkCall(task.Result), cancellationToken);
        }

        private static BitflyerRawCall<TResponse, JsonElement> MakeOkCall<TResponse>(TResponse response) =>
            new(
                DefaultRequest,
                new Ok<TResponse, JsonElement>(response, 200),
                new CallMeta(System.DateTimeOffset.UtcNow, System.TimeSpan.Zero, null));
    }
}
