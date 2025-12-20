using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using RawProductCode = ExchangeApi.Exchanges.Bitflyer.Raw.ProductCode;

namespace ExchangeApi.Exchanges.Bitflyer.Tests.Fakes
{
    internal sealed class FakeBitflyerPublicApi : IBitflyerPublicApi
    {
        private readonly Ticker _response;
        private readonly Board? _board;

        public FakeBitflyerPublicApi(Ticker response, Board? board = null)
        {
            _response = response;
            _board = board;
        }

        public Task<Ticker> GetTickerRawAsync(
            string productCode,
            bool useAliasPath = false,
            CancellationToken cancellationToken = default)
        {
            // Stage1 では BTC_JPY のみ想定なので、簡単なガードだけ入れておく
            if (productCode != "BTC_JPY")
            {
                throw new System.ArgumentException($"Unexpected productCode: {productCode}", nameof(productCode));
            }

            return Task.FromResult(_response);
        }

        public Task<Board> GetBoardRawAsync(string productCode, bool useAliasPath = false, CancellationToken cancellationToken = default)
        {
            if (_board is null)
            {
                throw new System.InvalidOperationException("Board response is not configured.");
            }

            if (productCode != "BTC_JPY")
            {
                throw new System.ArgumentException($"Unexpected productCode: {productCode}", nameof(productCode));
            }

            return Task.FromResult(_board);
        }

        public Task<IReadOnlyList<ExecutionPublicResponse>> GetExecutionsRawAsync(
            string productCode,
            int? count = null,
            long? before = null,
            long? after = null,
            bool useAliasPath = false,
            CancellationToken cancellationToken = default)
        {
            if (productCode != "BTC_JPY")
            {
                throw new System.ArgumentException($"Unexpected productCode: {productCode}", nameof(productCode));
            }

            IReadOnlyList<ExecutionPublicResponse> executions = new[]
            {
                new ExecutionPublicResponse
                {
                    Id = 1,
                    ProductCode = RawProductCode.BtcJpy,
                    Side = Side.Buy,
                    Price = 100m,
                    Size = 0.01m,
                    ExecDate = System.DateTimeOffset.UtcNow,
                }
            };

            return Task.FromResult(executions);
        }

        public Task<IReadOnlyList<Market>> GetMarketsAsync(string? region = null, bool useAliasPath = false, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<Market>>(new[] { new Market(RawProductCode.BtcJpy, "BTC_JPY") });

        public Task<IReadOnlyList<Chat>> GetChatsAsync(string? fromDate = null, string? region = null, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<Chat>>(new[] { new Chat("n", "m", System.DateTimeOffset.UtcNow) });

        public Task<HealthResponse> GetHealthAsync(string productCode, CancellationToken cancellationToken = default) =>
            Task.FromResult(new HealthResponse("NORMAL"));

        public Task<BoardStateResponse> GetBoardStateAsync(string productCode, CancellationToken cancellationToken = default) =>
            Task.FromResult(new BoardStateResponse("NORMAL", "RUNNING", null));

        public Task<CorporateLeverageResponse> GetCorporateLeverageAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(new CorporateLeverageResponse(
                CurrentMax: 7.7m,
                CurrentStartDate: System.DateTimeOffset.UtcNow,
                NextMax: 7.65m,
                NextStartDate: System.DateTimeOffset.UtcNow.AddDays(7)));

        public Task<FundingRateResponse> GetFundingRateAsync(string productCode, CancellationToken cancellationToken = default) =>
            Task.FromResult(new FundingRateResponse(0m, System.DateTimeOffset.UtcNow));
    }
}
