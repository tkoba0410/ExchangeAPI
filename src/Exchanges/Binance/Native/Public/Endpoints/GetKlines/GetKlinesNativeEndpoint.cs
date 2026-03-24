using ExchangeApi.Exchanges.Binance.Native.Internal.Shared;
using ExchangeApi.Exchanges.Binance.Protocol.Public.Endpoints.GetKlines;
using ExchangeApi.Exchanges.Binance.Vocabulary;
using ExchangeApi.Primitives.Calls;

namespace ExchangeApi.Exchanges.Binance.Native.Public.Endpoints.GetKlines;

public interface IGetKlinesNativeEndpoint
{
    Task<Call<GetKlinesRequest, IReadOnlyList<GetKlines.Item>>> CallAsync(
        GetKlinesRequest request,
        CancellationToken cancellationToken = default);
}

public sealed class GetKlinesNativeEndpoint : IGetKlinesNativeEndpoint
{
    private static readonly HashSet<string> SupportedIntervals =
    [
        "1s",
        "1m", "3m", "5m", "15m", "30m",
        "1h", "2h", "4h", "6h", "8h", "12h",
        "1d", "3d",
        "1w",
        "1M",
    ];

    private readonly IGetKlinesProtocolEndpoint _protocolEndpoint;

    public GetKlinesNativeEndpoint(IGetKlinesProtocolEndpoint protocolEndpoint)
    {
        _protocolEndpoint = protocolEndpoint;
    }

    public async Task<Call<GetKlinesRequest, IReadOnlyList<GetKlines.Item>>> CallAsync(
        GetKlinesRequest request,
        CancellationToken cancellationToken = default)
    {
        var semanticError = Validate(request);
        if (semanticError is not null)
        {
            return NativeCallFactory.Failure<GetKlinesRequest, IReadOnlyList<GetKlines.Item>>(
                request,
                semanticError,
                protocolCall: null,
                BinanceEndpointIds.GetKlines);
        }

        var protocolCall = await _protocolEndpoint.SendAsync(
            request.Symbol,
            request.Interval,
            request.StartTime,
            request.EndTime,
            request.TimeZone,
            request.Limit,
            cancellationToken);

        if (!protocolCall.IsSuccess)
        {
            return NativeCallFactory.Failure<GetKlinesRequest, IReadOnlyList<GetKlines.Item>>(
                request,
                protocolCall.Error!,
                protocolCall,
                BinanceEndpointIds.GetKlines);
        }

        if (protocolCall.Response!.StatusCode != 200)
        {
            return NativeCallFactory.Failure<GetKlinesRequest, IReadOnlyList<GetKlines.Item>>(
                request,
                new CallError
                {
                    Kind = CallErrorKinds.Http,
                    Message = $"Expected status 200 but got {protocolCall.Response.StatusCode}.",
                },
                protocolCall,
                BinanceEndpointIds.GetKlines);
        }

        try
        {
            var root = JsonValueReader.EnsureArray(protocolCall.Response.BodyText);
            var response = new List<GetKlines.Item>();

            foreach (var entry in root.EnumerateArray())
            {
                JsonValueReader.EnsureArrayLength(entry, 12, "Kline item");
                response.Add(new GetKlines.Item
                {
                    OpenTime = JsonValueReader.ReadRequiredInt64At(entry, 0, "Kline item"),
                    OpenPrice = JsonValueReader.ReadRequiredDecimalStringAt(entry, 1, "Kline item"),
                    HighPrice = JsonValueReader.ReadRequiredDecimalStringAt(entry, 2, "Kline item"),
                    LowPrice = JsonValueReader.ReadRequiredDecimalStringAt(entry, 3, "Kline item"),
                    ClosePrice = JsonValueReader.ReadRequiredDecimalStringAt(entry, 4, "Kline item"),
                    Volume = JsonValueReader.ReadRequiredDecimalStringAt(entry, 5, "Kline item"),
                    CloseTime = JsonValueReader.ReadRequiredInt64At(entry, 6, "Kline item"),
                    QuoteAssetVolume = JsonValueReader.ReadRequiredDecimalStringAt(entry, 7, "Kline item"),
                    NumberOfTrades = JsonValueReader.ReadRequiredInt32At(entry, 8, "Kline item"),
                    TakerBuyBaseAssetVolume = JsonValueReader.ReadRequiredDecimalStringAt(entry, 9, "Kline item"),
                    TakerBuyQuoteAssetVolume = JsonValueReader.ReadRequiredDecimalStringAt(entry, 10, "Kline item"),
                });
            }

            return NativeCallFactory.Success(request, (IReadOnlyList<GetKlines.Item>)response, protocolCall);
        }
        catch (CodecException ex)
        {
            return NativeCallFactory.Failure<GetKlinesRequest, IReadOnlyList<GetKlines.Item>>(
                request,
                new CallError
                {
                    Kind = CallErrorKinds.Codec,
                    Message = ex.Message,
                },
                protocolCall,
                BinanceEndpointIds.GetKlines);
        }
    }

    private static CallError? Validate(GetKlinesRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Symbol))
        {
            return Semantic("Symbol must not be blank.");
        }

        if (string.IsNullOrWhiteSpace(request.Interval))
        {
            return Semantic("Interval must not be blank.");
        }

        if (!SupportedIntervals.Contains(request.Interval))
        {
            return Semantic("Interval is not supported.");
        }

        if (request.Limit is < 1 or > 1000)
        {
            return Semantic("Limit must be between 1 and 1000.");
        }

        if (request.StartTime is not null && request.EndTime is not null && request.StartTime > request.EndTime)
        {
            return Semantic("StartTime must be less than or equal to EndTime.");
        }

        if (request.TimeZone is not null && !TryValidateTimeZone(request.TimeZone))
        {
            return Semantic("TimeZone must be within [-12:00, +14:00] and use a supported format.");
        }

        return null;
    }

    private static bool TryValidateTimeZone(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var trimmed = value.Trim();
        var sign = 1;
        if (trimmed[0] == '+')
        {
            trimmed = trimmed[1..];
        }
        else if (trimmed[0] == '-')
        {
            sign = -1;
            trimmed = trimmed[1..];
        }

        var parts = trimmed.Split(':');
        if (parts.Length is < 1 or > 2)
        {
            return false;
        }

        if (!int.TryParse(parts[0], System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out var hours))
        {
            return false;
        }

        var minutes = 0;
        if (parts.Length == 2)
        {
            if (parts[1].Length != 2 ||
                !int.TryParse(parts[1], System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out minutes))
            {
                return false;
            }
        }

        if (minutes is < 0 or >= 60)
        {
            return false;
        }

        var totalMinutes = sign * ((hours * 60) + minutes);
        return totalMinutes >= -12 * 60 && totalMinutes <= 14 * 60;
    }

    private static CallError Semantic(string message)
    {
        return new CallError
        {
            Kind = CallErrorKinds.Semantic,
            Message = message,
        };
    }
}
