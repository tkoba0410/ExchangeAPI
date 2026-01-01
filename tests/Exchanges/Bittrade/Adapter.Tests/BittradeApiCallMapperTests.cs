using ExchangeApi.Contracts.Call;
using ExchangeApi.Exchanges.Bittrade.Adapter.Internal;

namespace ExchangeApi.Exchanges.Bittrade.Tests;

public sealed class BittradeApiCallMapperTests
{
    [Theory]
    [InlineData(404, ApiErrorKind.NotFound)]
    [InlineData(429, ApiErrorKind.RateLimit)]
    public void Classify_maps_http_statuses(int statusCode, ApiErrorKind expected)
    {
        var kind = ApiCallMapper.Classify(statusCode, exchangeErrorCode: null, message: null);

        Assert.Equal(expected, kind);
    }
}
