using ExchangeApi.Primitives.Errors;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Exchanges.Bittrade.Api.Adapter.Internal;

namespace ExchangeApi.Tests.Exchanges.Bittrade.Adapter.Tests;

public sealed class BittradeApiCallMapperTests
{
    [Theory]
    [InlineData(CallErrorKind.Http, 500, ExchangeErrorCategory.Server)]
    [InlineData(CallErrorKind.Transport, null, ExchangeErrorCategory.Network)]
    public void ToExchangeErrorCategory_maps_call_error_kinds(
        CallErrorKind kind,
        int? statusCode,
        ExchangeErrorCategory expected)
    {
        var error = new CallError(kind, "test", HttpStatus: statusCode);
        var category = ApiCallMapper.ToExchangeErrorCategory(error);

        Assert.Equal(expected, category);
    }
}
