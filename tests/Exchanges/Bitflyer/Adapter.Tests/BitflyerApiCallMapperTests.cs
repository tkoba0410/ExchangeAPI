using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Spec.CallCommon;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Internal;

namespace ExchangeApi.Exchanges.Bitflyer.Tests;

public sealed class BitflyerApiCallMapperTests
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
