using System.Text.Json.Serialization;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Realtime.Models;

public interface IRealtimeMessage
{
    string Channel { get; }

    [JsonConverter(typeof(BitflyerUtcTimestampJsonConverter))]
    DateTimeOffset ReceivedAt { get; }
}
