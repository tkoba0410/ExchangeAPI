using ExchangeApi.Optional.Logging.Redaction;

namespace ExchangeApi.Optional.Logging.Jsonl;

public sealed class JsonlLogWriterOptions
{
    public Redactor Redactor { get; init; } = new();
}
