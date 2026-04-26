using ExchangeApi.Optional.Logging.Redaction;

namespace ExchangeApi.Optional.Logging.Tests;

public sealed class RedactorTests
{
    [Fact]
    public void RedactJson_RedactsSensitiveKeysRecursively()
    {
        var redactor = new Redactor();
        var json = """{"apiKey":"key-123","nested":{"apiSecret":"secret-456","signature":"sig-789"},"safe":"ok"}""";

        var redacted = redactor.RedactJson(json);

        Assert.DoesNotContain("key-123", redacted);
        Assert.DoesNotContain("secret-456", redacted);
        Assert.DoesNotContain("sig-789", redacted);
        Assert.Contains("ok", redacted);
    }

    [Fact]
    public void RedactText_RedactsKnownValuesAndHeaderLikePairs()
    {
        var redactor = new Redactor(new RedactionOptions
        {
            SensitiveValues = ["plain-secret"],
        });

        var redacted = redactor.RedactText("Authorization: Bearer token ACCESS-SIGN=abc plain-secret");

        Assert.DoesNotContain("Bearer token", redacted);
        Assert.DoesNotContain("abc", redacted);
        Assert.DoesNotContain("plain-secret", redacted);
        Assert.Contains("[REDACTED]", redacted);
    }
}
