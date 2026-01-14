using System.Net;
using ExchangeApi.Contracts.Errors;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Mappers;
using Xunit;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Abstract;

public class BitflyerErrorMapperTests
{
    [Theory]
    [InlineData("INSUFFICIENT_FUNDS", ExchangeErrorCategory.Balance)]
    [InlineData("AUTHENTICATION_ERROR", ExchangeErrorCategory.Auth)]
    [InlineData("TOO_MANY_REQUESTS", ExchangeErrorCategory.RateLimit)]
    [InlineData("SERVICE_UNAVAILABLE", ExchangeErrorCategory.Server)]
    [InlineData("INVALID_ORDER", ExchangeErrorCategory.Request)]
    public void MapErrorCategory_FromExchangeCode(string code, ExchangeErrorCategory expected)
    {
        var category = BitflyerErrorMapper.MapErrorCategory(null, code);
        Assert.Equal(expected, category);
    }

    [Theory]
    [InlineData(HttpStatusCode.BadRequest, ExchangeErrorCategory.Request)]
    [InlineData(HttpStatusCode.Unauthorized, ExchangeErrorCategory.Auth)]
    [InlineData(HttpStatusCode.Forbidden, ExchangeErrorCategory.Auth)]
    [InlineData((HttpStatusCode)429, ExchangeErrorCategory.RateLimit)]
    [InlineData(HttpStatusCode.InternalServerError, ExchangeErrorCategory.Server)]
    public void MapErrorCategory_FromStatusCode(HttpStatusCode status, ExchangeErrorCategory expected)
    {
        var category = BitflyerErrorMapper.MapErrorCategory(status, null);
        Assert.Equal(expected, category);
    }
}
