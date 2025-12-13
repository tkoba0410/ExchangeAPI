using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Exchange.Bitflyer.Raw;

namespace Exchange.Bitflyer.Tests.Fakes
{
    internal sealed class FakeBitflyerPublicApi : IBitflyerPublicApi
    {
        private readonly BitflyerTickerRaw _response;
        private readonly BitflyerBoardRaw? _board;

        public FakeBitflyerPublicApi(BitflyerTickerRaw response, BitflyerBoardRaw? board = null)
        {
            _response = response;
            _board = board;
        }

        public Task<BitflyerTickerRaw> GetTickerRawAsync(
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

        public Task<BitflyerBoardRaw> GetBoardRawAsync(string productCode, bool useAliasPath = false, CancellationToken cancellationToken = default)
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

        public Task<IReadOnlyList<BitflyerExecutionPublicResponse>> GetExecutionsRawAsync(
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

            IReadOnlyList<BitflyerExecutionPublicResponse> executions = new[]
            {
                new BitflyerExecutionPublicResponse
                {
                    Id = 1,
                    ProductCode = productCode,
                    Side = "BUY",
                    Price = 100m,
                    Size = 0.01m,
                    ExecDate = System.DateTimeOffset.UtcNow,
                }
            };

            return Task.FromResult(executions);
        }

        public Task<IReadOnlyList<BitflyerMarket>> GetMarketsAsync(string? region = null, bool useAliasPath = false, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<BitflyerMarket>>(new[] { new BitflyerMarket("BTC_JPY", "BTC_JPY") });

        public Task<IReadOnlyList<BitflyerChat>> GetChatsAsync(string? fromDate = null, string? region = null, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<BitflyerChat>>(new[] { new BitflyerChat("n", "m", System.DateTimeOffset.UtcNow) });

        public Task<BitflyerHealthResponse> GetHealthAsync(string productCode, CancellationToken cancellationToken = default) =>
            Task.FromResult(new BitflyerHealthResponse("NORMAL"));

        public Task<BitflyerBoardStateResponse> GetBoardStateAsync(string productCode, CancellationToken cancellationToken = default) =>
            Task.FromResult(new BitflyerBoardStateResponse("NORMAL", "RUNNING", null));

        public Task<JsonElement> GetCorporateLeverageAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(JsonDocument.Parse("{}").RootElement);

        public Task<BitflyerFundingRateResponse> GetFundingRateAsync(string productCode, CancellationToken cancellationToken = default) =>
            Task.FromResult(new BitflyerFundingRateResponse(productCode, 0m));
    }
}
