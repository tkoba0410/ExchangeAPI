using System;
using ExchangeApi.Contracts.Common.Dtos;
using Xunit;

namespace ExchangeApi.Tests.Common.Tests.Contracts;

public class BatchResultTests
{
    [Fact]
    public void BatchResult_CanRepresent_SuccessOnly()
    {
        var result = BatchResult<int>.Success(1, 2, 3);

        Assert.True(result.HasSuccesses);
        Assert.False(result.HasErrors);
        Assert.True(result.IsSuccessOnly);
        Assert.False(result.IsFailureOnly);
        Assert.False(result.IsPartialSuccess);
    }

    [Fact]
    public void BatchResult_CanRepresent_FailureOnly()
    {
        var result = BatchResult<int>.Failure(
            new BatchError("Ticker", BatchErrorKind.Transient, "timeout"));

        Assert.False(result.HasSuccesses);
        Assert.True(result.HasErrors);
        Assert.False(result.IsSuccessOnly);
        Assert.True(result.IsFailureOnly);
        Assert.False(result.IsPartialSuccess);
    }

    [Fact]
    public void BatchResult_CanRepresent_PartialSuccess()
    {
        var result = BatchResult<int>.From(
            successes: new[] { 1, 2 },
            errors: new[]
            {
                new BatchError("Board", BatchErrorKind.Transient, "rate limited")
            });

        Assert.True(result.HasSuccesses);
        Assert.True(result.HasErrors);
        Assert.False(result.IsSuccessOnly);
        Assert.False(result.IsFailureOnly);
        Assert.True(result.IsPartialSuccess);
    }

    [Fact]
    public void BatchError_Validates_RequiredFields()
    {
        Assert.Throws<ArgumentException>(() => new BatchError("", BatchErrorKind.Unknown, "x"));
        Assert.Throws<ArgumentException>(() => new BatchError("Ticker", BatchErrorKind.Unknown, ""));
    }
}
