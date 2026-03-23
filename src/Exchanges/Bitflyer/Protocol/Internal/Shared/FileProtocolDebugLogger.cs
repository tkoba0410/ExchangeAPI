using System.Text.Json;

namespace ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Shared;

public sealed class FileProtocolDebugLogger : IProtocolDebugLogger
{
    private readonly string _directoryPath;

    public FileProtocolDebugLogger(string directoryPath)
    {
        _directoryPath = directoryPath;
    }

    public async Task LogAsync(ProtocolDebugLogEntry entry, CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(_directoryPath);
        var fileName = $"{entry.TimestampUtc:yyyyMMdd-HHmmssfff}-{entry.EndpointId}-{Guid.NewGuid():N}.json";
        var filePath = Path.Combine(_directoryPath, fileName);
        var json = JsonSerializer.Serialize(entry, new JsonSerializerOptions
        {
            WriteIndented = true,
        });
        await File.WriteAllTextAsync(filePath, json, cancellationToken);
    }
}
