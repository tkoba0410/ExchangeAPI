using ExchangeApi.Optional.Logging.Evidence;

namespace ExchangeApi.Optional.Logging.Tests;

public sealed class EvidenceRunLabelTests
{
    [Theory]
    [InlineData("../secret", "secret")]
    [InlineData("live run", "live-run")]
    [InlineData("v2.1_check", "v2.1_check")]
    public void Constructor_SanitizesPathUnsafeCharacters(string input, string expected)
    {
        var label = new EvidenceRunLabel(input);

        Assert.Equal(expected, label.Value);
    }
}
