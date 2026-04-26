using System.Text.Json;

namespace ExchangeApi.Optional.Logging.Jsonl;

public sealed class JsonlLogWriter : IAsyncDisposable, IDisposable
{
    private readonly StreamWriter _writer;
    private readonly JsonlLogWriterOptions _options;
    private bool _disposed;

    public JsonlLogWriter(string path, JsonlLogWriterOptions? options = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path))!);
        _writer = new StreamWriter(new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Read));
        _options = options ?? new JsonlLogWriterOptions();
    }

    public async Task WriteAsync(JsonlLogEntry entry, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(entry);

        var json = JsonSerializer.Serialize(entry);
        await _writer.WriteLineAsync(_options.Redactor.RedactJson(json).AsMemory(), cancellationToken);
    }

    public Task FlushAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return _writer.FlushAsync(cancellationToken);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _writer.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        await _writer.DisposeAsync();
    }
}
