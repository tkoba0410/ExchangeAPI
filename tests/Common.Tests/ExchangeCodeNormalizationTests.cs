using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using Xunit;

namespace ExchangeApi.Tests.Common.Tests;

public sealed class ExchangeCodeNormalizationTests
{
    [Theory]
    [InlineData("bitflyer")]
    [InlineData("BitFlyer")]
    [InlineData("BIT-FLYER")]
    [InlineData("bit_flyer")]
    [InlineData(" bit flyer ")]
    public void Parse_NormalizesCommonVariants(string input)
    {
        var result = ExchangeCodeParser.Parse(input);
        Assert.Equal(ExchangeCode.Bitflyer, result);
    }

    [Fact]
    public void Formatter_ReturnsCanonicalId()
    {
        var id = ExchangeCodeFormatter.ToCanonicalId(ExchangeCode.Bitflyer);
        Assert.Equal("bitflyer", id);
    }
}
