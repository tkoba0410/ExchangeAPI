using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Bitflyer;
using ExchangeApi.Bitflyer.Models;

namespace ExchangeApi.Bitflyer.Tests.Fakes
{
    internal sealed class FakeBitflyerPublicApi : IBitflyerPublicApi
    {
        private readonly BitflyerTickerRaw _response;

        public FakeBitflyerPublicApi(BitflyerTickerRaw response)
        {
            _response = response;
        }

        public Task<BitflyerTickerRaw> GetTickerRawAsync(
            string productCode,
            CancellationToken cancellationToken = default)
        {
            // Stage1 では BTC_JPY のみ想定なので、簡単なガードだけ入れておく
            if (productCode != "BTC_JPY")
            {
                throw new System.ArgumentException($"Unexpected productCode: {productCode}", nameof(productCode));
            }

            return Task.FromResult(_response);
        }
    }
}
