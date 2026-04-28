using System.Text;
using System.Text.Json;
using ExchangeApi.Optional.Logging.Redaction;

namespace ExchangeApi.Optional.Logging.Realtime;

public sealed class RealtimeRawFrameLogRecordFactory
{
    private readonly Redactor _redactor;
    private readonly RealtimeRawFrameLogOptions _options;

    public RealtimeRawFrameLogRecordFactory(
        RealtimeRawFrameLogOptions? options = null,
        Redactor? redactor = null)
    {
        _options = options ?? new RealtimeRawFrameLogOptions();
        _redactor = redactor ?? new Redactor();
    }

    public RealtimeRawFrameLogRecord Create(
        string venue,
        string channel,
        DateTimeOffset receivedAt,
        string? rawFrameBody)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(venue);
        ArgumentException.ThrowIfNullOrWhiteSpace(channel);

        var payloadLength = rawFrameBody is null ? 0 : Encoding.UTF8.GetByteCount(rawFrameBody);
        if (!_options.IncludeBody)
        {
            return Skipped(venue, channel, receivedAt, payloadLength, "BodyLoggingDisabled");
        }

        if (rawFrameBody is null)
        {
            return Skipped(venue, channel, receivedAt, payloadLength, "BodyMissing");
        }

        if (payloadLength > _options.MaxRawFrameBodyBytes)
        {
            return Skipped(venue, channel, receivedAt, payloadLength, "PayloadTooLarge");
        }

        try
        {
            return new RealtimeRawFrameLogRecord
            {
                ReceivedAt = receivedAt,
                Venue = venue,
                Channel = channel,
                PayloadLength = payloadLength,
                Body = _redactor.RedactJson(rawFrameBody),
                BodySkipped = false,
            };
        }
        catch (JsonException)
        {
            return Skipped(venue, channel, receivedAt, payloadLength, "RedactionFailed");
        }
    }

    private static RealtimeRawFrameLogRecord Skipped(
        string venue,
        string channel,
        DateTimeOffset receivedAt,
        int payloadLength,
        string reason)
    {
        return new RealtimeRawFrameLogRecord
        {
            ReceivedAt = receivedAt,
            Venue = venue,
            Channel = channel,
            PayloadLength = payloadLength,
            BodySkipped = true,
            SkipReason = reason,
        };
    }
}
