using System;
using ExchangeApi.Contracts.Common.Errors;
using Xunit;

namespace ExchangeApi.Tests.Common.Tests.Contracts;

public class ExchangeApiExceptionTests
{
    [Fact]
    public void CanCreateWithMessage()
    {
        const string message = "error";
        var ex = new ExchangeApiException(message);

        Assert.Equal(message, ex.Message);
    }

    [Fact]
    public void CanCreateWithInnerException()
    {
        var inner = new InvalidOperationException("inner");
        var ex = new ExchangeApiException("outer", inner);

        Assert.Equal("outer", ex.Message);
        Assert.Same(inner, ex.InnerException);
    }
}
