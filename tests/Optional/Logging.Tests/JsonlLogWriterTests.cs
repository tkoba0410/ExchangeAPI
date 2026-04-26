using System.Text.Json;
using ExchangeApi.Optional.Logging.Jsonl;
using ExchangeApi.Optional.Logging.Redaction;

namespace ExchangeApi.Optional.Logging.Tests;

public sealed class JsonlLogWriterTests
{
    [Fact]
    public async Task WriteAsync_WritesOneRedactedJsonLine()
    {
        var path = Path.Combine(Path.GetTempPath(), $"exchangeapi-jsonl-{Guid.NewGuid():N}", "log.jsonl");
        await using (var writer = new JsonlLogWriter(
                         path,
                         new JsonlLogWriterOptions
                         {
                             Redactor = new Redactor(new RedactionOptions { SensitiveValues = ["secret-value"] }),
                         }))
        {
            await writer.WriteAsync(new JsonlLogEntry
            {
                Timestamp = new DateTimeOffset(2026, 4, 26, 0, 0, 0, TimeSpan.Zero),
                Level = "info",
                Category = "test",
                EventName = "sample",
                Data = new { apiSecret = "secret-value", visible = "ok" },
            });
        }

        var lines = await File.ReadAllLinesAsync(path);
        Assert.Single(lines);
        Assert.DoesNotContain("secret-value", lines[0]);

        using var document = JsonDocument.Parse(lines[0]);
        Assert.Equal("sample", document.RootElement.GetProperty("eventName").GetString());
        Assert.Equal("ok", document.RootElement.GetProperty("data").GetProperty("visible").GetString());
    }
}
